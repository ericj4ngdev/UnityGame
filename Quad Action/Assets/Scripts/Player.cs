using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;
    float hAxis;
    float vAxis;
    float jumpPower = 15f;    // 15
    bool wDown;
    bool jDown;

    bool isJump;
    bool isDodge;

    Vector3 moveVec;
    Vector3 dodgeVec;

    Rigidbody rigid;
    Animator anim;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();        
    }


    private void Update()
    {
        GetInput();
        Move();
        Turn();
        Jump();
        Dodge();
    }

    void GetInput()
    {
        // Axis값을 정수로 반환하는 함수
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        wDown = Input.GetButton("Walk");
        // 인풋매니저에 가서 Walk 키 설정
        // 토클형이기 때문에 down이 아닌 'get'button
        // 임의로 설정한 입력키이기에 get'button'
        jDown = Input.GetButtonDown("Jump");
    }
    void Move()
    {
        moveVec = new Vector3(hAxis, 0, vAxis).normalized;
        // 닷지중에는 이동 불가
        if (isDodge)
        {
            moveVec = dodgeVec;
        }
        transform.position += moveVec * speed * (wDown ? 0.3f : 1f) * Time.deltaTime;

        anim.SetBool("isRun", moveVec != Vector3.zero);
        anim.SetBool("isWalk", wDown);
    }
    void Turn()
    {
        transform.LookAt(transform.position + moveVec);
    }
    void Jump()
    {
        if (jDown && moveVec == Vector3.zero && !isJump && !isDodge)
        {
            rigid.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            anim.SetBool("isJump", true);
            anim.SetTrigger("doJump");
            isJump = true;
        }
    }

    // 움직임을 조건으로 추가해서 점프와 회피를 나누기
    void Dodge()
    {
        // 액션 도중 다른 액션이 실행되지 않도록 조건 추가
        if (jDown && moveVec != Vector3.zero && !isJump && !isDodge)
        {
            dodgeVec = moveVec;
            speed *= 2;
            anim.SetTrigger("doDodge");
            isDodge = true;

            // 시간차를 두어 느려지게
            Invoke("DodgeOut", 0.5f);   // 뒤 인자는 시간차
            

        }
    }

    void DodgeOut()
    {
        speed *= 0.5f;
        isDodge = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        // 착지하면 다시 false로 바뀌어야 한다. -> 충돌    
        if (collision.gameObject.tag == "Floor")
        {
            anim.SetBool("isJump", false);
            isJump = false;
        }
    }
}
