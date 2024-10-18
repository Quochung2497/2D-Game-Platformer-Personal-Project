using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class SecretMapTransition : MonoBehaviour
{
    [SerializeField] private string transitionTo;

    [SerializeField] private Transform startPoint;

    [SerializeField] private Vector2 exitDirection;

    [SerializeField] private float exitTime;

    public GameObject canvasUI;

    public static SecretMapTransition Instance;

    private SpriteRenderer sr;
    private BoxCollider2D box;
    private Animator anim;

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
        //Disable();
    }
    private void OnTriggerStay2D(Collider2D _other)
    {
        if (_other.CompareTag("Player"))
        { 
            canvasUI.SetActive(true);
            if (Input.GetKeyDown(KeyCode.E))
            {
                CheckShadeData();
                PlayerController.Instance.Interact();
                GlobalController.instance.transitionedFromScene = SceneManager.GetActiveScene().name;
                StartCoroutine(UIManager.Instance.sceneFader.FadeAndLoadScene(ScreenFader.FadeDirection.In, transitionTo));

                StartCoroutine(DelayDisable());
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
    public void Disable()
    {
        sr.enabled = false;
        box.enabled = false;
        anim.enabled = false;
    }
    public void Enable()
    {
        sr.enabled = true;
        box.enabled = true;
        anim.enabled = true;
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
    private IEnumerator DelayDisable()
    {
        yield return new WaitForSeconds(10);
        Disable();

    }
}
