using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //  �̱����� ���� �� ������ �� �ϳ��� �ν��Ͻ��� ������ �� �ִ� ������Ʈ
    //  ���� �Ŵ����� 1���� ������ �Ǳ� ������ �̱���ȭ �Ѵ�.
    public static GameManager instance = null;
    public BoardManager boardScript;
    public int playerFoodPoints = 100;
    //  ������ public ������ �ν����Ϳ��� �����
    [HideInInspector] public bool playersTurn = true;
    //  �� ���̿� ������ �� ���� ������� ��Ÿ����.
    public float turnDelay = 0.1f;

    private int level = 3;
    private List<Enemy> enemies;
    private bool enemiesMoving;

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
