using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MovingObject
{   
    //  ���� �÷��̾ ������ �� ������ ���� ����Ʈ
    public int playerDamage;

    private Animator animator;
    private Transform target;
    private bool skipMove;

    protected override void Start()
    {
        animator = GetComponent<Animator>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        base.Start();
    }

    //  �Ϲ��� �Է� T�� ����ϱ⿡ 
    //  �� ��쿡�� ���� ��ȣ�ۿ� �Ұ����� ����Ǵ� �÷��̾ �Է����� �Ѵ�.
    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        if (skipMove)
        {
            skipMove = false;
            return;
        }

        base.AttemptMove<T>(xDir, yDir);

        skipMove = true;
    }

    public void MoveEnemy()
    {
        int xDir = 0;
        int yDir = 0;

        //  x��ǥ�� �뷫 ������ üũ(= �츮�� ���� �÷��̾ ���� ���� ���Ѵٴ� �ǹ�)
        if(Mathf.Abs(target.position.x - transform.position.x) < float.Epsilon)
        {
            //  ���� ���� ���� �ִٸ� target��ġ�� y��ǥ�� transform��ġ�� y��ǥ ���� ū�� üũ
            //  target�� y���� �� ũ�� target�� ���� ���� �̵� �ƴϸ� �Ʒ��� �̵�
            yDir = target.position.y > transform.position.y ? 1 : -1;
        }
        else
        {
            xDir = target.position.x > transform.position.x ? 1 : -1;
        }

        AttemptMove<Player>(xDir, yDir);
    }

    protected override void OnCantMove<T>(T component)
    {
        Player hitPlayer = component as Player;

        hitPlayer.LoseFood(playerDamage);
        //throw new System.NotImplementedException();
    }
}
