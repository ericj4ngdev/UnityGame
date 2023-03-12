using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ExplosionBarrel : InteractionObject
{
    [Header("Explosion Barrel")] 
    [SerializeField]
    private GameObject explosionPrefab;

    [SerializeField] private float explosionDelayTime = 0.3f;
    [SerializeField] private float explosionRadius = 10.0f;
    [SerializeField] private float explosionForce = 1000.0f;

    private bool isExplode = false;

    public override void TakeDamage(int damage)
    {
        currentHP -= damage;
        if (currentHP <= 0 && isExplode == false)
        {
            StartCoroutine("ExplodeBarrel");
        }
    }

    private IEnumerator ExplodeBarrel()
    {
        yield return new WaitForSeconds(explosionDelayTime);
        // 근처 배럴이 터져서 다시 현재 배럴을 터뜨리려고 할 때(stackoverflow 방지)
        isExplode = true;
        
        // 폭발 이펙트
        Bounds bounds = GetComponent<Collider>().bounds;
        Instantiate(explosionPrefab, new Vector3(bounds.center.x, bounds.min.y, bounds.center.z), transform.rotation);

        // 폭발 범위에 있는 모든 오브젝트의 Collider정보를 받아와 폭발효과 처리
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider hit in colliders)
        {
            // 오브젝트가 플레이어인 경우
            PlayerController player = hit.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(50);
                continue;
            }
            EnemyFSM enemy = hit.GetComponentInParent<EnemyFSM>();
            if (enemy != null)
            {
                enemy.TakeDamage(300);
                continue;
            }

            InteractionObject interaction = hit.GetComponent<InteractionObject>();
            if (interaction != null)
            {
                interaction.TakeDamage(300);
            }
            // 중력을 가진 오브젝트이면 밀려나감
            Rigidbody rigidbody = hit.GetComponent<Rigidbody>();
            if(rigidbody != null)
            {
                rigidbody.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }
        }
        // 배럴 오브젝트 삭제
        Destroy(gameObject);
    }
}
