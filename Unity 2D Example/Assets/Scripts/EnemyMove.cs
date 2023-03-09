using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    Rigidbody2D rigid;
    Animator anim;
    SpriteRenderer spriteRenderer;
    CapsuleCollider2D coll;
    

    public int nextMove;        // �ൿ ��ǥ�� ������ ���� ����

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        coll = GetComponent<CapsuleCollider2D>();
        Think();

        Invoke("Think", 5);     // 5�ʸ��� Thinkȣ��
        // �־��� �ð��� ���� ��, ������ �Լ��� �����ϴ� �Լ�.
    }
    // Start is called before the first frame update

    

    // Update is called once per frame
    void FixedUpdate()
    {
        rigid.velocity = new Vector2(nextMove, rigid.velocity.y);

        // �������� �����ϴ� �ڵ�
        Vector2 frontVec = new Vector2(rigid.position.x + nextMove*0.3f, rigid.position.y); // 0.3�� ray�� ĳ���ͻ��� �Ÿ���
        // ���ڰ� �������� �������� �ν��ϴ� �Ÿ� ���� ��������.
        
        Debug.DrawRay(frontVec, Vector3.down, new Color(0, 1, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 1, LayerMask.GetMask("Platform"));
        if (rayHit.collider == null)
        {
            /*Debug.Log("���! �� �� ����������");*/            // �浹�� ��ü �̸� ���
            Turn();
        }
    }
    // ���
    void Think()
    {
        // Set Next Active
        nextMove = Random.Range(-1, 2);

        // Sprite Animation
        anim.SetInteger("WalkSpeed", nextMove);

        // Flip Sprite
        if(nextMove !=0)
            spriteRenderer.flipX = nextMove == 1;       // 0�϶�?

        // Recursive ��������� ��ʹ� �������� ���ش�.
        /*Invoke("Think", 5);     // �����ϴ� �ð��� ����?*/
        float nextThinkTime = Random.Range(2f, 5f);     // �����ϴ� �ð��� 2~4�� ���� 
        Invoke("Think", nextThinkTime);
    }

    void Turn()
    {
        nextMove *= -1;         // ������ȯ, �׷� Invoke�� 5�ʴ� ��� �ʱ�ȭ?
        spriteRenderer.flipX = nextMove == 1;
        CancelInvoke();         // ���� �۵����� ��� Invoke�Լ��� ���ߴ� �Լ�.
        Invoke("Think", 5);
    }

    public void OnDamaged()
    {
        // sprite Alpha
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);
        // Sprite Flip Y = ��������. 
        spriteRenderer.flipY = true;
        // Colllider Disable
        coll.enabled = false;
        // Die Effect Jump
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
        // Destroy
        Invoke("DeActive", 5);
    }
    void DeActive()
    {
        gameObject.SetActive(false);

    }

}
