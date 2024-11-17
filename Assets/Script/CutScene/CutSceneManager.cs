using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class CutSceneManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayableDirector timeline;
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private string SceneName;

    [Header("Video Clips")]
    [SerializeField] private VideoClip[] videoClips;

    private void Start()
    {
        StartCoroutine(CutsceneSequence());
    }

    private IEnumerator CutsceneSequence()
    {
        yield return new WaitForSeconds(3f);
        if (timeline != null)
        {
            timeline.Play();
            yield return new WaitForSeconds((float)timeline.duration);
        }

        if (videoClips != null)
        {
            yield return PlayVideosSequentially();
        }
        yield return new WaitForSeconds(0.5f);
        Debug.Log("Gameplay started!");
        SceneManager.LoadScene(SceneName, LoadSceneMode.Single);
    }

    private IEnumerator PlayVideosSequentially()
    {
        for (int i = 0; i < videoClips.Length; i++)
        {
            videoPlayer.clip = videoClips[i];
            videoPlayer.Prepare();
            yield return new WaitUntil(() => videoPlayer.isPrepared);
            videoPlayer.Play();

            Debug.Log($"Playing clip {i}: {videoClips[i].name}");
            yield return new WaitUntil(() => !videoPlayer.isPlaying);

            Debug.Log($"Finished playing clip {i}: {videoClips[i].name}");
        }

    }
}

