using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MovingObject
{
    //  �÷��̾ ���� �μ� �� �� ������Ʈ�� ����� ������
    public int wallDamage = 1;
    //  �����̳� �Ҵٸ� �������� �÷��̾� ���ھ ������ ����
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

    //  ������ �ٲٸ鼭 ���ھ �ٽ� ���� �Ŵ����� �Է��� �ֱ� ����,
    //  �ش� ���� ������ �÷��̾� ���ھ ����
    private int food;

    //  ��ġ��ũ���� �÷��̾��� ��ġ�� ���۵Ǵ� ������ �����ϱ� ���� ���
    //  -Vector2.one ��ũ�� ���� ��ġ�� �ǹ� �� ��ġ �Է��� �ִ��� üũ�ϱ� ���� ����ϴ� ����
    //  �̸� ���� touchOrigin�� ���� �ٲ��� ���� ��ġ�� �̷������ �� ������ ���� ������ �ʱ�ȭ�صд�.
    private Vector2 touchOrigin = -Vector2.one;

    //  MovingObject�� �ִ� Start�ʹ� �ٸ���
    //  Player�� Start�� �����ϱ� ���� protected override 
    protected override void Start()
    {
        animator = GetComponent<Animator>();

        //  �÷��̾�� �ش� ���� ���� ���� ������ ������ �� �ְ�,
        //  ������ �ٲ� �� ���ӸŴ����� �ٽ� ������ �� ����
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

        //  �����̳� �������� �����̴� ������ 1�̳� -1�� �����ؼ� ���
        int horizontal = 0;
        int vertical = 0;
        
        //  �����Ϳ��� Ű����� ��� �׽�Ʈ �غ��� �ʹٸ� ���� Ÿ���� ���ĵ� ������� �ٲܼ��� �ְ�,
        //  UNITY_EDITOR�� �߰����ָ�ȴ�.
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBPLAYER

        //  ���� �����ϱ� ����, ���� ���� �ڵ�� Ű���峪 ��Ʈ�ѷ��� �Է¿� ���
        //  ���߿� ������̳� ��ġ ��ũ�� �Է��� �޴� ������ ���� ����
        horizontal = (int)(Input.GetAxisRaw("Horizontal"));
        vertical = (int)(Input.GetAxisRaw("Vertical"));

        //  ���� horizontal�� 0�� �ƴ϶�� �������� �̵����̱� ������
        //  �밢������ �����̴� ���� �������� ���� ���� 0���� 
        if (horizontal != 0)
            vertical = 0;

#else
        //  Input.touchCount�� 0���� ũ�� �Է� �ý����� �ϳ� �̻��� ��ġ�� �����ߴٴ� �ǹ�
        if (Input.touchCount > 0)
        {
            //  ù��° ��ġ ������ ��Ƴ���, ������ ��ġ���� ���� ����
            //  �� ������ �� ���⿡ ���� �� �հ����� �������θ� ������ ���̱� ����
            Touch myTouch = Input.touches[0];

            //  ��Ƴ� ��ġ�� ����� Ȯ���� '��ġ�� �� ����'(Began) �������� üũ
            if(myTouch.phase == TouchPhase.Began)
            {
                touchOrigin = myTouch.position;
            }
            //  touchOrigin�� ó���� -1�� �ʱ�ȭ �س��� ������
            //  �հ����� ��ũ������ ������ ���¸� �ǹ��ϴ� Ended ������ �������� üũ�� �� �ְ�,
            //  ���ÿ� ���� touchOrigin.x �� 0�̻� �̶�� ��ġ�� ȭ�� ���ο��� �̷�����ٴ� ���
            //  ������ �� �ʱ�ȭ �ߴ� ������ ���� �ٲ������ ��ġ�� �����ٴ� ���� �ǹ�
            else if(myTouch.phase == TouchPhase.Ended && touchOrigin.x >= 0)
            {
                //  ��ġ�� �������� �������� x��ǥ �� ���̸� ���
                Vector2 touchEnd = myTouch.position;
                float x = touchEnd.x - touchOrigin.x;
                float y = touchEnd.y - touchOrigin.y;
                //  touchOrigin.x�� -1�� �����ؼ� ����ؼ� ������ �ݺ����� �ʵ��� �Ѵ�.
                touchOrigin.x = -1;

                //  ��ġ ��Ʈ���̶��� ������ �Ϻ��� ���� �����ΰ� �ƴϱ� ������
                //  ������ ��ġ�� �ǵ��� '����'�ؾ��Ѵ�.
                //  ���� �־��� ���⿡�� ��ġ�� ���� ���� ������, ���� ���� ������ �˾Ƴ�����
                //  �̸� Math.Abs �� ����� x�� y���� ū�� üũ�ؼ� �˾Ƴ���.
                if (Mathf.Abs(x) > Mathf.Abs(y))
                    //  x�� 0���� ū�� üũ�ؼ� ������� �������� �Ǻ��ϰ� ������ �������� �Ϸ��� ������ �˾Ƴ���.
                    horizontal = x > 0 ? 1 : -1;
                else
                    vertical = y > 0 ? 1 : -1;
            }
        }

#endif

        //  ���� �ϳ��� 0�� �ƴϸ� �����̷� �Ѵٴ� ���̱� ������ AttemptMove ȣ��
        if (horizontal != 0 || vertical != 0)
            //  �Ϲ��� ���� Wall�� �Է�
            //  �÷��̾ ��ȣ�ۿ��� �� �ִ� ���� ��������� �𸥴ٴ� �ǹ�
            //  �Ϲ��� �Է� T ( <T> )�� �����ν� �츮�� �Լ��� ȣ���� �� ��ȣ�ۿ��� ������Ʈ�� Ư���� �� �ִ�.
            //  �� ��쿡�� �÷��̾� ��ũ��Ʈ���� �츮�� ���� ��ȣ�ۿ� �� ���� ������ �� �ְ�,
            //  �� ��ũ��Ʈ�� ��쿡�� �÷��̾�� ��ȣ�ۿ� �� ���� ������ Ư���� �� �ִ�.
            AttemptMove<Wall>(horizontal, vertical);


    }

    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        //  �÷��̾ ������ �� ���� ���� ������ 1�� �Ҵ´�
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
            //  �ⱸ Ʈ���ſ� �浹 �� 1�� �Ŀ� �� �Լ��� ȣ��
            //  �� 1�ʰ� �����ϰ� ������ �ٽ� �������� �ǹ�
            //  Invoke �Լ�
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

    //  ������ �ٽ� �ε�(�÷��̾ �ⱸ ������Ʈ�� �浹���� �� Restart�� ȣ��, ���� ������ �Ѿ�°� �ǹ�)
    private void Restart()
    {   
        //Application.LoadLevel �� Application.loadedLevel �� ���� ������ �ʴ´�.
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
