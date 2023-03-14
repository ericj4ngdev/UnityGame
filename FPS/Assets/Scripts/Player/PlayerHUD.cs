using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHUD : MonoBehaviour
{
    private WeaponBase weapon;
    
    [Header("Components")]
    [SerializeField]
    private Status status;

    [Header("Weapon Base")]
    [SerializeField]
    private TextMeshProUGUI textWeaponName;
    [SerializeField]
    private Image imageWeaponIcon;
    [SerializeField]
    private Sprite[] spriteWeaponIcons;
    [SerializeField]
    private Vector2[] sizeWeaponIcons;


    [Header("Ammo")] 
    [SerializeField]
    private TextMeshProUGUI textAmmo;

    [Header("Magazine")] 
    [SerializeField] 
    private GameObject magazineUIPrefab;  // 탈창 UI 프리펩
    [SerializeField] 
    private Transform magazineParent;      // 탄창 UI가 배치되는 패널
    [SerializeField] 
    private int maxMagazineCount;          // 처음 생성하는 최대 탄창 수

    private List<GameObject> magazineList;      // 탄창 UI리스트

    [Header("HP & BloodScreen UI")] 
    [SerializeField] private TextMeshProUGUI textHP;         // 플레이어 체력 출력하는 Text
    [SerializeField] private Image imageBloodScreen;
    [SerializeField] private AnimationCurve curveBloodScreen;
    
    private void Awake()
    {
        status.onHPEvent.AddListener(UpdateHPHUD);
    }

    public void SetupAllWeapons(WeaponBase[] weapons)
    {
        SetupMagazine();

        // 사용 가능한 모든 무기의 이벤트 등록
        for (int i = 0; i < weapons.Length; ++i)
        {
            weapons[i].onAmmoEvent.AddListener(UpdateAmmoHUD);
            weapons[i].onMagazineEvent.AddListener(UpdateMagazineHUD);
        }
    }

    public void SwitchingWeapon(WeaponBase newWeapon)
    {
        weapon = newWeapon;
        SetupWeapon();
    }
    
    private void SetupWeapon()
    {
        textWeaponName.text = weapon.WeaponName.ToString();
        imageWeaponIcon.sprite = spriteWeaponIcons[(int)weapon.WeaponName];
        imageWeaponIcon.rectTransform.sizeDelta = sizeWeaponIcons[(int)weapon.WeaponName];
    }
    private void UpdateAmmoHUD(int currentAmmo, int maxAmmo)
    {
        textAmmo.text = $"<size=40>{currentAmmo}/</size>{maxAmmo}";
    }

    private void SetupMagazine()
    {
        magazineList = new List<GameObject>();
        for (int i = 0; i < maxMagazineCount; ++i)
        {
            GameObject clone = Instantiate(magazineUIPrefab);
            clone.transform.SetParent(magazineParent);
            clone.SetActive(false);
            
            magazineList.Add(clone);
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

    private void UpdateHPHUD(int previous, int current)
    {
        textHP.text = "HP" + current;
        // 체력이 증가했을 때는 화면에 빨간색 이미지를 출력하지 않도록 return
        if (previous <= current) return;
        
        if (previous - current > 0)
        {
            StopCoroutine("OnBloodScreen");
            StartCoroutine("OnBloodScreen");
        }
    }

    private IEnumerator OnBloodScreen()
    {
        float percent = 0;
        while (percent < 1)
        {
            percent += Time.deltaTime;

            Color color = imageBloodScreen.color;
            color.a = Mathf.Lerp(1, 0, curveBloodScreen.Evaluate(percent));
            imageBloodScreen.color = color;

            yield return null;
        }
    }
}
