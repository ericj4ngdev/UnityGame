using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class AmmoEvent : UnityEngine.Events.UnityEvent<int,int> { }
[System.Serializable]
public class MagazineEvent : UnityEngine.Events.UnityEvent<int> { }

public class WeaponAssaultRifle : MonoBehaviour
{
    [HideInInspector]
    public AmmoEvent onAmmoEvent = new AmmoEvent();

    [HideInInspector] 
    public MagazineEvent onMagazineEvent = new MagazineEvent();
    
    [Header("Fire Effects")] [SerializeField]
    private GameObject muzzleFlashEffect;           // 총구 이펙트

    [Header("Spawn Points")] 
    [SerializeField]
    private Transform casingSpawnPoint;             // 탄피 생성 위치
    [SerializeField] 
    private Transform bulletSpawnPoint;             // 총알 생성 위치
    
    [Header("Audio Clips")] 
    [SerializeField]
    private AudioClip audioClipTakeOutWeapon;       // 무기 장착 사운드
    [SerializeField]
    private AudioClip audioClipFire;                // 공격 사운드
    [SerializeField] 
    private AudioClip audioClipReload;              // 재장전 사운드
    
    [Header("Weapon Setting")] [SerializeField]
    private WeaponSetting weaponSetting;            // 무기 설정
    
    [Header("Aim UI")] 
    [SerializeField]
    private Image imageAim;
    
    private float lastAttackTime = 0;               // 마지막 발사시간 체크용
    private bool isReload = false;                  // 재장전 중인지 체크
    private bool isAttack = false;                  // 공격 여부 체크용
    private bool isModeChange = false;              // 모드 전환 여부 체크용
    private float defaultModeFOV = 60;              // 기본 모드에서의 카메라 FOV
    private float aimModeFOV = 30;                  // AIM 모드에서의 카메라 FOV
    
