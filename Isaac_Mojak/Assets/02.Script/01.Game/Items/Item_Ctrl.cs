using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;
using static UnityEngine.UI.Image;



public class Item_Ctrl : MonoBehaviour
{
    public EItemType ItemType;
    public SpriteRenderer ItemImage;

    bool IsItem = true;

    bool isPlayerPushing = false;
    public SpringJoint2D slider;

    public AudioClip ItemPickUp_Audio;

    Room_Ctrl Room_Onwer;

    void Awake()
    {
        IsItem = true;
        slider = GetComponent<SpringJoint2D>();

        ItemType = (EItemType)Random.Range(0, (int)EItemType.End);
        ItemImage.sprite = Resources.Load<Sprite>($"Item/Item/{ItemType.ToString()}");
    }

    void Start()
    {
        Room_Onwer = transform.parent?.parent?.parent?.gameObject.GetComponent<Room_Ctrl>();
        if (Room_Onwer)
        {
            Room_Onwer.Item = this;
            this.gameObject.SetActive(false);
        }


    }
    private void Update()
    {
        if (!IsItem) return;
        if (!isPlayerPushing) return;

        Vector2 force = slider.reactionForce;

        if (isPlayerPushing && force.magnitude > 200f)
        {
            IsItem = false;
            Mgr_Sound.Inst.PlaySound(this.gameObject, ItemPickUp_Audio);
            Mgr_Game.Inst.Player.AddItem(ItemType);
            ItemImage.gameObject.SetActive(false);
        }
    }


    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
            isPlayerPushing = true;
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
            isPlayerPushing = false;
    }

}
