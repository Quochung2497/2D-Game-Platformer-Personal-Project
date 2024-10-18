using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    public Transform target;
    public Camera cam;
    private float distance = 5f;
    public float speed = 100f;

    void Update()
    {
        float scroll = Input.mouseScrollDelta.y;
        distance -= scroll * Time.deltaTime * speed;
        cam.transform.position = target.transform.position - (cam.transform.forward * distance);
    }
}