    private AudioSource audioSource;                // 사운드 재생 컴포넌트
    private PlayerAnimatorController animator;      // 애니메이션 재생 제어
    private CasingMemoryPool casingMemoryPool;      // 탄피 생성 후 활성/비활성 관리
    private ImpactMemoryPool impactMemoryPool;      // 공격 효과 생성 후 활성/비활성 관리
    private Camera mainCamera;                      // 광선 발사
    
    
    // 외부에서 필요한 정보를 열람하기 위해 정의한 Get Property's
    public WeaponName WeaponName => weaponSetting.weaponName;
    public int CurrentMagazine => weaponSetting.currentMagazine;
    public int MaxMagazine => weaponSetting.maxMagazine;
    
    
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponentInParent<PlayerAnimatorController>();
        casingMemoryPool = GetComponent<CasingMemoryPool>();
        impactMemoryPool = GetComponent<ImpactMemoryPool>();
        mainCamera = Camera.main;
        // 처음 탄창 수 최대로 설정
        weaponSetting.currentMagazine = weaponSetting.maxMagazine;
        // 처음 탄 수는 최대로 설정
        weaponSetting.currentAmmo = weaponSetting.maxAmmo;
    }

    private void OnEnable()
    {
        // 무기장착 사운드 재생
        PlaySound(audioClipTakeOutWeapon);
        // 총구 이펙트 오브젝트 비활성화
        muzzleFlashEffect.SetActive(false);
        // 무기의 탄창 정보를 갱신한다. 
        onMagazineEvent.Invoke(weaponSetting.currentMagazine);
        // 무기가 활성화될 때, 해당 무기의 탄 수 정보를 갱신한다. 
        onAmmoEvent.Invoke(weaponSetting.currentAmmo,weaponSetting.maxAmmo);

        ResetVariables();
    }

    public void StartWeaponAction(int type = 0)
    {
        // 재장전 중일 때는 무기 액션을 할 수 없다.
        if (isReload == true) return;
        
        // 모드 전환중이면 무기 액션을 할 수 없다. 
        if (isModeChange == true) return;
        
        // 실제 공격은 OnAttack메소드에 있으며 
        // OnAttackLoop는 OnAttack을 매프레임 실행
        // 마우스 좌클릭(공격 시전)
        if (type == 0)
        {
            // 연속 공격
            if (weaponSetting.isAutomaticAttack == true)
            {
                isAttack = true;
                StartCoroutine("OnAttackLoop");
            }
            // 단발 공격
            else
            {
                OnAttack();
            }
        }
        // 마우스 우클릭 (모드 전환)
        else
        {
            if (isAttack == true) return;

            StartCoroutine("OnModeChange");
        }
    }
    // 연속 공격 종료 코드
    public void StopWeaponAction(int type = 0)
    {
        if (type == 0)
        {
            isAttack = false;
            StopCoroutine("OnAttackLoop");
        }
    }

    public void StartReload()
    {
        if (isReload == true || weaponSetting.currentMagazine <=0 ) return;       // 현재 재장전 중이면 불가능(?) 그냥 재장전 체크 아닌가
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
            // animator.Play("Fire",-1,0); // 같은 애니메이션을 반복할 때, 애니메이션을 끊고 처음부터 다시 재생
            string animation = animator.AimModeIs == true ? "AimFire" : "Fire";
            animator.Play(animation, -1, 0);
            
            // 총구 이펙트 재생(default mode 일 때만 재생)
            if(animator.AimModeIs == false) StartCoroutine("OnMuzzleFlashEffect");
            // 공격 사운드 재생
            PlaySound(audioClipFire);
            // 탄피 생성
            casingMemoryPool.SpawnCasing(casingSpawnPoint.position, transform.right);
            
            // 광선을 발사해 원하는 위치 공격
            TwoStepRaycast();
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
        while (true)
        {
            if (audioSource.isPlaying == false && animator.CurrentAnimationIs("Movement"))
            {
                isReload = false;       // 재장전 끝

                // 현재 탄창 수를 1 감소시키고 
                weaponSetting.currentMagazine--;
                onMagazineEvent.Invoke(weaponSetting.currentMagazine);
                
                // 현재 탄수 최대로 설정. 탄수 정보를 Text UI에 업데이트
                weaponSetting.currentAmmo = weaponSetting.maxAmmo;
                onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo);
                yield break;
            }
            yield return null;
        }
    }
    private void TwoStepRaycast()
    {
        Ray ray;
        RaycastHit hit;
        Vector3 targetPoint = Vector3.zero;
        
        // 화면 중앙 좌표(aim기준으로 Raycast 연산) - 카메라가 여기서 필요 
        ray = mainCamera.ViewportPointToRay(Vector2.one * 0.5f);
        if (Physics.Raycast(ray, out hit, weaponSetting.attackDistance))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.origin + ray.direction * weaponSetting.attackDistance;
        }
        Debug.DrawRay(ray.origin, ray.direction*weaponSetting.attackDistance, Color.red);
        
        // 첫번째 Raycast연산으로 얻어진 targetPoint를 목표지점으로 설정하고
        // 총구를 시작지점으로 하여 Raycast연산
        Vector3 attackDirection = (targetPoint - bulletSpawnPoint.position).normalized;
        if (Physics.Raycast(bulletSpawnPoint.position, attackDirection, out hit, weaponSetting.attackDistance))
        {
            impactMemoryPool.SpawnImpact(hit);
        }
        Debug.DrawRay(bulletSpawnPoint.position, attackDirection * weaponSetting.attackDistance,Color.blue);
    }

    private IEnumerator OnModeChange()
    {
        float current = 0;
        float percent = 0;
        float time = 0.35f;

        // 참이면 거짓, 거짓이면 참으로 설정
        animator.AimModeIs = !animator.AimModeIs;
        imageAim.enabled = !imageAim.enabled;

        float start = mainCamera.fieldOfView;
        float end = animator.AimModeIs == true ? aimModeFOV : defaultModeFOV;

        isModeChange = true;

        while (percent < 1)
        {
            current += Time.deltaTime;
            percent = current / time;
            
            // mode에 따라 카메라 시야각을 변경
            mainCamera.fieldOfView = Mathf.Lerp(start, end, percent);
            
            yield return null;
        }
        isModeChange = false;
    }

    private void ResetVariables()
    {
        isReload = false;
        isAttack = false;
        isModeChange = false;
    }
    private void PlaySound(AudioClip clip)
    {
        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.Play();
    }
}
