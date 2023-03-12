using System;
using UnityEngine;

public enum ImpactType {Normal = 0, Obstacle, Enemy, InteractionObject}
public class ImpactMemoryPool : MonoBehaviour
{
    [SerializeField] 
    private GameObject[] impactPrefab;      // 피격 이펙트 
    private MemoryPool[] memoryPool;        // 피격 이펙트 메모리풀

    private void Awake()
    {
        memoryPool = new MemoryPool[impactPrefab.Length];
        for ( int i = 0; i < impactPrefab.Length; ++i )
        {
            memoryPool[i] = new MemoryPool(impactPrefab[i]);
        }
    }

    public void SpawnImpact(RaycastHit hit)
    {
        // 부딪힌 오브젝트의 Tag정보에 따라 다르게 처리
        if (hit.transform.CompareTag("ImpactNormal"))
        {
            OnSpawnImpact(ImpactType.Normal, hit.point, Quaternion.LookRotation(hit.normal));
        }
        else if (hit.transform.CompareTag("ImpactObstacle"))
        {
            OnSpawnImpact(ImpactType.Obstacle, hit.point, Quaternion.LookRotation(hit.normal));
        }
        else if (hit.transform.CompareTag("ImpactEnemy"))
        {
            OnSpawnImpact(ImpactType.Enemy,hit.point,Quaternion.LookRotation(hit.normal));
        }
        else if (hit.transform.CompareTag("InteractionObject"))
        {
            // 상호작용 오브젝트의 종류가 많기 때문에 오브젝트 색상에 따라 색상만 바뀌도록 설정
            Color color = hit.transform.GetComponent<MeshRenderer>().material.color;
            OnSpawnImpact(ImpactType.InteractionObject, hit.point, Quaternion.LookRotation(hit.normal), color);
        }
    } 

    // 매개변수 색상 추가
    public void OnSpawnImpact(ImpactType type, Vector3 position, Quaternion rotation, Color color = new Color())
    {
        GameObject item = memoryPool[(int)type].ActivatePoolItem();
        item.transform.position = position;
        item.transform.rotation = rotation;
        item.GetComponent<Impact>().Setup(memoryPool[(int)type]);

        if (type == ImpactType.InteractionObject)
        {
            ParticleSystem.MainModule main = item.GetComponent<ParticleSystem>().main;
            main.startColor = color;        // 색상만 변경
        }
    }
}
