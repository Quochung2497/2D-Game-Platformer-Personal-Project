using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAfterImage : MonoBehaviour
{   /*
    public float afterImagesDelay;

    private float afterImagesSeconds;
    [SerializeField] private GameObject afterImage;
    public bool Generate = false;
    private void Start()
    {
        afterImagesSeconds = afterImagesDelay;
    }

    private void Update()
    {
        if (Generate)
        {
            if (afterImagesSeconds > 0)
            {
                afterImagesSeconds -= Time.deltaTime;
            }
            else
            {
                //generate afterimages
                GameObject currentImage = Instantiate(afterImage, transform.position, transform.rotation);
                Sprite currentSprite = GetComponent<SpriteRenderer>().sprite;
                currentImage.GetComponent<SpriteRenderer>().sprite = currentSprite; 
                afterImagesSeconds = afterImagesDelay;
            }
        }
    }*/
    public float afterImagesDelay;
    private float afterImagesSeconds;
    public bool Generate = false;

    private void Start()
    {
        SetDelay();
    }

    private void Update()
    {
        if (Generate)
        {
            if (afterImagesSeconds > 0)
            {
                afterImagesSeconds -= Time.deltaTime;
            }
            else
            {
                GameObject afterImage = PlayerAfterImagesPool.Instance.GetFromPool();

                afterImage.transform.position = transform.position;
                afterImage.transform.rotation = transform.rotation;

                Sprite currentSprite = GetComponent<SpriteRenderer>().sprite;
                afterImage.GetComponent<SpriteRenderer>().sprite = currentSprite;

                StartCoroutine(ReturnAfterImageToPool(afterImage));
                SetDelay();
            }
        }
    }

    private IEnumerator ReturnAfterImageToPool(GameObject afterImage)
    {
        yield return new WaitForSeconds(0.3f);
        PlayerAfterImagesPool.Instance.ReturnToPool(afterImage);
    }
    public float SetDelay()
    {
        return afterImagesSeconds = afterImagesDelay;
    }
}
