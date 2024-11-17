using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DestroyCamera : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        isDestroy();
    }
    private void isDestroy()
    {
        int currentScene = SceneManager.GetActiveScene().buildIndex;
        int[] destroyScenes = { 1, 3, 8 };
        if (System.Array.Exists(destroyScenes, scene => scene == currentScene))
        {
            Destroy(gameObject);
        }
    }
}
