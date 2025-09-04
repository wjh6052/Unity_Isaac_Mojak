using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mgr_Camera : MonoBehaviour
{
    public CinemachineVirtualCamera VirtualCamera;

    public static Mgr_Camera Inst;

    private void Awake()
    {
        Inst = this;
    }

    public void MoveCamera(Transform target)
    {
        VirtualCamera.Follow = target;
    }

}
