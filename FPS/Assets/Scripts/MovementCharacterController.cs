using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// 이 명령이 포함된 스크립트를 게임 오브젝트에 컴포넌트로 추가하면 해당 컴포넌트도 자동으로 추가된다. 
[RequireComponent(typeof(CharacterController))]
public class MovementCharacterController : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    private Vector3 moveForce;

    [SerializeField] private float jumpForce;
    [SerializeField] private float gravity;

    public float MoveSpeed
    {
        set => moveSpeed = Mathf.Max(0, value); // 음수 방지
        get => moveSpeed;
    }
    
    private CharacterController characterController;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (!characterController.isGrounded)
        {
            moveForce.y += gravity * Time.deltaTime;
        }
        
        characterController.Move(moveForce * Time.deltaTime);
    }

    public void MoveTo(Vector3 direction)
    {
        direction = transform.rotation * new Vector3(direction.x, 0, direction.z);

        moveForce = new Vector3(direction.x * moveSpeed, moveForce.y, direction.z * moveSpeed);
        
    }

    public void Jump()
    {
        if (characterController.isGrounded)
        {
            moveForce.y = jumpForce;
        }
    }
    
    
}
