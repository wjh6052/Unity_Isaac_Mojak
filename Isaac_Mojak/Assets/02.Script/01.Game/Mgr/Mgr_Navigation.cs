using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NavMeshPlus.Components;



public class Mgr_Navigation : MonoBehaviour
{
    public NavMeshSurface Surface;


    public static Mgr_Navigation Inst;

    private void Awake()
    {
        Inst = this;

        Surface = GetComponent<NavMeshSurface>();
    }


    private void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B)) // �׽�Ʈ��
        {
            NavMeshUpdate();
        }
    }

    public void NavMeshUpdate()
    {
        Surface.BuildNavMesh(); // NavMesh ����
    }
}
