using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Casing : MonoBehaviour
{
    [SerializeField] private float deactivateTime = 5.0f;
    [SerializeField] private float casingSpin = 1.0f;
    [SerializeField] private AudioClip[] audioClips;

    private Rigidbody rigidbody3D;
    private AudioSource audioSource;
    private MemoryPool memoryPool;

    public void Setup(MemoryPool pool, Vector3 direction)
    {
        rigidbody3D = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        memoryPool = pool;

        // 탄피 이동 속력과 회전 속력 설정
        rigidbody3D.velocity = new Vector3(direction.x, 1.0f, direction.z);
        rigidbody3D.angularVelocity = new Vector3(Random.Range(-casingSpin, casingSpin),
            Random.Range(-casingSpin, casingSpin),
            Random.Range(-casingSpin, casingSpin));
        
        // 탄피 자동 비활성화를 위한 코루틴 실행
        StartCoroutine("DeactivateAfterTime");

    }

    private void OnCollisionEnter(Collision collision)
    {
        int index = Random.Range(0, audioClips.Length);
        audioSource.clip = audioClips[index];
        audioSource.Play();
    }

    private IEnumerator DeactivateAfterTime()
    {
        yield return new WaitForSeconds(deactivateTime);
        memoryPool.DeactivatePoolItem(this.gameObject);
    }
}
