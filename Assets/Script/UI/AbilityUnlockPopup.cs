using System;
using UnityEngine;
using UnityEngine.UI;

public sealed class AbilityUnlockPopup : MonoBehaviour
{
    [SerializeField] private Button continueButton;

    private Action onContinue;
    private PauseMenuUI pauseMenu;
    private float previousTimeScale;
    private bool wasPaused;
    private bool isOpen;

    public void Show(Action continueAction)
    {
        if (isOpen) return;

        pauseMenu = PauseMenuUI.Instance;
        previousTimeScale = Time.timeScale;
        wasPaused = pauseMenu.GameIsPaused;
        onContinue = continueAction;
        isOpen = true;

        pauseMenu.GameIsPaused = true;
        Time.timeScale = 0f;
        gameObject.SetActive(true);
    }

    private void OnEnable()
    {
        continueButton.onClick.AddListener(Continue);
    }

    private void OnDisable()
    {
        continueButton.onClick.RemoveListener(Continue);
        ReleasePause();
    }

    private void Continue()
    {
        if (!isOpen) return;

        Action continueAction = onContinue;
        ReleasePause();
        gameObject.SetActive(false);
        continueAction?.Invoke();
    }

    private void ReleasePause()
    {
        if (!isOpen) return;

        isOpen = false;
        onContinue = null;
        Time.timeScale = previousTimeScale;
        if (pauseMenu != null) pauseMenu.GameIsPaused = wasPaused;
    }
}
