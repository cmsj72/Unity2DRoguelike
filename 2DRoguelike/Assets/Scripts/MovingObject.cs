using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//  abstract : 추상 클래스, 기능을 완성하지 않아도 되게 하고, 해당 클래스는 반드시 파생클래스로 삽입되야함
public abstract class MovingObject : MonoBehaviour
{
    public float moveTime = 0.1f;
    //  이동할 공간이 열려있고, 그 곳으로 이동하려 할 때, 충돌이 일어났는지 체크할 장소
    public LayerMask blockingLayer;

    private BoxCollider2D boxCollider;
    private Rigidbody2D rb2D;
    private float inverseMoveTime;

    //  자식 클래스가 덮어써서 재정의 할 수 있게(오버라이드)
    //  하나 이상의 자식 클래스가 Start를 다르게 설정할 지도 모를 때 유용
    protected virtual void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        rb2D = GetComponent<Rigidbody2D>();
        //  moveTime의 역수를 저장함으로서, 나누기 대신에 계산에 효율적인 곱하기를 사용가능
        inverseMoveTime = 1f / moveTime;
    }

    //  out 키워드는 입력을 레퍼런스로 받게함
    //  Move함수가 2개 이상의 값을 리턴하기 위해 out 사용
    protected bool Move(int xDir, int yDir, out RaycastHit2D hit)
    {
        Vector2 start = transform.position;
        Vector2 end = start + new Vector2(xDir, yDir);

        //  Ray를 사용할 때, 자기 자신의 충돌체에 부딪치지 않게 하기 위해 boxCollider 해제
        boxCollider.enabled = false;
        //  시작점과 끝점의 라인을 가져와 BlockingLayer와의 충돌을 검사
        hit = Physics2D.Linecast(start, end, blockingLayer);
        //  검사 후 다시 boxCollider 사용
        boxCollider.enabled = true;

        //  만약 hit.trnasform 이 null과 같다면
        //  라인으로 검사한 공간이 열려 있고 그곳으로 이동할 수 있음
        if(hit.transform == null)
        {
            StartCoroutine(SmoothMovement(end));
            //  이동할 수 있다는 뜻으로 true 리턴
            return true;
        }
        //  이동을 실패했다는 뜻으로 false 리턴
        return false;
    }

    //  유닛들을 한 공간에서 다른 곳으로 옮기는데 사용
    //  어디로 이동할 건지 표시할 end를 입력으로 받는다.
    protected IEnumerator SmoothMovement(Vector3 end)
    {
        //  end와 현재 위치의 차이 벡터에 sqrMagnitude로 거리를 구한다.
        //  Magnitude : 벡터 길이, sqrMagnitude : 벡터 길이 제곱에 루트를 한 값?
        float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

        //  Epsilon : 0에 가까운 엄청 작은 수(like 극한?)
        while(sqrRemainingDistance > float.Epsilon)
        {
            Vector3 newPosition = Vector3.MoveTowards(rb2D.position, end, inverseMoveTime * Time.deltaTime);
            rb2D.MovePosition(newPosition);
            //  이동후에 남은거리 다시 계산
            sqrRemainingDistance = (transform.position - end).sqrMagnitude;
            //  루프를 갱신하기 전에 다음 프레임을 기다림
            yield return null;
        }
    }

    //  일반형 입력 T를 받음
    //  일반형 입력 T는 막혔을 때, 유닛이 반응할 컴포넌트 타입을 가리키기 위해 사용
    //  적에 적용된 경우 상대는 플레이어, 플레이어에 적용된 경우 상대는 벽들이 된다.
    //  그래서 플레이어가 벽을 공격하고 파괴할 수 있다.
    //  where 이라는 키워드로 T가 컴포넌트 종류를 가리키게 함
    protected virtual void AttemptMove<T>(int xDir, int yDir)
        where T : Component
    {
        RaycastHit2D hit;
        bool canMove = Move(xDir, yDir, out hit);

        //  hit이 Move에 out 입력으로 들어갔기 때문에 Move에서 부딪힌 transform이 null인지 확인할 수 있다.
        if (hit.transform == null)
            //  무언가와 부딪히지 않으면 return;
            return;

        //  무언가와 부딪혔다면 충돌한 오브젝트의 컴포넌트의 레퍼런스를 T타입의 컴포넌트에 할당
        T hitComponent = hit.transform.GetComponent<T>();

        //  만약 canMove가 false(이동을 실패)이고, hitComponent가 null이 아니라면
        //  움직이던 오브젝트가 막혔고, 상호작용할 수 있는 오브젝트와 충돌했음을 의미
        if (!canMove && hitComponent != null)
            //  OnCantMove 함수를 호출해서 hitComponent를 입력으로 넣음
            OnCantMove(hitComponent);
    }

    //  일반형(Generic) 입력 T를 T형의 component라는 변수로서 받아옴
    //  여기 추상문은 사용할 것들이 현재 존재하지 않거나, 불완전하게 만들어졌음을 의미(상속한 자식 클래스에서 완성하면 된다.)
    //  상속한 자식 클래스의 함수로 덮어써서 완성(오버라이드)
    protected abstract void OnCantMove<T>(T component)
        where T : Component;
}
//  일반형(Generic)을 사용하는 이유는 플레이어와 적이 MovingObject를 상속하는데
//  플레이어는 벽과 상호작용할 수 있어야 하고, 적은 플레이어와 상호작용할 수 있어야 함
//  이는 나중에 상호작용할 hitComponent의 종류를 알 수 없다는 얘기임
//  일반형을 사용함으로서 당장 이들의 레퍼런스를 OnCantMove로 가져와 입력할 수 있고,
//  이를 상속한 클래스들의 각각의 구현에 따라 동작하게 할 수 있다.
