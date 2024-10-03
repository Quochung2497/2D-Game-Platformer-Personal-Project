using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingLight : MonoBehaviour
{
    public float speed = 1.0f;
    public float range = 5.0f;

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        transform.position = startPosition + new Vector3(Mathf.PingPong(Time.time * speed, range), 0, 0);
    }
}
