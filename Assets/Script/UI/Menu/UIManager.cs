using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public ScreenFader sceneFader;

    public GameObject mapHandler;

    public VidPlayer videoplayer;

    public UIcontroller uiController;


    [SerializeField] GameObject deathScreen, BlackScreen;

    [SerializeField] GameObject halfMana, fullMana;

 

    public enum ManaState
    {
        FullMana,
        HalfMana
    }
    public ManaState manaState;

    

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        DontDestroyOnLoad(gameObject);

        sceneFader = GetComponentInChildren<ScreenFader>();
        
        uiController = GetComponentInChildren<UIcontroller>();
    }
    public void SwitchMana(ManaState _manaState)
    {
        switch (_manaState)
        {
            case ManaState.FullMana:

                halfMana.SetActive(false);
                fullMana.SetActive(true);

                break;

            case ManaState.HalfMana:

                fullMana.SetActive(false);
                halfMana.SetActive(true);

                break;
        }
        manaState = _manaState;
    }
    public IEnumerator ActivateDeathScreen()
    {
        yield return new WaitForSeconds(1f);
        StartCoroutine(sceneFader.Fade(ScreenFader.FadeDirection.In));
        yield return new WaitForSeconds(1f);
        BlackScreen.SetActive(true);
        deathScreen.SetActive(true);
    }
    public IEnumerator DeactivateDeathScreen()
    {
        yield return new WaitForSeconds(1f);
        BlackScreen.SetActive(false);
        deathScreen.SetActive(false);
        StartCoroutine(sceneFader.Fade(ScreenFader.FadeDirection.Out));
    }

}
