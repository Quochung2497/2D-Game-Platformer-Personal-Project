using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    private Animator anim;
    //public GameObject chargeEnemy, chargeEnemy1;
    private BoxCollider2D originalBoxCollider;
    private bool activeCoroutineStarted = false;
    private void Start()
    {
        anim = GetComponent<Animator>();
    }
    public void OpenDoor()
    {
        anim.SetBool("isOpening", true);
        BoxCollider2D box = GetComponent<BoxCollider2D>();
        if (box != null)
        {
            originalBoxCollider = box;
            box.enabled = false;
            //Destroy(box);
        }
    }
}
