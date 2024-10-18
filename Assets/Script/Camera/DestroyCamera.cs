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
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            Destroy(gameObject);
        }
        if (SceneManager.GetActiveScene().buildIndex == 6)
        {
            Destroy(gameObject);
        }
    }
}
