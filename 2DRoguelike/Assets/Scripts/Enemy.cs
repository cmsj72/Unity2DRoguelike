using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MovingObject
{   
    //  적이 플레이어를 공격할 때 뺄셈할 음식 포인트
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

    //  일반형 입력 T를 사용하기에 
    //  이 경우에는 적이 상호작용 할것으로 예상되는 플레이어를 입력으로 한다.
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

        //  x좌표가 대략 같은지 체크(= 우리의 적과 플레이어가 같은 열에 속한다는 의미)
        if(Mathf.Abs(target.position.x - transform.position.x) < float.Epsilon)
        {
            //  만약 같은 열에 있다면 target위치의 y좌표가 transform위치의 y좌표 보다 큰지 체크
            //  target의 y값이 더 크면 target을 향해 위로 이동 아니면 아래로 이동
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
