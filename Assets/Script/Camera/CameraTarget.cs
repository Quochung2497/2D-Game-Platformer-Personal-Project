using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTarget : MonoBehaviour
{
    private Camera cam;
    private Transform player;
    private Rigidbody2D playerRb;
    [SerializeField] private float boundX = 5f;
    [SerializeField] private float boundY = 3f;
    [SerializeField] private float mouseSpeed = 2f;
    [SerializeField] private float movementThreshold = 0.1f;
    [SerializeField] private float canLook = 1f;
    private float timeCanLook;

    private bool isPlayerMoving;

    void Start()
    {
        playerRb = PlayerController.Instance.GetComponent<Rigidbody2D>();
        player = PlayerController.Instance.transform;
        cam = Camera.main;
        FollowPlayer();
    }

    void Update()
    {
        if (cam == null) return;
        isPlayerMoving = playerRb.velocity.sqrMagnitude > movementThreshold * movementThreshold;
        timeCanLook += Time.deltaTime;
        if (!isPlayerMoving)
        {
            if(timeCanLook > canLook)
            {
                LookAtMouse();
            }
        }
        else
        {
            FollowPlayer();
        }
    }

    private void LookAtMouse()
    {
        Vector3 mousePos = Input.mousePosition;

        mousePos.z = Vector3.Distance(cam.transform.position, player.position);

        Vector3 worldMousePos = cam.ScreenToWorldPoint(mousePos);

        Vector3 targetPos = CalculateTargetPos(player.position, worldMousePos);

        //transform.position = targetPos;
        transform.position = Vector3.Lerp(transform.position, targetPos, mouseSpeed * Time.deltaTime);
    }

    private void FollowPlayer()
    {
        /*Vector3 targetPos = Vector3.Lerp(transform.position, new Vector3(player.position.x, player.position.y, transform.position.z), followSpeed * Time.deltaTime);

        if (Vector3.Distance(new Vector2(transform.position.x, transform.position.y), new Vector2(player.position.x, player.position.y)) <= snapThreshold)
        {
            targetPos = new Vector3(player.position.x, player.position.y, transform.position.z);
        }
        transform.position = targetPos;
        timeCanLook = 0;*/

        Vector3 targetPos = CalculateTargetPos(player.position, transform.position);
        //transform.position = Vector3.Lerp(transform.position, targetPos, followSpeed * Time.deltaTime);
        transform.position = targetPos;

        timeCanLook = 0;
    }
    private Vector3 CalculateTargetPos(Vector3 playerPos, Vector3 objPos)
    {
        Vector3 targetPos = (playerPos + objPos) / 2f;

        targetPos.x = Mathf.Clamp(targetPos.x, playerPos.x - boundX, playerPos.x + boundX);
        targetPos.y = Mathf.Clamp(targetPos.y, playerPos.y - boundY, playerPos.y + boundY);
        return new Vector3(targetPos.x, targetPos.y, transform.position.z);
    }
}

