using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponRevolver : WeaponBase
{
    [Header("Fire Effects")] [SerializeField]
    private GameObject muzzleFlashEffect;

    [Header("Spawn Points")] [SerializeField]
    private Transform bulletSpawnPoint;

    [Header("Audio Clips")] 
    [SerializeField]
    private AudioClip audioClipFire;
    [SerializeField]
    private AudioClip audioClipReload;

    private ImpactMemoryPool impactMemoryPool;
    private Camera mainCamera;
    
    private void OnEnable()
    {
        // 총구 이펙트 오브젝트 비활성화
        muzzleFlashEffect.SetActive(false);
        // 무기의 탄창 정보를 갱신한다. 
        onMagazineEvent.Invoke(weaponSetting.currentMagazine);
        // 무기가 활성화될 때, 해당 무기의 탄 수 정보를 갱신한다. 
        onAmmoEvent.Invoke(weaponSetting.currentAmmo,weaponSetting.maxAmmo);

        ResetVariables();
    }
    private void Awake()
    {
        // 기반 클래스의 초기화를 위한 Setup() 메소드 호출
        base.Setup();
        
        impactMemoryPool = GetComponent<ImpactMemoryPool>();
        mainCamera = Camera.main;
        // 처음 탄창 수 최대로 설정
        weaponSetting.currentMagazine = weaponSetting.maxMagazine;
        // 처음 탄 수는 최대로 설정
        weaponSetting.currentAmmo = weaponSetting.maxAmmo;
    }
    public override void StartWeaponAction(int type = 0)
    {
        if (type == 0 && isAttack == false && isReload == false)
        {
            OnAttack();
        }
    }

    public override void StopWeaponAction(int type = 0)
    {
        isAttack = false;
    }

    public override void StartReload()
    {
        if (isReload == true || weaponSetting.currentMagazine <=0 ) return;       // 현재 재장전 중이면 불가능(?) 그냥 재장전 체크 아닌가
        StopWeaponAction();                 // 무기 액션 도중에 R키를 눌러 재장전을 시도하면 무기액션 종료 후 재장전
        StartCoroutine("OnReload");
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
            animator.Play("Fire",-1,0); 
            
            // 총구 이펙트 재생(default mode 일 때만 재생)
            StartCoroutine("OnMuzzleFlashEffect");
            // 공격 사운드 재생
            PlaySound(audioClipFire);
            
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
            if (hit.transform.CompareTag("ImpactEnemy"))
            {
                hit.transform.GetComponent<EnemyFSM>().TakeDamage(weaponSetting.damage);
            }
            else if (hit.transform.CompareTag("InteractionObject"))
            {
                hit.transform.GetComponent<InteractionObject>().TakeDamage(weaponSetting.damage);
            }
        }
        Debug.DrawRay(bulletSpawnPoint.position, attackDirection * weaponSetting.attackDistance,Color.blue);
    }
    private void ResetVariables()
    {
        isReload = false;
        isAttack = false;
    }
}
