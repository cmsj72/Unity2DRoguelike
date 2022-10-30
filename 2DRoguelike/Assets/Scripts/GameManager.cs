using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //  ������ ���۵Ǳ� ���� �� ������ ����� �ð�
    public float levelStartDelay = 2f;
    //  �̱����� ���� �� ������ �� �ϳ��� �ν��Ͻ��� ������ �� �ִ� ������Ʈ
    //  ���� �Ŵ����� 1���� ������ �Ǳ� ������ �̱���ȭ �Ѵ�.
    public static GameManager instance = null;
    public BoardManager boardScript;
    public int playerFoodPoints = 100;
    //  ������ public ������ �ν����Ϳ��� �����
    [HideInInspector] public bool playersTurn = true;
    //  �� ���̿� ������ �� ���� ������� ��Ÿ����.
    public float turnDelay = 0.1f;

    private Text levelText;
    private GameObject levelImage;
    private int level = 1;
    private List<Enemy> enemies;
    private bool enemiesMoving;
    //  ���� ������ ����� ������ üũ�ϰ�, ���带 ����� �߿��� �÷��̾ �����̴� ���� ����
    private bool doingSetup;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        //  ���� �Ŵ����� ���� �Ѿ�鼭�� ��� ������ ����ϰ� �ؾ��ϱ� ������
        //  ���� �Ѿ �� �ı��Ǹ� �ȵȴ�.
        DontDestroyOnLoad(gameObject);
        enemies = new List<Enemy>();
        //  ������Ʈ�� ���۷����� ���� ����(call by reference : ���� �����ϴ� ���� �ƴ� ���� ������Ʈ ��� �� ��ü�� ������)
        boardScript = GetComponent<BoardManager>();
        InitGame();
    }

    //  ����Ƽ API �⺻ ���� �Լ�
    //  ���� �ε� �� �� ���� ȣ��, ���� ���� ���ڸ� ���ϰ� �� ������ �ε������ InitGame �Լ��� ȣ���ϴµ� ���
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

        //  ���� ������ üũ => ù ����
        if (enemies.Count == 0)
        {
            //  ����ϴ� ���� ������ �ϴ� �÷��̾ ��ٸ��� �Ѵ�.
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
