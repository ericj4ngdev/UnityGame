using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAssaultRifle : MonoBehaviour
{
    [Header("Fire Effects")] [SerializeField]
    private GameObject muzzleFlashEffect;           // 총구 이펙트

    [Header("Spawn Points")] [SerializeField]
    private Transform casingSpawnPoint;             // 탄피 생성 위치
    
    [Header("Audio Clips")] [SerializeField]
    private AudioClip audioClipTakeOutWeapon;       // 무기 장착 사운드
    [SerializeField]
    private AudioClip audioClipFire;
    
    [Header("Weapon Setting")] [SerializeField]
    private WeaponSetting weaponSetting;            // 무기 설정
    private float lastAttackTime = 0;               // 마지막 발사시간 체크용
    
    private AudioSource audioSource;                // 사운드 재생 컴포넌트
    private PlayerAnimatorController animator;      // 애니메이션 재생 제어
    private CasingMemoryPool casingMemoryPool;      // 탄피 생성 후 활성/비활성 관리
    
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponentInParent<PlayerAnimatorController>();
        casingMemoryPool = GetComponent<CasingMemoryPool>();
    }

    private void OnEnable()
    {
        // 무기장착 사운드 재생
        PlaySound(audioClipTakeOutWeapon);
        // 총구 이펙트 오브젝트 비활성화
        muzzleFlashEffect.SetActive(false);
    }

    public void StartWeaponAction(int type = 0)
    {
        // 실제 공격은 OnAttack메소드에 있으며 
        // OnAttackLoop는 OnAttack을 매프레임 실행
        // 마우스 좌클릭(공격 시전)
        if (type == 0)
        {
            // 연속 공격
            if (weaponSetting.isAutomaticAttack == true)
                StartCoroutine("OnAttackLoop");
            // 단발 공격
            else
            {
                OnAttack();
            }

        }
    }

    // 연속 공격 종료 코드
    public void StopWeaponAction(int type = 0)
    {
        if(type ==0)
            StopCoroutine("OnAttackLoop");
    }

    private IEnumerator OnAttackLoop()
    {
        while (true)
        {
            OnAttack();
            yield return null;
        }
    }

    public void OnAttack()
    {
        if (Time.time - lastAttackTime > weaponSetting.attackRate)
        {
            // 달리는 동안에는 공격 X
            if (animator.MoveSpeed > 0.5f) return;
            // 공격 주기가 되어야 공격할 수 있도록 하기 위해 현재 시간 저장
            lastAttackTime = Time.time;
            // 무기 애니메이션 재생
            animator.Play("Fire",-1,0); // 같은 애니메이션을 반복할 때, 애니메이션을 끊고 처음부터 다시 재생
            // 총구 이펙트 재생
            StartCoroutine("OnMuzzleFlashEffect");
            // 공격 사운드 재생
            PlaySound(audioClipFire);
            // 탄피 생성
            casingMemoryPool.SpawnCasing(casingSpawnPoint.position, transform.right);
        }
    }

    private IEnumerator OnMuzzleFlashEffect()
    {
        muzzleFlashEffect.SetActive(true);
        yield return new WaitForSeconds(weaponSetting.attackRate * 0.3f);
        muzzleFlashEffect.SetActive(false);
    }
    private void PlaySound(AudioClip clip)
    {
        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.Play();
    }
}
