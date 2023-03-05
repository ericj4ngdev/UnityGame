using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AmmoEvent : UnityEngine.Events.UnityEvent<int,int> { }
public class WeaponAssaultRifle : MonoBehaviour
{
    [HideInInspector]
    public AmmoEvent onAmmoEvent = new AmmoEvent();
    
    [Header("Fire Effects")] [SerializeField]
    private GameObject muzzleFlashEffect;           // 총구 이펙트

    [Header("Spawn Points")] [SerializeField]
    private Transform casingSpawnPoint;             // 탄피 생성 위치
    
    [Header("Audio Clips")] [SerializeField]
    private AudioClip audioClipTakeOutWeapon;       // 무기 장착 사운드
    [SerializeField]
    private AudioClip audioClipFire;
    [SerializeField] 
    private AudioClip audioClipReload;              // 재장전 사운드
    
    [Header("Weapon Setting")] [SerializeField]
    private WeaponSetting weaponSetting;            // 무기 설정
    private float lastAttackTime = 0;               // 마지막 발사시간 체크용
    [SerializeField] private bool isReload = false;                  // 재장전 중인지 체크
    
    private AudioSource audioSource;                // 사운드 재생 컴포넌트
    private PlayerAnimatorController animator;      // 애니메이션 재생 제어
    private CasingMemoryPool casingMemoryPool;      // 탄피 생성 후 활성/비활성 관리

    // 외부에서 필요한 정보를 열람하기 위해 정의한 Get Property's
    public WeaponName WeaponName => weaponSetting.weaponName;
    
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponentInParent<PlayerAnimatorController>();
        casingMemoryPool = GetComponent<CasingMemoryPool>();
        
        // 처음 탄 수는 최대로 설정
        weaponSetting.currentAmmo = weaponSetting.maxAmmo;
    }

    private void OnEnable()
    {
        // 무기장착 사운드 재생
        PlaySound(audioClipTakeOutWeapon);
        // 총구 이펙트 오브젝트 비활성화
        muzzleFlashEffect.SetActive(false);
        // 무기가 활성화될 때, 해당 무기의 탄 수 정보를 갱신한다. 
        onAmmoEvent.Invoke(weaponSetting.currentAmmo,weaponSetting.maxAmmo);
    }

    public void StartWeaponAction(int type = 0)
    {
        // 재장전 중일 때는 무기 액션을 할 수 없다.
        if (isReload == true) return;
        
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

    public void StartReload()
    {
        if (isReload == true) return;       // 현재 재장전 중이면 불가능(?) 그냥 재장전 체크 아닌가
        StopWeaponAction();                 // 무기 액션 도중에 R키를 눌러 재장전을 시도하면 무기액션 종료 후 재장전
        StartCoroutine("OnReload");
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
            // 탄수가 없으면 공격 불가능
            if (weaponSetting.currentAmmo <= 0)
            {
                return;
            }
            // 공격시 currentAmmo 1 감소, UI 업데이트
            weaponSetting.currentAmmo--;
            onAmmoEvent.Invoke(weaponSetting.currentAmmo,weaponSetting.maxAmmo);
            
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
    
    private IEnumerator OnReload()
    {
        isReload = true;
        // 재장전 애니메이션, 사운드 재생
        animator.OnReload();
        PlaySound(audioClipReload);
        // ==============
        // 재장전 애니메이션과 사운드 재생 중 일때, .while반복문 실행
        while (true)
        {
            Debug.Log("R키 누르고 while문 실행");
            // 종료 여부 체크, 둘다 종료되면 
            if (audioSource.isPlaying == false && animator.CurrentAnimationIs("Movement"))
            {
                Debug.Log("R키 누르고 if문 실행");
                isReload = false;       // 재장전 끝
                // 현재 탄수 최대로 설정
                weaponSetting.currentAmmo = weaponSetting.maxAmmo;
                // 탄수 정보를 Text UI에 업데이트
                onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo);
                yield break;
            }
            yield return null;
        }
    }

    private void PlaySound(AudioClip clip)
    {
        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.Play();
    }
}
