using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MovingObject
{
    //  �÷��̾ ���� �μ� �� �� ������Ʈ�� ����� ������
    public int wallDamage = 1;
    //  �����̳� �Ҵٸ� �������� �÷��̾� ���ھ ������ ����
    public int pointsPerFood = 10;
    public int pointsPerSoda = 20;
    public float restartLevelDelay = 1f;

    private Animator animator;
    //  ������ �ٲٸ鼭 ���ھ �ٽ� ���� �Ŵ����� �Է��� �ֱ� ����,
    //  �ش� ���� ������ �÷��̾� ���ھ ����
    private int food;

    //  MovingObject�� �ִ� Start�ʹ� �ٸ���
    //  Player�� Start�� �����ϱ� ���� protected override 
    protected override void Start()
    {
        animator = GetComponent<Animator>();

        //  �÷��̾�� �ش� ���� ���� ���� ������ ������ �� �ְ�,
        //  ������ �ٲ� �� ���ӸŴ����� �ٽ� ������ �� ����
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

        //  �����̳� �������� �����̴� ������ 1�̳� -1�� �����ؼ� ���
        int horizontal = 0;
        int vertical = 0;

        //  ���� �����ϱ� ����, ���� ���� �ڵ�� Ű���峪 ��Ʈ�ѷ��� �Է¿� ���
        //  ���߿� ������̳� ��ġ ��ũ�� �Է��� �޴� ������ ���� ����
        horizontal = (int)Input.GetAxis("Horizontal");
        vertical = (int)Input.GetAxis("Vertical");

        //  ���� horizontal�� 0�� �ƴ϶�� �������� �̵����̱� ������
        //  �밢������ �����̴� ���� �������� ���� ���� 0���� 
        if (horizontal != 0)
            vertical = 0;

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
        base.AttemptMove<T>(xDir, yDir);

        RaycastHit2D hit;

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
