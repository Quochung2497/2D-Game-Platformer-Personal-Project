using System;
using System.Collections;
using UnityEngine;
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
    private bool skipRequested;
    private bool segmentActive;
    private bool videoFinished;
    private bool isTransitioning;

    public event Action SegmentChanged;
    public bool CanSkip => segmentActive && !skipRequested && !isTransitioning;

    private void OnEnable()
    {
        videoPlayer.loopPointReached += OnVideoFinished;
    }

    private void Start()
    {
        cutsceneCoroutine = StartCoroutine(CutsceneSequence());
    }

    private void OnDisable()
    {
        videoPlayer.loopPointReached -= OnVideoFinished;
        segmentActive = false;

        if (cutsceneCoroutine != null)
        {
            StopCoroutine(cutsceneCoroutine);
            cutsceneCoroutine = null;
        }
    }

    public void SkipCutScene()
    {
        if (!CanSkip) return;

        skipRequested = true;
    }

    private IEnumerator CutsceneSequence()
    {
        yield return new WaitForSeconds(3f);
        if (storyManager != null)
        {
            yield return StorySequence();
        }

        int firstVideoIndex = storyManager != null ? 1 : 0;
        if (videoClips != null && videoClips.Length > firstVideoIndex)
        {
            yield return PlayVideosSequentially(firstVideoIndex);
        }

        yield return MoveToNextScene();
        cutsceneCoroutine = null;
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
        BeginSegment();
        yield return new WaitUntil(() => skipRequested || StoryManagerCompleted());
        segmentActive = false;
        storyManager.SetActive(false);
    }

    private bool StoryManagerCompleted()
    {
        StoryManager manager = storyManager.GetComponent<StoryManager>();
        return manager != null && manager.IsCompleted();
    }

    private IEnumerator PlayVideosSequentially(int firstVideoIndex)
    {
        for (int i = firstVideoIndex; i < videoClips.Length; i++)
        {
            videoPlayer.Stop();
            videoPlayer.isLooping = false;
            videoPlayer.clip = videoClips[i];
            videoFinished = false;
            BeginSegment();

            videoPlayer.Prepare();
            yield return new WaitUntil(() => skipRequested || videoPlayer.isPrepared);
            if (!skipRequested)
            {
                videoPlayer.Play();
                yield return new WaitUntil(() => skipRequested || videoFinished);
            }

            segmentActive = false;
            videoPlayer.Stop();
        }
    }

    private void BeginSegment()
    {
        skipRequested = false;
        segmentActive = true;
        SegmentChanged?.Invoke();
    }

    private void OnVideoFinished(VideoPlayer source)
    {
        if (!source.isLooping)
        {
            videoFinished = true;
        }
    }

    private IEnumerator MoveToNextScene()
    {
        if (isTransitioning) yield break;

        isTransitioning = true;
        yield return FadeUI.FadeIn(delay);
        SceneManager.LoadScene(SceneName, LoadSceneMode.Single);
    }
}
