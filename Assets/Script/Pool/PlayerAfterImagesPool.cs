using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAfterImagesPool : MonoBehaviour
{
    [SerializeField] private GameObject afterImagePrefab;
    [SerializeField] private int poolSize = 10;

    private List<GameObject> availableAfterImages = new List<GameObject>();

    public static PlayerAfterImagesPool Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        GrowPool();
        DontDestroyOnLoad(gameObject);
    }

    private void GrowPool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject afterImage = Instantiate(afterImagePrefab);
            afterImage.transform.SetParent(transform);
            afterImage.SetActive(false);
            availableAfterImages.Add(afterImage);
        }
    }

    public GameObject GetFromPool()
    {
        if (availableAfterImages.Count == 0)
        {
            GrowPool();
        }

        GameObject afterImage = availableAfterImages[0];
        availableAfterImages.RemoveAt(0);
        afterImage.SetActive(true);
        return afterImage;
    }

    public void ReturnToPool(GameObject afterImage)
    {
        afterImage.SetActive(false);

        availableAfterImages.Add(afterImage);
    }
}
