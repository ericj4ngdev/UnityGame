using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Input KeyCodes")] [SerializeField]
    private KeyCode keyCodeRun = KeyCode.LeftShift;
    [SerializeField]
    private KeyCode keyCodeJump = KeyCode.Space;
    
    [Header("Audio Clips")] [SerializeField]
    private AudioClip audioClipWalk;
    [SerializeField] private AudioClip audioClipRun;
    
    
    // 클래스 객체 가져옴
    private RotateToMouse rotateToMouse;
    private MovementCharacterController movement;
    private Status status;      // 이동속도 등의 플레이어 정보
    private PlayerAnimatorController animator;
    private AudioSource audioSource;

    private void Awake()
    {
        // 마우스 커서 안보이게 설정
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        rotateToMouse = GetComponent<RotateToMouse>();
        movement = GetComponent<MovementCharacterController>();
        status = GetComponent<Status>();
        animator = GetComponent<PlayerAnimatorController>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        UpdateRotate(); 
        UpdateMove();
        UpdateJump();
    }

    private void UpdateRotate()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        
        rotateToMouse.UpdateRotate(mouseX,mouseY);
    }

    private void UpdateMove()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        // 이동중일떄
        if (x != 0 || z != 0)
        {
            bool isRun = false;
            // 옆이나 뒤로 이동시 달리기 제한
            if (z > 0) isRun = Input.GetKey(keyCodeRun);

            movement.MoveSpeed = isRun == true ? status.RunSpeed : status.WalkSpeed;
            animator.MoveSpeed = isRun == true ? 1 : 0.5f;
            audioSource.clip = isRun == true ? audioClipRun : audioClipWalk;

            if (audioSource.isPlaying == false)
            {
                audioSource.loop = true;
                audioSource.Play();
            }
        }
        // 제자리에 멈춰있을 때
        else
        {
            movement.MoveSpeed = 0;
            animator.MoveSpeed = 0;
            // 멈췄을때 사운드가 재생 중이면 정지
            if (audioSource.isPlaying == true)
            {
                audioSource.Stop();
            }
        }
        movement.MoveTo(new Vector3(x,0,z));
    }

    private void UpdateJump()
    {
        if (Input.GetKeyDown(keyCodeJump))
        {
            movement.Jump();
        }
    }
    
}
