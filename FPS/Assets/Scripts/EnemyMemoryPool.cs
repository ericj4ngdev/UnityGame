using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyMemoryPool : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private GameObject enemySpawnPointPrefab;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private float enemySpawnTime = 1;
    [SerializeField] private float enmeySpawnLatency = 1;

    private MemoryPool spawnPointMemoryPool;
    private MemoryPool enemyMemoryPool;

    private int numberOfEnemiesSpawnedAtOnce = 1;                   // 동시에 생성되는 적의 숫자
    private Vector2Int mapSize = new Vector2Int(100, 100);      // 맵 크기

    private void Awake()
    {
        spawnPointMemoryPool = new MemoryPool(enemySpawnPointPrefab);
        enemyMemoryPool = new MemoryPool(enemyPrefab);

        StartCoroutine("SpawnTile");
    }

    private IEnumerator SpawnTile()
    {
        int currentNumber = 0;
        int maximumNumber = 50;

        while (true)
        {
            // 동시에 ~~숫자만큼 적이 생성되도록 반복문 사용
            for (int i = 0; i < numberOfEnemiesSpawnedAtOnce; ++i)
            {
                GameObject item = spawnPointMemoryPool.ActivatePoolItem();      // 기둥 오브젝트 생성 
                item.transform.position = new Vector3(Random.Range(-mapSize.x * 0.49f, mapSize.x * 0.49f), 1,
                    Random.Range(-mapSize.y * 0.49f, mapSize.y * 0.49f));
                StartCoroutine("SpawnEnemy", item);     // 적 생성
            }

            currentNumber++;
            if (currentNumber >= maximumNumber)
            {
                currentNumber = 0;
                numberOfEnemiesSpawnedAtOnce++;
            }

            yield return new WaitForSeconds(enemySpawnTime);
        }
    }
    
    private IEnumerator SpawnEnemy(GameObject point)
    {
        yield return new WaitForSeconds(enmeySpawnLatency);
            
        // 적 오브젝트를 생성하고 적의 위치를 point의 위치로 설정
        GameObject item = enemyMemoryPool.ActivatePoolItem();
        item.transform.position = point.transform.position;
        
        item.GetComponent<EnemyFSM>().Setup(target);
            
        // 타일 오브젝트를 비활성화
        spawnPointMemoryPool.DeactivatePoolItem(point);
    }
    
}
