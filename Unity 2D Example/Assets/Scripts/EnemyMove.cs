using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    Rigidbody2D rigid;
    Animator anim;
    SpriteRenderer spriteRenderer;
    CapsuleCollider2D coll;
    

    public int nextMove;        // 행동 지표를 결정할 변수 생성

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        coll = GetComponent<CapsuleCollider2D>();
        Think();

        Invoke("Think", 5);     // 5초마다 Think호출
        // 주어진 시간이 지난 뒤, 지정된 함수를 실행하는 함수.
    }
    // Start is called before the first frame update

    

    // Update is called once per frame
    void FixedUpdate()
    {
        rigid.velocity = new Vector2(nextMove, rigid.velocity.y);

        // 낭떠러지 인지하는 코드
        Vector2 frontVec = new Vector2(rigid.position.x + nextMove*0.3f, rigid.position.y); // 0.3은 ray와 캐릭터사이 거리로
        // 숫자가 작을수록 낭떠러지 인식하는 거리 폭이 좁아진다.
        
        Debug.DrawRay(frontVec, Vector3.down, new Color(0, 1, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 1, LayerMask.GetMask("Platform"));
        if (rayHit.collider == null)
        {
            /*Debug.Log("경고! 이 앞 낭떠러지다");*/            // 충돌한 물체 이름 출력
            Turn();
        }
    }
    // 재귀
    void Think()
    {
        // Set Next Active
        nextMove = Random.Range(-1, 2);

        // Sprite Animation
        anim.SetInteger("WalkSpeed", nextMove);

        // Flip Sprite
        if(nextMove !=0)
            spriteRenderer.flipX = nextMove == 1;       // 0일때?

        // Recursive 통상적으로 재귀는 마지막에 써준다.
        /*Invoke("Think", 5);     // 생각하는 시간도 랜덤?*/
        float nextThinkTime = Random.Range(2f, 5f);     // 생각하는 시간이 2~4초 사이 
        Invoke("Think", nextThinkTime);
    }

    void Turn()
    {
        nextMove *= -1;         // 방향전환, 그럼 Invoke의 5초는 어떻게 초기화?
        spriteRenderer.flipX = nextMove == 1;
        CancelInvoke();         // 현재 작동중인 모든 Invoke함수를 멈추는 함수.
        Invoke("Think", 5);
    }

    public void OnDamaged()
    {
        // sprite Alpha
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);
        // Sprite Flip Y = 뒤집힌다. 
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
