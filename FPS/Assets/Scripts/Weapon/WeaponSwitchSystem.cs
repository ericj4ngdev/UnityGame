using UnityEngine;

public class WeaponSwitchSystem : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerHUD playerHUD;

    [SerializeField] private WeaponBase[] weapons;
    
    private WeaponBase currentWeapon;
    private WeaponBase previousWeapon;

    private void Awake()
    {
        playerHUD.SetupAllWeapons(weapons);

        for (int i = 0; i < weapons.Length; ++i)
        {
            if (weapons[i].gameObject != null)
            {
                weapons[i].gameObject.SetActive(false);
            }
        }
        SwitchingWeapon(WeaponType.Main);
    }

    private void Update()
    {
        UpdateSwitch();
    }

    private void UpdateSwitch()
    {
        if (!Input.anyKeyDown) return;
        int inputIndex = 1;
        if (int.TryParse(Input.inputString, out inputIndex) && (inputIndex > 0 && inputIndex < 5))
        {
            SwitchingWeapon((WeaponType)(inputIndex - 1));
        }
    }

    private void SwitchingWeapon(WeaponType weaponType)
    {
        // 교체가능한 무기가 없으면 종료
        if (weapons[(int)weaponType] == null)
        {
            return;
        }
        // 현재 사용중인 무기가 있으면 이전 무기 정보에 저장
        if (currentWeapon != null)
        {
            previousWeapon = currentWeapon;
        }
        // 무기 교체
        currentWeapon = weapons[(int)weaponType];
        // 현재 사용중인 무기로 교체하려고 할때 종료
        if(currentWeapon == previousWeapon)
        {
            return;
        }
        // 무기를 사용하는 PlayerController, PlayerHUD에 현재 무기 정보 전달
        playerController.SwitchingWeapon(currentWeapon);
        playerHUD.SwitchingWeapon(currentWeapon);

        // 이전에 사용하던 무기 비활성화
        if (previousWeapon != null)
        {
            previousWeapon.gameObject.SetActive(false);
        }
        // 현재 사용하는 무기 활성화
        currentWeapon.gameObject.SetActive(true);
    }
}