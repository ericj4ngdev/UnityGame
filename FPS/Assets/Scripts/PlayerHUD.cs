using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHUD : MonoBehaviour
{
    [Header("Components")] [SerializeField]
    private WeaponAssaultRifle weapon;

    [Header("Weapon Base")]
    [SerializeField]
    private TextMeshProUGUI textWeaponName;
    [SerializeField]
    private Image imageWeaponIcon;
    [SerializeField]
    private Sprite[] spriteWeaponIcons;

    [Header("Ammo")] 
    [SerializeField]
    private TextMeshProUGUI textAmmo;

    [Header("Magazine")] 
    [SerializeField] 
    private GameObject magazineUIPrefab;  // 탈창 UI 프리펩
    [SerializeField] 
    private Transform magazineParent;      // 탄창 UI가 배치되는 패널

    private List<GameObject> magazineList;      // 탄창 UI리스트
    
    
    private void Awake()
    {
        SetupWeapon();      // 현재 무기 정보 갱신
        SetupMagazine();
        // 메소드가 등록되어 있느 이벤트클래스의 
        // Invoke() 메소드가 호출된 때 등록된 메소드가 실행된다.
        weapon.onAmmoEvent.AddListener(UpdateAmmoHUD);
        weapon.onMagazineEvent.AddListener(UpdateMagazineHUD);
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

    private void SetupMagazine()
    {
        magazineList = new List<GameObject>();
        for (int i = 0; i < weapon.MaxMagazine; i++)
        {
            GameObject clone = Instantiate(magazineUIPrefab);
            clone.transform.SetParent(magazineParent);
            clone.SetActive(false);
            
            magazineList.Add(clone);
        }
        // weapon에 등록되어 있는 현재 탄창 개수만큼 오브젝트 활성화
        for (int i = 0; i < weapon.CurrentMagazine; ++i)
        {
            magazineList[i].SetActive(true);
        }
    }

    private void UpdateMagazineHUD(int currentMagazine)
    {
        // 전부 비활성화하고 currentMagazine 개수만큼 활성화
        for (int i = 0; i < magazineList.Count; ++i)
        {
            magazineList[i].SetActive(false);
        }
        for (int i = 0; i < currentMagazine; i++)
        {
            magazineList[i].SetActive(true);
        }
    }
}
