using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Trapdoor_Ctrl : MonoBehaviour
{
    Room_Ctrl Room_Onwer;

    void Start()
    {
        Room_Onwer = transform.parent?.parent?.parent?.gameObject.GetComponent<Room_Ctrl>();
        if (Room_Onwer) Room_Onwer.Trapdoor = this;

        this.gameObject.SetActive(false);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            Mgr_Game.Inst.Player.rb.velocity = Vector3.zero;
            Mgr_Loading.Inst.StartChangeScenes(ELoadingType.ToMenu);
        }
    }
}
