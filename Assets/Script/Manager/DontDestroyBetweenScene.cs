using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyBetweenScene : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
}
