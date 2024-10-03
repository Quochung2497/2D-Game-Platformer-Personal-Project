using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VidPlayer : MonoBehaviour
{
    [SerializeField] private string VideoUrL = "https://quochung2497.github.io/TestVideo/Voidheart.mp4";
    private VideoPlayer videoplayer;

    private void Awake()
    {
        videoplayer = GetComponent<VideoPlayer>();
        if (videoplayer)
        {
            videoplayer.url = VideoUrL;
            //videoplayer.playOnAwake = false;
            videoplayer.Prepare();
            videoplayer.prepareCompleted += OnVideoPrepared;
        }
    }

    public void OnVideoPrepared(VideoPlayer source)
    {
        videoplayer.Play();
    }

    public void StopVideo()
    {
        videoplayer = GetComponent<VideoPlayer>();
        if (videoplayer && videoplayer.isPlaying)
        {
            videoplayer.Stop();
        }
    }
}
