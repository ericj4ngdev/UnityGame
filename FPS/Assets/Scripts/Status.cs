using UnityEngine;

public class Status : MonoBehaviour
{
    [Header("Walk, Run Speed")] [SerializeField]
    private float walkSpeed;
    [SerializeField]
    private float runSpeed;

    // 외부에서 값을 확인하는 용도로 get프로퍼티 정의
    public float WalkSpeed => walkSpeed;
    public float RunSpeed => runSpeed;
}
