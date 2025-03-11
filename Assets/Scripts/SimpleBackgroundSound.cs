using UnityEngine;

public class SimpleBackgroundSound : MonoBehaviour
{
    public static SimpleBackgroundSound Instance;

    [Header("Audio Settings")]
    [SerializeField]
    private AudioClip[] musicTracks;

    [SerializeField]
    private bool playOnAwake = true;

    [SerializeField]
    private int defaultTrack = 0;

    [SerializeField]
    private float volume = 0.7f;

    [SerializeField]
    private bool dontDestroyOnLoad = true;

    private AudioSource audioSource;
    private int currentTrack = -1;

    private void Awake()
    {
        // Simple singleton implementation
        if (Instance == null)
        {
            Instance = this;

            if (dontDestroyOnLoad)
                DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Get or add AudioSource component
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Configure AudioSource
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.volume = volume;
    }

    private void Start()
    {
        if (playOnAwake && musicTracks.Length > 0)
        {
            PlayMusic(defaultTrack);
        }
    }

    /// <summary>
    /// Play a specific music track
    /// </summary>
    /// <param name="trackIndex">Index of the music track to play</param>
    public void PlayMusic(int trackIndex)
    {
        // Validate track index
        if (trackIndex < 0 || trackIndex >= musicTracks.Length)
        {
            Debug.LogWarning("Invalid track index: " + trackIndex);
            return;
        }

        // Don't restart if it's already playing this track
        if (currentTrack == trackIndex && audioSource.isPlaying)
            return;

        // Set the new track and play it
        audioSource.clip = musicTracks[trackIndex];
        audioSource.Play();
        currentTrack = trackIndex;
    }

    /// <summary>
    /// Stop the currently playing music
    /// </summary>
    public void StopMusic()
    {
        audioSource.Stop();
    }

    /// <summary>
    /// Pause the currently playing music
    /// </summary>
    public void PauseMusic()
    {
        audioSource.Pause();
    }

    /// <summary>
    /// Resume the paused music
    /// </summary>
    public void ResumeMusic()
    {
        audioSource.UnPause();
    }

    /// <summary>
    /// Set the volume of the music
    /// </summary>
    /// <param name="newVolume">Volume value between 0 and 1</param>
    public void SetVolume(float newVolume)
    {
        volume = Mathf.Clamp01(newVolume);
        audioSource.volume = volume;
    }
}
