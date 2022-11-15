using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;


public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;
    [SerializeField] List<CinemachineVirtualCamera> allCams;

    void Awake()
    {
        Instance = this;
    }

    public void SwitchTo(string camName)
    {
        foreach(var cam in allCams)
        {
            cam.Priority = cam.name == camName ? 100 : 10;
        }
    }
}
