using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    // 타입 열거
    // enum은 Type이라는 타입의 열거형 변수이다.
    // enum뒤에는 타입 이름을 지정해 줘야 한다.
    // {} 안에 데이터들을 열거
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
