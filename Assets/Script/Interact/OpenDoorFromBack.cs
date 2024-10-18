using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoorFromBack : MonoBehaviour
{
    public DoorController Door;

    public static OpenDoorFromBack Instance;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        Disable();
    }
    public void Disable()
    {
        GetComponent<BoxCollider2D>().enabled = false;
    }
    public void Enable()
    {
        GetComponent<BoxCollider2D>().enabled = true;
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Door.OpenDoor();
        }
    }
}
