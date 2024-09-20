using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.ParticleSystem;


public class UnlockCastSpell : MonoBehaviour
{
    [SerializeField] GameObject Effect, UISideSpell, UIUpSpell, UIDownSpell;
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
            if (PlayerController.Instance.unlockedCastSpell)
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
        UISideSpell.SetActive(true);
        Time.timeScale = 0;
        PauseMenuUI.Instance.GameIsPaused = true;
        yield return new WaitForSecondsRealtime(3f);
        PauseMenuUI.Instance.GameIsPaused = false;
        Time.timeScale = 1;
        UISideSpell.SetActive(false);
        UIUpSpell.SetActive(true);
        Time.timeScale = 0;
        PauseMenuUI.Instance.GameIsPaused = true;
        yield return new WaitForSecondsRealtime(3f);
        PauseMenuUI.Instance.GameIsPaused = false;
        Time.timeScale = 1;
        UIUpSpell.SetActive(false);
        UIDownSpell.SetActive(true);
        Time.timeScale = 0;
        PauseMenuUI.Instance.GameIsPaused = true;
        yield return new WaitForSecondsRealtime(3f);
        PauseMenuUI.Instance.GameIsPaused = false;
        Time.timeScale = 1;
        UIDownSpell.SetActive(false);
        PlayerController.Instance.unlockedCastSpell = true;
        SaveData.Instance.SavePlayerData();
        Destroy(gameObject);
    }
}
