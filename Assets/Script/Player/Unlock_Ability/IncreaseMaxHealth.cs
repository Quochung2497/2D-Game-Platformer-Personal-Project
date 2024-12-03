using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public class IncreaseMaxHealth : MonoBehaviour
{
    [SerializeField] GameObject particles;
    [SerializeField] GameObject heartFillUI;
    [SerializeField] AudioClip Sound;
    [SerializeField] HeartShards heartShards;

    bool used;
    public float amplitude = 1f; // Biên độ của dao động
    public float frequency = 1f; // Tần số của dao động
    private Vector2 initialPosition;
    private AudioSource audioSource;
    void Start()
    {
        if (PlayerController.Instance != null)
        {
            if (PlayerController.Instance.maxHealth >= PlayerController.Instance.maxTotalHealth)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            // Nếu PlayerController.Instance chưa được khởi tạo, in ra thông báo cảnh báo
            Debug.LogWarning("PlayerController.Instance is not initialized yet!");
        }
        initialPosition = transform.position;
        audioSource = GetComponent<AudioSource>();
    }
    private void Update()
    {
        float yOffset = Mathf.Sin(Time.time * frequency) * amplitude;
        transform.position = new Vector2(initialPosition.x, initialPosition.y + yOffset);
    }

    private void OnTriggerEnter2D(Collider2D _collision)
    {
        if (_collision.CompareTag("Player") && !used)
        {
            used = true;
            StartCoroutine(ShowUI());
        }
    }
    IEnumerator ShowUI()
    {
        audioSource.PlayOneShot(Sound);
        GameObject _particles = Instantiate(particles, transform.position, Quaternion.identity);
        Destroy(_particles, 0.5f);
        yield return new WaitForSeconds(0.5f);
        gameObject.GetComponent<SpriteRenderer>().enabled = false;

        heartFillUI.SetActive(true);
        heartShards.initialFillAmount = PlayerController.Instance.heartShards * 0.25f;
        PlayerController.Instance.heartShards++;
        heartShards.targetFillAmount = PlayerController.Instance.heartShards * 0.25f;

        StartCoroutine(heartShards.LerpFill());
        yield return new WaitForSecondsRealtime(3f);


        SaveData.Instance.SavePlayerData();
        heartFillUI.SetActive(false);
        Destroy(gameObject);
    }
}
