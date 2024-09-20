using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class TheEndMapTranstition : MonoBehaviour
{
    [SerializeField] private string transitionTo;

    [SerializeField] private Transform startPoint;

    [SerializeField] private Vector2 exitDirection;

    [SerializeField] private float exitTime;


    public float duration = 2f; // Thời gian di chuyển
    public GameObject canvasUI;

    public Transform EndingPosition;

    public static TheEndMapTranstition Instance;
    public float amplitude = 0.25f;
    public float frequency = 1f;
    private Vector2 initialPosition;
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

    private void Update()
    {
        if(!FinalBoss.Instance.alive)
        {
            Debug.Log("Starting Ending coroutine");
            StartCoroutine(Ending());
        }
    }



    IEnumerator Ending()
    {
        yield return new WaitForSeconds(5);
        // Di chuyển orb đến vị trí chỉ định
       
        Vector3 startPosition = transform.position;
        Vector3 endPosition = EndingPosition.position;
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        Debug.Log("Object moved to position: " + transform.position);
        transform.position = endPosition;
        yield return new WaitForSeconds(3.5f);
        //float yOffset = Mathf.Sin(Time.time * frequency) * amplitude;
        //transform.position = new Vector2(initialPosition.x, initialPosition.y + yOffset);
    }

    private void Start()
    {
        if (GlobalController.instance.transitionedFromScene == transitionTo)
        {
            PlayerController.Instance.transform.position = startPoint.position;

            StartCoroutine(PlayerController.Instance.WalkIntoNewScene(exitDirection, exitTime));
        }
        StartCoroutine(UIManager.Instance.sceneFader.Fade(ScreenFader.FadeDirection.Out));
        initialPosition = EndingPosition.position;
    }
    private void OnTriggerStay2D(Collider2D _other)
    {
        if (_other.CompareTag("Player"))
        { 
            canvasUI.SetActive(true);
            if (Input.GetKeyDown(KeyCode.E))
            {


                StartCoroutine(EndingTransition());
                //StartCoroutine(DelayDisable());

            }
        }
    }

    IEnumerator EndingTransition()
    {
        PlayerController.Instance.TheEnd();
        yield return new WaitForSeconds(4.5f);
        CheckShadeData();
        GlobalController.instance.transitionedFromScene = SceneManager.GetActiveScene().name;

        PlayerController.Instance.pState.cutscenes = true;

        PlayerController.Instance.pState.Invincible = true;

        StartCoroutine(UIManager.Instance.sceneFader.FadeAndLoadScene(ScreenFader.FadeDirection.In, transitionTo));
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            canvasUI.SetActive(false);
        }
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
