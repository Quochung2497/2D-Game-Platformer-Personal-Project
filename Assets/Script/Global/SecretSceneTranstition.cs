using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class SecretSceneTranstition : MonoBehaviour
{
    [SerializeField] private string transitionTo;

    [SerializeField] private Transform startPoint;

    [SerializeField] private Vector2 exitDirection;

    [SerializeField] private float exitTime;

    public GameObject canvasUI;

    public static SecretSceneTranstition Instance;

    private SpriteRenderer sr;
    private BoxCollider2D box;
    private Animator anim;
    private Light2D light;

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
    }

    private void Start()
    {
        if (GlobalController.instance.transitionedFromScene == transitionTo)
        {
            PlayerController.Instance.transform.position = startPoint.position;

            StartCoroutine(PlayerController.Instance.WalkIntoNewScene(exitDirection, exitTime));
        }
        StartCoroutine(UIManager.Instance.sceneFader.Fade(ScreenFader.FadeDirection.Out));
        sr = GetComponent<SpriteRenderer>();
        box = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
        light = GetComponent<Light2D>();
    }
    private void OnTriggerStay2D(Collider2D _other)
    {
        if (_other.CompareTag("Player"))
        {
            canvasUI.SetActive(true);
            if (_other.CompareTag("Player") && Input.GetKeyDown(KeyCode.E))
            {
                CheckShadeData();
                PlayerController.Instance.Interact();
                GlobalController.instance.transitionedFromScene = SceneManager.GetActiveScene().name;
                StartCoroutine(UIManager.Instance.sceneFader.FadeAndLoadScene(ScreenFader.FadeDirection.In, transitionTo));


                //StartCoroutine(DelayDisable());
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            canvasUI.SetActive(false);
        }
    }
    private IEnumerator DelayDisable()
    {
        yield return new WaitForSeconds(10);
        Disable();
    }
    public void Disable()
    {
        sr.enabled = false;
        box.enabled = false;
        anim.enabled = false;
        light.enabled = false;
    }
    public void Enable()
    {
        sr.enabled = true;
        box.enabled = true;
        anim.enabled = true;
        light.enabled = true;
    }
    void CheckShadeData()
    {
        GameObject[] enemyObjects = GameObject.FindGameObjectsWithTag("Enemy");

        for (int i = 0; i < enemyObjects.Length; i++)
        {
            if (enemyObjects[i].GetComponent<Shade>() != null)
            {
                SaveData.Instance.SaveShadeData();
            }
        }
    }
}
