using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleBarrel : InteractionObject
{
    [Header("Destructible Barrel")] [SerializeField]
    private GameObject destructibleBarrelPieces;

    private bool isDestroyed = false;

    public override void TakeDamage(int damage)
    {
        currentHP -= damage;

        if (currentHP <= 0 && isDestroyed == false)
        {
            isDestroyed = true;
            // 부서진 드럼통 생성
            Instantiate(destructibleBarrelPieces, transform.position, transform.rotation);
            Destroy(gameObject);        // 멀쩡한 드럼통 삭제
        }
    }
}
