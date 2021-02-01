using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class VideoPlayerStopControl : MonoBehaviour
{
    private VideoPlayer videoPlayer = default;
    
    // Start is called before the first frame update
    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        
        videoPlayer.Play();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (videoPlayer.clockTime >= videoPlayer.length)
        {
            SceneManager.LoadScene(1);
        }
    }
}
