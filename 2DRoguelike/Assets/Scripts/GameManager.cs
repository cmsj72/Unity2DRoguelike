using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //  레벨이 시작되기 전에 초 단위로 대기할 시간
    public float levelStartDelay = 2f;
    //  싱글턴은 게임 상에 언제나 단 하나의 인스턴스만 존재할 수 있는 오브젝트
    //  게임 매니저는 1개만 있으면 되기 때문에 싱글턴화 한다.
    public static GameManager instance = null;
    public BoardManager boardScript;
    public int playerFoodPoints = 100;
    //  변수가 public 이지만 인스펙터에서 숨긴다
    [HideInInspector] public bool playersTurn = true;
    //  턴 사이에 게임이 얼마 동안 대기할지 나타낸다.
    public float turnDelay = 0.1f;

    private Text levelText;
    private GameObject levelImage;
    private int level = 1;
    private List<Enemy> enemies;
    private bool enemiesMoving;
    //  게임 정보를 만드는 중인지 체크하고, 보드를 만드는 중에는 플레이어가 움직이는 것을 방지
    private bool doingSetup;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        //  게임 매니저가 신이 넘어가면서도 계속 점수를 계산하게 해야하기 때문에
        //  신이 넘어갈 때 파괴되면 안된다.
        DontDestroyOnLoad(gameObject);
        enemies = new List<Enemy>();
        //  컴포넌트를 레퍼런스로 들고와 저장(call by reference : 값을 복사하는 것이 아닌 실제 오브젝트 대상 그 자체를 가져옴)
        boardScript = GetComponent<BoardManager>();
        InitGame();
    }

    //  유니티 API 기본 제공 함수
    //  씬이 로드 될 때 마다 호출, 현재 레벨 숫자를 더하고 새 레벨이 로드됬을때 InitGame 함수를 호출하는데 사용
    private void OnLevelWasLoaded(int index)
    {

    }

    public void GameOver()
    {
        enabled = false;
    }

    void InitGame()
    {
        enemies.Clear();
        boardScript.SetupScene(level);
    }

    // Update is called once per frame
    void Update()
    {
        if (playersTurn || enemiesMoving)
            return;

        StartCoroutine(MoveEnemies());
    }

    public void AddEnemyToList(Enemy script)
    {
        enemies.Add(script);
    }

    IEnumerator MoveEnemies()
    {
        enemiesMoving = true;
        yield return new WaitForSeconds(turnDelay);

        //  적이 없는지 체크 => 첫 레벨
        if (enemies.Count == 0)
        {
            //  대기하는 적이 없지만 일단 플레이어가 기다리게 한다.
            yield return new WaitForSeconds(turnDelay);
        }
        for(int i = 0; i < enemies.Count; i++)
        {
            enemies[i].MoveEnemy();
            yield return new WaitForSeconds(enemies[i].moveTime);
        }

        playersTurn = true;
        enemiesMoving = false;
    }
}
