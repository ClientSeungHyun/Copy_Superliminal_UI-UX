using UnityEngine;
using UnityEngine.Video;

public class VideoEndCheck : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        VideoPlayer videoPlayer = GetComponent<VideoPlayer>();

        videoPlayer.Play();
        videoPlayer.loopPointReached += OnVideoFinished;
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        SceneTransitionManager.Instance.ChangeLevel("Map");
    }
}
