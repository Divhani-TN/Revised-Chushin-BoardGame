using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
public class VideoSequenceController : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public List<VideoClip> videoClips;
    private int currentVideoIndex = 0;

    void Start()
    {
        if (videoPlayer == null)
            videoPlayer = GetComponent<VideoPlayer>();

        videoPlayer.loopPointReached += OnVideoEnd;
    }

    public void PlayNextVideo()
    {
        if (videoClips.Count == 0)
            return;

        videoPlayer.clip = videoClips[currentVideoIndex];
        videoPlayer.Play();
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        currentVideoIndex++;
        if (currentVideoIndex >= videoClips.Count)
        {
            currentVideoIndex = 0; // Optionally loop back to the first video
            videoPlayer.Stop(); // Or stop playback completely
            return;
        }
        PlayNextVideo();
    }

    public void StartVideoSequence()
    {
        currentVideoIndex = 0;
        PlayNextVideo();
    }
}

