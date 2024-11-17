using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class UnlockDash : MonoBehaviour
{
    [SerializeField] GameObject Effect, CanvasUI;
    [SerializeField] AudioClip Sound;
    bool used;
    public float amplitude = 0.25f;
    public float frequency = 1f;
    private Vector2 initialPosition;
    private AudioSource audioSource;
    private void Awake()
    {
        if (PlayerController.Instance != null)
        {
            if (PlayerController.Instance.unlockedDash)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            Debug.LogWarning("PlayerController.Instance is not initialized yet!");
        }
        initialPosition = transform.position;
    }
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    private void Update()
    {
        float yOffset = Mathf.Sin(Time.time * frequency) * amplitude;
        transform.position = new Vector2(initialPosition.x, initialPosition.y + yOffset);
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !used)
        {
            used = true;
            StartCoroutine(ShowUI());
        }
    }

    IEnumerator ShowUI()
    {
        audioSource.PlayOneShot(Sound);
        GameObject _particles = Instantiate(Effect, transform.position, Quaternion.identity);
        Destroy(_particles, 0.5f);
        yield return new WaitForSeconds(0.5f);
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
        CanvasUI.SetActive(true);
        Time.timeScale = 0;
        PauseMenuUI.Instance.GameIsPaused = true;
        yield return new WaitForSecondsRealtime(3f);
        PauseMenuUI.Instance.GameIsPaused = false;
        Time.timeScale = 1;
        PlayerController.Instance.unlockedDash = true;
        SaveData.Instance.SavePlayerData();
        CanvasUI.SetActive(false);
        Destroy(gameObject);
    }
}
