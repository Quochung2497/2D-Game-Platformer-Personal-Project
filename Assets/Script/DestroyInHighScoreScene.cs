using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DestroyInHighScoreScene : MonoBehaviour
{
    void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex == 5)
        {
            Destroy(gameObject);
        }
    }
}
