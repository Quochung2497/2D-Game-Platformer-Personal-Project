using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("----------Audio Source---------")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource vfxSource;
    [SerializeField] AudioSource foleySource;
    [SerializeField] AudioSource uiSource;

    [Header("----------Audio Clip---------")]
    public AudioClip[] theme;
    public AudioClip[] vfx;
    public AudioClip[] Enviroment;
    public AudioClip[] Enemy;
    public AudioClip Click;
    // Start is called before the first frame update
    void Start()
    {
        SelectTheme();
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
        //DontDestroyOnLoad(gameObject);
    }

    public void SelectTheme()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        if(currentScene == "Main Menu")
        {
            musicSource.clip = theme[0];
        }
        else if (currentScene == "Tutorial Map")
        {
            musicSource.clip = theme[1];
        }
        else if (currentScene == "Map 1")
        {
            musicSource.clip = theme[2];
        }
        else if (currentScene == "Map 2")
        {
            musicSource.clip = theme[3];
        }
        else if (currentScene == "Map 3")
        {
            musicSource.clip = null;
        }
        else if (currentScene == "Map 4")
        {
            musicSource.clip = theme[5];
        }
        else if (currentScene == "The End")
        {
            musicSource.clip = theme[6];
        }
        if (musicSource.clip != null)
        { musicSource.Play(); }
    }
    public void PlayVfx(AudioClip audioclip)
    {
        vfxSource.PlayOneShot(audioclip);
    }
    public void PlayFoley(AudioClip audioclip)
    {
        foleySource.PlayOneShot(audioclip);
    }
}
