using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSCounter : MonoBehaviour
{
    private float fps;
    public TMPro.TextMeshProUGUI FPSCounterText;
    public TMPro.TextMeshProUGUI scoreText;
    void Start()
    {
        InvokeRepeating("FPS", 1, 1);
    }
    private void Update()
    {
        // Cập nhật điểm số hiển thị
        if (GlobalController.instance != null)
        {
            scoreText.text =  GlobalController.instance.playerScore.ToString();
        }
    }
    void FPS()
    {
        fps = (int)(1f / Time.unscaledDeltaTime);
        FPSCounterText.text = "FPS: " + fps.ToString();
    }
}
