using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class CutSceneManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject storyManager;
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private PrologueFadeUI FadeUI;
    [SerializeField] private string SceneName;
    [SerializeField] private float delay;

    [Header("Video Clips")]
    [SerializeField] private VideoClip[] videoClips;

    private Coroutine cutsceneCoroutine;
    private void Start()
    {
        cutsceneCoroutine = StartCoroutine(CutsceneSequence());
    }

    /*private void Update()
    {
        if (Input.GetKey(KeyCode.Mouse0) || Input.GetKey(KeyCode.JoystickButton0))
        {
            skipTimer += Time.deltaTime;
            Debug.Log("timer = " + skipTimer);
            if (skipTimer >= skipThreshold)
            {
                SkipCutScene();
            }
        }
        else
        {
            skipTimer = Mathf.Max(skipTimer - Time.deltaTime, 0);
        }
    }*/

    public void SkipCutScene()
    {
        StopCoroutine(cutsceneCoroutine);
        if (videoPlayer.isPlaying)
        {
            videoPlayer.Stop();
        }
        StartCoroutine(MoveToNextScene());
    }


    private IEnumerator CutsceneSequence()
    {
        yield return new WaitForSeconds(3f);
        if (storyManager != null)
        {
            yield return StorySequence();
        }

        if (videoClips != null && videoClips.Length > 0 && !storyManager.activeInHierarchy)
        {
            yield return PlayVideosSequentially();
        }
        StartCoroutine(MoveToNextScene());
    }

    private IEnumerator StorySequence()
    {
        videoPlayer.isLooping = true;
        videoPlayer.clip = videoClips[0];
        videoPlayer.Prepare();
        yield return new WaitUntil(() => videoPlayer.isPrepared);
        videoPlayer.Play(); 
        yield return new WaitUntil(() => videoPlayer.isPlaying);
        yield return FadeUI.FadeOut(delay);
        storyManager.SetActive(true);
        yield return new WaitUntil(() => StoryManagerCompleted());
        storyManager.SetActive(false);
    }

    private bool StoryManagerCompleted()
    {
        StoryManager manager = storyManager.GetComponent<StoryManager>();
        if (manager != null)
        {
            return manager.IsCompleted();
        }
        else
        {
            return false; 
        }
    }

    private IEnumerator PlayVideosSequentially()
    {
        for (int i = 1; i < videoClips.Length; i++)
        {
            videoPlayer.isLooping = false;
            videoPlayer.clip = videoClips[i];
            videoPlayer.Prepare();
            yield return new WaitUntil(() => videoPlayer.isPrepared);
            videoPlayer.Play();

            Debug.Log($"Playing clip {i}: {videoClips[i].name}");
            yield return new WaitUntil(() => !videoPlayer.isPlaying);

            Debug.Log($"Finished playing clip {i}: {videoClips[i].name}");
        }

    }

    private IEnumerator MoveToNextScene()
    {
        yield return FadeUI.FadeIn(delay);
        Debug.Log("Gameplay started!");
        SceneManager.LoadScene(SceneName, LoadSceneMode.Single);
    }
}

