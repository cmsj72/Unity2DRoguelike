using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//  abstract : �߻� Ŭ����, ����� �ϼ����� �ʾƵ� �ǰ� �ϰ�, �ش� Ŭ������ �ݵ�� �Ļ�Ŭ������ ���ԵǾ���
public abstract class MovingObject : MonoBehaviour
{
    public float moveTime = 0.1f;
    //  �̵��� ������ �����ְ�, �� ������ �̵��Ϸ� �� ��, �浹�� �Ͼ���� üũ�� ���
    public LayerMask blockingLayer;

    private BoxCollider2D boxCollider;
    private Rigidbody2D rb2D;
    private float inverseMoveTime;

    //  �ڽ� Ŭ������ ����Ἥ ������ �� �� �ְ�(�������̵�)
    //  �ϳ� �̻��� �ڽ� Ŭ������ Start�� �ٸ��� ������ ���� �� �� ����
    protected virtual void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        rb2D = GetComponent<Rigidbody2D>();
        //  moveTime�� ������ ���������μ�, ������ ��ſ� ��꿡 ȿ������ ���ϱ⸦ ��밡��
        inverseMoveTime = 1f / moveTime;
    }

    //  out Ű����� �Է��� ���۷����� �ް���
    //  Move�Լ��� 2�� �̻��� ���� �����ϱ� ���� out ���
    protected bool Move(int xDir, int yDir, out RaycastHit2D hit)
    {
        Vector2 start = transform.position;
        Vector2 end = start + new Vector2(xDir, yDir);

        //  Ray�� ����� ��, �ڱ� �ڽ��� �浹ü�� �ε�ġ�� �ʰ� �ϱ� ���� boxCollider ����
        boxCollider.enabled = false;
        //  �������� ������ ������ ������ BlockingLayer���� �浹�� �˻�
        hit = Physics2D.Linecast(start, end, blockingLayer);
        //  �˻� �� �ٽ� boxCollider ���
        boxCollider.enabled = true;

        //  ���� hit.trnasform �� null�� ���ٸ�
        //  �������� �˻��� ������ ���� �ְ� �װ����� �̵��� �� ����
        if(hit.transform == null)
        {
            StartCoroutine(SmoothMovement(end));
            //  �̵��� �� �ִٴ� ������ true ����
            return true;
        }
        //  �̵��� �����ߴٴ� ������ false ����
        return false;
    }

    //  ���ֵ��� �� �������� �ٸ� ������ �ű�µ� ���
    //  ���� �̵��� ���� ǥ���� end�� �Է����� �޴´�.
    protected IEnumerator SmoothMovement(Vector3 end)
    {
        //  end�� ���� ��ġ�� ���� ���Ϳ� sqrMagnitude�� �Ÿ��� ���Ѵ�.
        //  Magnitude : ���� ����, sqrMagnitude : ���� ���� ������ ��Ʈ�� �� ��?
        float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

        //  Epsilon : 0�� ����� ��û ���� ��(like ����?)
        while(sqrRemainingDistance > float.Epsilon)
        {
            Vector3 newPosition = Vector3.MoveTowards(rb2D.position, end, inverseMoveTime * Time.deltaTime);
            rb2D.MovePosition(newPosition);
            //  �̵��Ŀ� �����Ÿ� �ٽ� ���
            sqrRemainingDistance = (transform.position - end).sqrMagnitude;
            //  ������ �����ϱ� ���� ���� �������� ��ٸ�
            yield return null;
        }
    }

    //  �Ϲ��� �Է� T�� ����
    //  �Ϲ��� �Է� T�� ������ ��, ������ ������ ������Ʈ Ÿ���� ����Ű�� ���� ���
    //  ���� ����� ��� ���� �÷��̾�, �÷��̾ ����� ��� ���� ������ �ȴ�.
    //  �׷��� �÷��̾ ���� �����ϰ� �ı��� �� �ִ�.
    //  where �̶�� Ű����� T�� ������Ʈ ������ ����Ű�� ��
    protected virtual void AttemptMove<T>(int xDir, int yDir)
        where T : Component
    {
        RaycastHit2D hit;
        bool canMove = Move(xDir, yDir, out hit);

        //  hit�� Move�� out �Է����� ���� ������ Move���� �ε��� transform�� null���� Ȯ���� �� �ִ�.
        if (hit.transform == null)
            //  ���𰡿� �ε����� ������ return;
            return;

        //  ���𰡿� �ε����ٸ� �浹�� ������Ʈ�� ������Ʈ�� ���۷����� TŸ���� ������Ʈ�� �Ҵ�
        T hitComponent = hit.transform.GetComponent<T>();

        //  ���� canMove�� false(�̵��� ����)�̰�, hitComponent�� null�� �ƴ϶��
        //  �����̴� ������Ʈ�� ������, ��ȣ�ۿ��� �� �ִ� ������Ʈ�� �浹������ �ǹ�
        if (!canMove && hitComponent != null)
            //  OnCantMove �Լ��� ȣ���ؼ� hitComponent�� �Է����� ����
            OnCantMove(hitComponent);
    }

    //  �Ϲ���(Generic) �Է� T�� T���� component��� �����μ� �޾ƿ�
    //  ���� �߻��� ����� �͵��� ���� �������� �ʰų�, �ҿ����ϰ� ����������� �ǹ�(����� �ڽ� Ŭ�������� �ϼ��ϸ� �ȴ�.)
    //  ����� �ڽ� Ŭ������ �Լ��� ����Ἥ �ϼ�(�������̵�)
    protected abstract void OnCantMove<T>(T component)
        where T : Component;
}
//  �Ϲ���(Generic)�� ����ϴ� ������ �÷��̾�� ���� MovingObject�� ����ϴµ�
//  �÷��̾�� ���� ��ȣ�ۿ��� �� �־�� �ϰ�, ���� �÷��̾�� ��ȣ�ۿ��� �� �־�� ��
//  �̴� ���߿� ��ȣ�ۿ��� hitComponent�� ������ �� �� ���ٴ� �����
//  �Ϲ����� ��������μ� ���� �̵��� ���۷����� OnCantMove�� ������ �Է��� �� �ְ�,
//  �̸� ����� Ŭ�������� ������ ������ ���� �����ϰ� �� �� �ִ�.
