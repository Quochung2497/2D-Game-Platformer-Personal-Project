using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] AudioMixer audioMixer;

    public void SetVolume(float _volume)
    {
        audioMixer.SetFloat("Volume", _volume);
    }
    public void SetQuality(int _qualityIndex)
    {
        Debug.Log("Quality Index: " + _qualityIndex);
        QualitySettings.SetQualityLevel(_qualityIndex);
    }
    public void SetFullScreen(bool _isFullScreen)
    {
        Screen.fullScreen = _isFullScreen;
    }
    public void Quit()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
    public void LoadGame()
    {
        SaveData.Instance.LoadPlayerData();
    }
}
