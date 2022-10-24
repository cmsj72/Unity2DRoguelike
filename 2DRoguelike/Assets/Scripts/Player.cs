using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MovingObject
{
    //  플레이어가 벽을 부술 때 벽 오브젝트에 적용될 데미지
    public int wallDamage = 1;
    //  음식이나 소다를 집었을때 플레이어 스코어에 더해질 점수
    public int pointsPerFood = 10;
    public int pointsPerSoda = 20;
    public float restartLevelDelay = 1f;

    private Animator animator;
    //  레벨을 바꾸면서 스코어를 다시 게임 매니저로 입력해 넣기 전에,
    //  해당 레벨 동안의 플레이어 스코어를 저장
    private int food;

    //  MovingObject에 있는 Start와는 다르게
    //  Player의 Start를 구현하기 위해 protected override 
    protected override void Start()
    {
        animator = GetComponent<Animator>();

        //  플레이어는 해당 레벨 동안 음식 점수를 관리할 수 있고,
        //  레벨이 바뀔 때 게임매니저로 다시 저장할 수 있음
        food = GameManager.instance.playerFoodPoints;

        base.Start();
    }

    private void OnDisable()
    {
        GameManager.instance.playerFoodPoints = food;
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.instance.playersTurn) return;

        //  수평이나 수직으로 움직이는 방향을 1이나 -1로 저장해서 사용
        int horizontal = 0;
        int vertical = 0;

        //  따로 빌드하기 위해, 현재 조작 코드는 키보드나 컨트롤러의 입력에 기반
        //  나중에 모바일이나 터치 스크린 입력을 받는 버전을 만들 예정
        horizontal = (int)Input.GetAxis("Horizontal");
        vertical = (int)Input.GetAxis("Vertical");

        //  만약 horizontal이 0이 아니라면 수평으로 이동중이기 때문에
        //  대각선으로 움직이는 것을 막기위해 수직 값을 0으로 
        if (horizontal != 0)
            vertical = 0;

        //  둘중 하나라도 0이 아니면 움직이려 한다는 뜻이기 때문에 AttemptMove 호출
        if (horizontal != 0 || vertical != 0)
            //  일반형 변수 Wall을 입력
            //  플레이어가 상호작용할 수 있는 벽에 대면할지도 모른다는 의미
            //  일반형 입력 T ( <T> )를 줌으로써 우리가 함수를 호출할 때 상호작용할 컴포넌트를 특정할 수 있다.
            //  이 경우에는 플레이어 스크립트에서 우리는 벽과 상호작용 할 것을 예상할 수 있고,
            //  적 스크립트의 경우에는 플레이어와 상호작용 할 것을 예상해 특정할 수 있다.
            AttemptMove<Wall>(horizontal, vertical);


    }

    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        //  플레이어가 움직일 때 마다 음식 점수를 1씩 잃는다
        food--;
        base.AttemptMove<T>(xDir, yDir);

        RaycastHit2D hit;

        CheckIfGameOver();

        GameManager.instance.playersTurn = false;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Exit")
        {
            //  출구 트리거와 충돌 후 1초 후에 이 함수를 호출
            //  → 1초간 정지하고 레벨을 다시 시작함을 의미
            //  Invoke 함수
            Invoke("Restart", restartLevelDelay);
            enabled = false;
        }
        else if (other.tag == "Food")
        {
            food += pointsPerFood;
            other.gameObject.SetActive(false);
        }
        else if(other.tag == "Soda")
        {
            food += pointsPerSoda;
            other.gameObject.SetActive(false);
        }
    }
    protected override void OnCantMove<T>(T component)
    {
        Wall hitWall = component as Wall;
        hitWall.DamageWall(wallDamage);
        animator.SetTrigger("playerChop");
    }

    //  레벨을 다시 로드(플레이어가 출구 오브젝트와 충돌했을 때 Restart를 호출, 다음 레벨로 넘어가는걸 의미)
    private void Restart()
    {   
        //Application.LoadLevel 과 Application.loadedLevel 은 이제 사용되지 않는다.
        //Application.LoadLevel(Application.loadedLevel);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoseFood(int loss)
    {
        animator.SetTrigger("playerHit");
        food -= loss;
        CheckIfGameOver();
    }

    private void CheckIfGameOver()
    {
        if(food <= 0)
        {
            GameManager.instance.GameOver();
        }
    }    
}
