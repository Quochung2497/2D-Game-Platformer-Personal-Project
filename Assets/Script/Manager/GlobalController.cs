using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;


public class GlobalController : MonoBehaviour
{
    public int playerScore = 0;
    public string playerName;


    public string transitionedFromScene;
    public Vector2 platformingRespawnPoint;
    public Vector2 respawnPoint;
    [SerializeField] Vector2 defaultRespawnPoint;
    private Bench bench;

    //private Bench currentBench;
    public static GlobalController instance { get; private set; }

    public GameObject player;
    public GameObject shade;

    private int enemiesKilled = 0;
    public int requiredKills = 2;


    void Awake()
    {
        SaveData.Instance.Initialize();
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(gameObject);
        SaveScene();
        if (PlayerController.Instance != null)
        {
            if (PlayerController.Instance.halfMana)
            {
                SaveData.Instance.LoadShadeData();
                if (SaveData.Instance.sceneWithShade == SceneManager.GetActiveScene().name || SaveData.Instance.sceneWithShade == "")
                {
                    Instantiate(shade, SaveData.Instance.shadePos, SaveData.Instance.shadeRot);
                }
            }
        }
        bench = FindObjectOfType<Bench>();
        LoadPlayerScore();
    }

    private void Start()
    {
        respawnPoint = defaultRespawnPoint;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            SaveData.Instance.SavePlayerData();
        }
        SavePlayerScore();
    }

    public void EnemyKilled()
    {
        enemiesKilled++;
        if (enemiesKilled >= requiredKills)
        {
            StartCoroutine(OpenTeleportationPoint());
        }
    }
    public void SaveScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SaveData.Instance.sceneNames.Add(currentSceneName);
    }
    IEnumerator OpenTeleportationPoint()
    {
        yield return new WaitForSeconds(1);
        SecretSceneTranstition.Instance.Enable();
    }

    public void SavePlayerScore()
    {
        PlayerPrefs.SetInt("playerScore", playerScore);
        PlayerPrefs.Save();
    }

    public void LoadPlayerScore()
    {
        playerScore = PlayerPrefs.GetInt("playerScore", 0);
    }

    public void RespawnPlayer()
    {
        SaveData.Instance.LoadBench();
        Debug.Log("Loaded benchSceneName: " + SaveData.Instance.benchSceneName);
        Debug.Log("Loaded benchPos: " + SaveData.Instance.benchPos);

        if (!string.IsNullOrEmpty(SaveData.Instance.benchSceneName))
        {
            SceneManager.LoadScene(SaveData.Instance.benchSceneName);
        }

        // Assuming benchPos is set to some invalid position by default
        if (SaveData.Instance.benchPos != Vector2.zero) // Example of invalid position check
        {
            respawnPoint = SaveData.Instance.benchPos;
            Debug.Log("Respawn point updated to bench position: " + respawnPoint); // Check if respawnPoint is correctly updated
        }
        else
        {
            respawnPoint = defaultRespawnPoint;
            Debug.Log("Respawn point set to default position: " + respawnPoint); // Check if respawnPoint is set to default position
        }

        PlayerController.Instance.transform.position = respawnPoint;
        Debug.Log("Player respawned at: " + PlayerController.Instance.transform.position);

        StartCoroutine(UIManager.Instance.DeactivateDeathScreen());
        PlayerController.Instance.Respawned();

    }

    public void DecreasePlayerScoreByHalf()
    {
        if(playerScore < 0)
        {
            playerScore = 0;
        }
        playerScore = Mathf.RoundToInt(playerScore / 2f);
        SavePlayerScore();
    }
}
