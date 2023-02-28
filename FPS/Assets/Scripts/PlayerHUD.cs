using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHUD : MonoBehaviour
{
    [Header("Components")] [SerializeField]
    private WeaponAssaultRifle weapon;

    [Header("Weapon Base")] [SerializeField]
    private TextMeshProUGUI textWeaponName;

    [SerializeField]private Image imageWeaponIcon;
    [SerializeField]private Sprite[] spriteWeaponIcons;

    [Header("Ammo")] [SerializeField] private TextMeshProUGUI textAmmo;

    private void Awake()
    {
        SetupWeapon();      // 현재 무기 정보 갱신
        // 메소드가 등록되어 있느 이벤트클래스의 
        // Invoke() 메소드가 호출된 때 등록된 메소드가 실행된다.
        weapon.onAmmoEvent.AddListener(UpdateAmmoHUD);
    }

    private void SetupWeapon()
    {
        textWeaponName.text = weapon.WeaponName.ToString();
        imageWeaponIcon.sprite = spriteWeaponIcons[(int)weapon.WeaponName];
    }

    private void UpdateAmmoHUD(int currentAmmo, int maxAmmo)
    {
        textAmmo.text = $"<size=40>{currentAmmo}/</size>{maxAmmo}";
    }    
    
}
