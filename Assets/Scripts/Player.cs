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
        // Axis���� ������ ��ȯ�ϴ� �Լ�
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        wDown = Input.GetButton("Walk");        
        // ��ǲ�Ŵ����� ���� Walk Ű ����
        // ��Ŭ���̱� ������ down�� �ƴ� 'get'button
        // ���Ƿ� ������ �Է�Ű�̱⿡ get'button'

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
