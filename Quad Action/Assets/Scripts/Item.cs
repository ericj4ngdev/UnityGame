using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    // Ÿ�� ����
    // enum�� Type�̶�� Ÿ���� ������ �����̴�.
    // enum�ڿ��� Ÿ�� �̸��� ������ ��� �Ѵ�.
    // {} �ȿ� �����͵��� ����
    public enum Type { Ammo, Coin, Grenade, Heart, Weapon};
    public Type type;
    public int value;

    Transform tr;
    float rotateSpeed = 20f;

    private void Awake()
    {
        tr = GetComponent<Transform>();
    }
    private void Update()
    {
        tr.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);
    }
}
