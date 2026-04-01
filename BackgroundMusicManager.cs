using UnityEngine;

public class BackgroundMusicManager : MonoBehaviour
{
    public static BackgroundMusicManager Instance;

    [Header("--- Audio Setup ---")]
    public AudioSource musicSource; 
    [Range(0f, 1f)] public float musicVolume = 0.25f;

    private bool isFadingOut = false;

    void Awake()
    {
        // 1. Singleton 
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // 2. AudioSource 
        if (musicSource == null) musicSource = GetComponent<AudioSource>();

        // 3. Cấu hình Loop cho nhạc nền Arcade #17
        musicSource.loop = true;
        musicSource.playOnAwake = true;
        musicSource.volume = musicVolume;
    }

    void Update()
    {
        // 4. Kiểm tra trạng thái Game Over từ PlayerStats để xử lý nhạc
        if (PlayerStats.Instance != null && PlayerStats.Instance.gameEnded && !isFadingOut)
        {
            isFadingOut = true;
        }

        if (isFadingOut)
        {
            FadeOutMusic();
        }
    }

    private void FadeOutMusic()
    {
        // Nhạc nhỏ dần khi game over để nhường chỗ cho tiếng Ending của SoundManager
        if (musicSource.volume > 0)
        {
            musicSource.volume -= Time.unscaledDeltaTime * 0.5f;
        }
        else
        {
            musicSource.Stop();
        }
    }

    // Đổi nhạc hoặc bật lại nhạc khi chơi ván mới
    public void RestartMusic()
    {
        isFadingOut = false;
        musicSource.volume = musicVolume;
        if (!musicSource.isPlaying) musicSource.Play();
    }
}
