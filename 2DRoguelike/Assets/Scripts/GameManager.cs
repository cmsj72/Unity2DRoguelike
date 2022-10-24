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

    private int level = 3;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        //  ���� �Ŵ����� ���� �Ѿ�鼭�� ��� ������ ����ϰ� �ؾ��ϱ� ������
        //  ���� �Ѿ �� �ı��Ǹ� �ȵȴ�.
        DontDestroyOnLoad(gameObject);
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
        boardScript.SetupScene(level);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
