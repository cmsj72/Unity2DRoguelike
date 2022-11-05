using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MovingObject
{
    //  플레이어가 벽을 부술 때 벽 오브젝트에 적용될 데미지
    public int wallDamage = 1;
    //  음식이나 소다를 집었을때 플레이어 스코어에 더해질 점수
    public int pointsPerFood = 10;
    public int pointsPerSoda = 20;
    public float restartLevelDelay = 1f;
    public Text foodText;
    public AudioClip moveSound1;
    public AudioClip moveSound2;
    public AudioClip eatSound1;
    public AudioClip eatSound2;
    public AudioClip drinkSound1;
    public AudioClip drinkSound2;
    public AudioClip gameOverSound1;

    private Animator animator;

    //  레벨을 바꾸면서 스코어를 다시 게임 매니저로 입력해 넣기 전에,
    //  해당 레벨 동안의 플레이어 스코어를 저장
    private int food;

    //  터치스크린에 플레이어의 터치가 시작되는 지점을 저장하기 위해 사용
    //  -Vector2.one 스크린 밖의 위치를 의미 → 터치 입력이 있는지 체크하기 위해 사용하는 상태
    //  이를 위해 touchOrigin의 값을 바꿔줄 실제 터치가 이루어지기 전 까지는 거짓 값으로 초기화해둔다.
    private Vector2 touchOrigin = -Vector2.one;

    //  MovingObject에 있는 Start와는 다르게
    //  Player의 Start를 구현하기 위해 protected override 
    protected override void Start()
    {
        animator = GetComponent<Animator>();

        //  플레이어는 해당 레벨 동안 음식 점수를 관리할 수 있고,
        //  레벨이 바뀔 때 게임매니저로 다시 저장할 수 있음
        food = GameManager.instance.playerFoodPoints;

        foodText.text = "Food: " + food;

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
        
        //  에디터에서 키보드로 계속 테스트 해보고 싶다면 빌드 타겟을 스탠드 얼론으로 바꿀수도 있고,
        //  UNITY_EDITOR를 추가해주면된다.
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBPLAYER

        //  따로 빌드하기 위해, 현재 조작 코드는 키보드나 컨트롤러의 입력에 기반
        //  나중에 모바일이나 터치 스크린 입력을 받는 버전을 만들 예정
        horizontal = (int)(Input.GetAxisRaw("Horizontal"));
        vertical = (int)(Input.GetAxisRaw("Vertical"));

        //  만약 horizontal이 0이 아니라면 수평으로 이동중이기 때문에
        //  대각선으로 움직이는 것을 막기위해 수직 값을 0으로 
        if (horizontal != 0)
            vertical = 0;

#else
        //  Input.touchCount이 0보다 크면 입력 시스템이 하나 이상의 터치를 감지했다는 의미
        if (Input.touchCount > 0)
        {
            //  첫번째 터치 지점만 잡아내고, 나머지 터치들은 전부 무시
            //  이 게임은 한 방향에 대한 한 손가락의 스와이핑만 지원할 것이기 때문
            Touch myTouch = Input.touches[0];

            //  잡아낸 터치의 페이즈가 확실히 '터치가 막 시작'(Began) 상태인지 체크
            if(myTouch.phase == TouchPhase.Began)
            {
                touchOrigin = myTouch.position;
            }
            //  touchOrigin을 처음에 -1로 초기화 해놨기 때문에
            //  손가락이 스크린에서 떼어진 상태를 의미하는 Ended 페이즈 상태인지 체크할 수 있고,
            //  동시에 만약 touchOrigin.x 가 0이상 이라면 터치가 화면 내부에서 이루어졌다는 얘기
            //  선언할 때 초기화 했던 값에서 값이 바뀌었으며 터치가 끝났다는 것을 의미
            else if(myTouch.phase == TouchPhase.Ended && touchOrigin.x >= 0)
            {
                //  터치의 시작점과 종료점의 x좌표 상 차이를 계산
                Vector2 touchEnd = myTouch.position;
                float x = touchEnd.x - touchOrigin.x;
                float y = touchEnd.y - touchOrigin.y;
                //  touchOrigin.x를 -1로 설정해서 계속해서 참값을 반복하지 않도록 한다.
                touchOrigin.x = -1;

                //  터치 컨트롤이란게 언제나 완벽한 직선 형태인건 아니기 때문에
                //  유저의 터치의 의도를 '예측'해야한다.
                //  따라서 주어진 방향에서 터치가 좀더 가로 쪽인지, 좀더 세로 쪽인지 알아내야함
                //  이를 Math.Abs 를 사용해 x가 y보다 큰지 체크해서 알아낸다.
                if (Mathf.Abs(x) > Mathf.Abs(y))
                    //  x가 0보다 큰지 체크해서 양수인지 음수인지 판별하고 유저가 스와이프 하려는 방향을 알아낸다.
                    horizontal = x > 0 ? 1 : -1;
                else
                    vertical = y > 0 ? 1 : -1;
            }
        }

#endif

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
        foodText.text = "Food: " + food;
        base.AttemptMove<T>(xDir, yDir);

        RaycastHit2D hit;
        if(Move(xDir,yDir, out hit))
        {
            SoundManager.instance.RandomizeSfx(moveSound1, moveSound2);
        }

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
            foodText.text = "+" + pointsPerFood + " Food: " + food;
            SoundManager.instance.RandomizeSfx(eatSound1, eatSound2);
            other.gameObject.SetActive(false);
        }
        else if(other.tag == "Soda")
        {
            food += pointsPerSoda;
            foodText.text = "+" + pointsPerFood + " Food: " + food;
            SoundManager.instance.RandomizeSfx(drinkSound1, drinkSound2);
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
        foodText.text = "-" + loss + " Food: " + food;
        CheckIfGameOver();
    }

    private void CheckIfGameOver()
    {
        if(food <= 0)
        {            
            SoundManager.instance.PlaySingle(gameOverSound1);
            SoundManager.instance.musicSource.Stop();
            GameManager.instance.GameOver();
        }
    }    
}
