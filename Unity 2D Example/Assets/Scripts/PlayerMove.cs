using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public GameManager gameManager;
    public float maxSpeed;
    public float jumpPower;
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Animator anim;
    BoxCollider2D boxCollider;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }


    // 단발적인 키 입력은 update에서 
    // Update is called once per frame
    void Update()
    {
        // Jump
        if (Input.GetButtonDown("Jump") && !anim.GetBool("isJumping"))  // 애니메이션이 isJumping이 아니면
        {
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            anim.SetBool("isJumping", true);
            /*Debug.Log("점프");*/
        }
        // Stop Speed, 정지
        if (Input.GetButtonUp("Horizontal")){       // 손에서 a,d 땔 때
            rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.5f, rigid.velocity.y);
            /*Debug.Log("정지하는 중");*/
        }
        // Direction Sprite, 방향 전환
        if (Input.GetButton("Horizontal")) {    // 손에서 a,d 누를 때
            spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;
            /*Debug.Log("바라보는 방향 전환");*/
        }
        // Animation
        if (Mathf.Abs(rigid.velocity.x) < 0.01){        // 멈추면
            anim.SetBool("isWalking", false);       // isWalking이 false.
            /*Debug.Log("정지");*/
        }
        // SetBool : 처음 정한 변수 자료형
        else
            anim.SetBool("isWalking", true);
    }
    void FixedUpdate()
    {
        // Move By Control, Move Speed
        float h = Input.GetAxisRaw("Horizontal");
        rigid.AddForce(Vector2.right * h * 3, ForceMode2D.Impulse);

        // Max Speed
        if (rigid.velocity.x > maxSpeed)
        {         // Right Max Speed
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
            /*Debug.Log("오른쪽 속도 상한선 도달");*/
        }
        else if (rigid.velocity.x < maxSpeed * (-1)){ // Left Max Speed
            rigid.velocity = new Vector2(maxSpeed * (-1), rigid.velocity.y);
            /*Debug.Log("왼쪽 속도 상한선 도달");*/
        }

        // Landing PLatform
        if (rigid.velocity.y < 0){  // 아래로 내려갈 때 
            Debug.DrawRay(rigid.position, Vector3.down, new Color(0, 1, 0));
            RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Platform"));
            // RaycastHit2D변수의 콜라이더로 검색 확인 가능.
            if (rayHit.collider != null)
            {
                if (rayHit.distance < 0.55f)
                    /*Debug.Log(rayHit.collider.name);*/ // 충돌한 물체 이름 출력
                    anim.SetBool("isJumping", false);
            }
        }
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {// 몬스터보다 위에 있음 + 아래로 낙하중 = 밟기 => Attack
            if(rigid.velocity.y < 0 && transform.position.y > collision.transform.position.y)
            {
                OnAttack(collision.transform);
            }
            else
                OnDamaged(collision.transform.position);
        }
            
    }

    private void OnTriggerEnter2D(Collider2D collision){
        if(collision.gameObject.tag == "Item"){
            // 동전별 점수
            bool isBronze = collision.gameObject.name.Contains("Bronze");
            bool isSilver = collision.gameObject.name.Contains("Silver");
            bool isGold = collision.gameObject.name.Contains("Gold");
            if (isBronze)
                gameManager.stagePoint += 50;
            else if (isSilver)
                gameManager.stagePoint += 100;
            else if (isGold)
                gameManager.stagePoint += 200;

            // 먹으면 사라진다. 아이템에 onTrigger 체크해줘야 한다.
            collision.gameObject.SetActive(false);
        }
        else if(collision.gameObject.tag == "Finish")        {
            // 다음 스테이지(매니저 역할)
            gameManager.NextStage();
        }
    }



    void OnAttack(Transform enemy){
        // 밟으면 점수 오름
        gameManager.stagePoint += 100;
        EnemyMove enemyMove = enemy.GetComponent<EnemyMove>();
        enemyMove.OnDamaged();
    }

    // 무적 효과 함수 생성
    void OnDamaged(Vector2 targetPos){

        gameManager.HealthDown();

        // 레이어를 Player -> PlayerDamaged 로 변경
        gameObject.layer = 11;      // 숫자로 써도 된다.
        // 유저에게 무적상태임을 알려준다.
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);
        // 넉백
        int dirc = transform.position.x - targetPos.x > 0 ? 1 : -1;
        rigid.AddForce(new Vector2(dirc, 1) * 7, ForceMode2D.Impulse);
        // 무적시간 결정
        Invoke("OffDamaged", 3);
    }

    void OffDamaged()
    {
        gameObject.layer = 10;
        spriteRenderer.color = new Color(1, 1, 1, 1);
    }

    public void OnDie()
    {
        // sprite Alpha
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);
        // Sprite Flip Y = 뒤집힌다. 
        spriteRenderer.flipY = true;
        // Colllider Disable
        boxCollider.enabled = false;
        // Die Effect Jump
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
    }




}
