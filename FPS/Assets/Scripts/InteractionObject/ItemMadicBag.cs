using System.Collections;
using UnityEngine;

public class ItemMadicBag : ItemBase
{
    [SerializeField] private GameObject hpEffectPrefab;
    [SerializeField] private int increasHP = 50;
    [SerializeField] private float moveDistance = 0.2f;
    [SerializeField] private float pingpongSpeed = 0.5f;
    [SerializeField] private float rotateSpeed = 50;

    private IEnumerator Start()
    {
        float y = transform.position.y;

        while (true)
        {
            // y축을 기준으로 회전
            transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);
            
            // 처음 배치된 위치를 기준으로 y위치를 위, 아래로 이동
            Vector3 position = transform.position;
            position.y = Mathf.Lerp(y, y + moveDistance, Mathf.PingPong(Time.time * pingpongSpeed, 1));
            transform.position = position;

            yield return null;
        }
    }

    public override void Use(GameObject entity)
    {
        entity.GetComponent<Status>().IncreaseHP(increasHP);
        Instantiate(hpEffectPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
    
}
