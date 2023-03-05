using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorController : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    public float MoveSpeed
    {
        set => animator.SetFloat("movementSpeed", value);
        get => animator.GetFloat("movementSpeed");
    }

    // onReload를 On 시켜 재장전 애니메이션을 실행
    public void OnReload()
    {
        animator.SetTrigger("onReload");
    }
    
    public void Play(string stateName, int layer, float normalizedTime)
    {
        animator.Play(stateName,layer,normalizedTime);
    }

    // 매개변수로 받아온 애니메이션이 현재 재생중인지 확인 후 bool 반환  
    public bool CurrentAnimationIs(string name)
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName(name);
    }
    
}
