using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIcontroller : MonoBehaviour
{
    public GameObject[] UIGameObject;
    
    public void CheckScene()
    {
        if(SceneManager.GetActiveScene().name == "TutorialBoard Map")
        {
            TurnOff();
        }
        else
        {
            TurnOn();
        }
    }

    private void TurnOff()
    {
        for(int i = 0; i < UIGameObject.Length; i++)
        {
            UIGameObject[i].gameObject.SetActive(false);
        }
    }
    // Update is called once per frame
    private void TurnOn()
    {
        for (int i = 0; i < UIGameObject.Length; i++)
        {
            UIGameObject[i].gameObject.SetActive(true);
        }
    }
}
