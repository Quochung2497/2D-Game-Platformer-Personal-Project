using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class Bench : MonoBehaviour
{
    public GameObject canvanUI;
    bool inRange = false;
    public bool interacted;
    private Animator anim;
    private Light2D light;
    
    private void Start()
    {
        anim = GetComponent<Animator>();
        light = GetComponent<Light2D>();
    }
    void Update()
    {
        if (inRange)
        {
            interacted = true;
            light.enabled = true;
            anim.SetTrigger("isLit");
            SaveData.Instance.benchSceneName = SceneManager.GetActiveScene().name;
            SaveData.Instance.benchPos = new Vector2(transform.position.x, transform.position.y);
            SaveData.Instance.SaveBench();
            SaveData.Instance.SavePlayerData();
            GlobalController.instance.SavePlayerScore();
        }
    }

    void OnTriggerStay2D(Collider2D _collision)
    {
        canvanUI.SetActive(true);
        if (_collision.CompareTag("Player")) inRange = true;
    }

    void OnTriggerExit2D(Collider2D _collision)
    {
        canvanUI.SetActive(false);
        if (_collision.CompareTag("Player"))
        {
            inRange = false;
            interacted = false;
        }
    }
}
