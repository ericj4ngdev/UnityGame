using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public GameObject[] Cameras;
    private int CamNo = 0;
    
    void FocusCamera(int No)
    {
        for(int i = 0; i < Cameras.Length; i++)
        {
            Cameras[i].SetActive(i == No);
        }
    }
    void Start()
    {
        FocusCamera(CamNo);
    }
    
    void ChangeCamera(int direct)
    {
        CamNo += direct;

        if(CamNo >= Cameras.Length)
        {
            CamNo = 0;
        }
        if (CamNo < 0)
        {
            CamNo = Cameras.Length - 1;
        }
        FocusCamera(CamNo);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))        // 좌클릭
        {
            ChangeCamera(1);        // CamNo가 1 증가
        }
    }
}
