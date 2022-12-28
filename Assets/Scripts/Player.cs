using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;
    float hAxis;
    float vAxis;
    bool wDown;

    Vector3 moveVec;

    Animator anim;
    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
    }


    private void Update()
    {
        // Axis값을 정수로 반환하는 함수
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        wDown = Input.GetButton("Walk");        
        // 인풋매니저에 가서 Walk 키 설정
        // 토클형이기 때문에 down이 아닌 'get'button
        // 임의로 설정한 입력키이기에 get'button'

        moveVec = new Vector3(hAxis, 0, vAxis).normalized;
        /*if(wDown)
            transform.position += moveVec * speed * 0.3f * Time.deltaTime;
        else
            transform.position += moveVec * speed * Time.deltaTime;*/

        transform.position += moveVec * speed * (wDown ? 0.3f : 1f) * Time.deltaTime;

        anim.SetBool("isRun", moveVec != Vector3.zero);
        anim.SetBool("isWalk", wDown);

        transform.LookAt(transform.position + moveVec);

    }
}
