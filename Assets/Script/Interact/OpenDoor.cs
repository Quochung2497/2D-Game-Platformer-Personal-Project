using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoor : MonoBehaviour
{
    public DoorController Door;

    public GameObject canvasUI;
    private void OnTriggerStay2D(Collider2D collision)
    {
        canvasUI.SetActive(true);
        if (collision.CompareTag("Player") && Input.GetKeyDown(KeyCode.E))
        {
            PlayerController.Instance.Interact();
            //Door.OpeningDoor();
            Door.OpenDoor();
            PlayerController.Instance.WalkIntoDoor();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        canvasUI.SetActive(false);
    }
}
