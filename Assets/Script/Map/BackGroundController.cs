using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundController : MonoBehaviour
{
    [SerializeField] private Vector2 parallaxMultiplier;
    [SerializeField] private bool InfiniteScrolling;

    private Transform cameraTransform;
    private Vector3 lastCameraPosition;
    private float textureUnitSizeX;
    private float textureUnitSizeY;

    void Start()
    {
        cameraTransform = Camera.main.transform;
        lastCameraPosition = cameraTransform.position;
        Sprite sprite = GetComponent<SpriteRenderer>().sprite;
        Texture2D texture = sprite.texture;
        textureUnitSizeX = texture.width / sprite.pixelsPerUnit;
        textureUnitSizeY = texture.height / sprite.pixelsPerUnit;
    }

    void LateUpdate()
    {
        Move();
        Scrolling();
    }
    private void Move()
    {
        Vector3 Movement = cameraTransform.position - lastCameraPosition;
        transform.position += new Vector3(Movement.x * parallaxMultiplier.x, Movement.y * parallaxMultiplier.y);
        lastCameraPosition = cameraTransform.position;
    }
    private void Scrolling()
    {
        if(InfiniteScrolling)
        {
            if(Mathf.Abs(cameraTransform.position.x - transform.position.x) >= textureUnitSizeX)
            {
                float offsetPositionX = (cameraTransform.position.x - transform.position.x) % textureUnitSizeX;
                transform.position = new Vector3(cameraTransform.position.x + offsetPositionX, transform.position.y);
            }
            if (Mathf.Abs(cameraTransform.position.y - transform.position.y) >= textureUnitSizeY)
            {
                float offsetPositionY = (cameraTransform.position.y - transform.position.y) % textureUnitSizeY;
                transform.position = new Vector3(transform.position.x,cameraTransform.position.y + offsetPositionY);
            }
        }
    }
}
