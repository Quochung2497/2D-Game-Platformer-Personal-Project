using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    [SerializeField] private string transitionTo;

    [SerializeField] private Transform startPoint; 

    [SerializeField] private Vector2 exitDirection; 

    [SerializeField] private float exitTime; 

   
    private void Start()
    {
        if (GlobalController.instance.transitionedFromScene == transitionTo)
        {
            PlayerController.Instance.transform.position = startPoint.position;

            StartCoroutine(PlayerController.Instance.WalkIntoNewScene(exitDirection, exitTime));
        }
        UIManager.Instance.uiController.CheckScene();
        StartCoroutine(UIManager.Instance.sceneFader.Fade(ScreenFader.FadeDirection.Out));
    }
    private void OnTriggerEnter2D(Collider2D _other)
    {
        if (_other.CompareTag("Player"))
        {
            MoveToNextScene();
        }
    }

    private void MoveToNextScene()
    {
        CheckShadeData();
        GlobalController.instance.transitionedFromScene = SceneManager.GetActiveScene().name;
        StartCoroutine(UIManager.Instance.sceneFader.FadeAndLoadScene(ScreenFader.FadeDirection.In, transitionTo));
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
