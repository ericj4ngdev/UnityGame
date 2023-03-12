using System;
using System.Collections;
using UnityEngine;
public class EnemyProjectile : MonoBehaviour
{
    private MovementTransform movement;
    private float projectileDistance = 30;
    private int damage = 5;                 // 발사체 공격력
    public void Setup(Vector3 position)
    {
        movement = GetComponent<MovementTransform>();

        StartCoroutine("OnMove", position);
    }

    private IEnumerator OnMove(Vector3 targetPosition)
    {
        Vector3 start = transform.position;

        movement.MoveTo((targetPosition - transform.position).normalized);
        
        while ( true )
        {
            if(Vector3.Distance(transform.position, start) >= projectileDistance)
            {
                Destroy(gameObject);
                
                yield break;
            }
            yield return null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player Hit");
            // 발사체가 플레이어와 충돌했을 때, 플레이어의 PlayerController스크립트의 TakeDamage를 호출한다. 
            other.GetComponent<PlayerController>().TakeDamage(damage);
            
            Destroy(gameObject);
        }
    }
    
    
    
}