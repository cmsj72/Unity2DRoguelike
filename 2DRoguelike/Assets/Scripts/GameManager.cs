using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //  싱글턴은 게임 상에 언제나 단 하나의 인스턴스만 존재할 수 있는 오브젝트
    //  게임 매니저는 1개만 있으면 되기 때문에 싱글턴화 한다.
    public static GameManager instance = null;
    public BoardManager boardScript;

    private int level = 3;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        //  게임 매니저가 신이 넘어가면서도 계속 점수를 계산하게 해야하기 때문에
        //  신이 넘어갈 때 파괴되면 안된다.
        DontDestroyOnLoad(gameObject);
        //  컴포넌트를 레퍼런스로 들고와 저장(call by reference : 값을 복사하는 것이 아닌 실제 오브젝트 대상 그 자체를 가져옴)
        boardScript = GetComponent<BoardManager>();
        InitGame();
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
