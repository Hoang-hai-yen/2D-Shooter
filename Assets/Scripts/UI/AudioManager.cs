using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Âm thanh (Kéo thả)")]
    public AudioClip backgroundMusic;
    public AudioClip loseSound;

    [Header("Tham chiếu (Kéo thả)")]
    public Health playerHealth; // Kéo object Player vào đây

    // Hai AudioSource: 1 cho nhạc (loop), 1 cho SFX (one-shot)
    private AudioSource bgmSource;
    private AudioSource sfxSource;

    void Start()
    {
        // Lấy 2 AudioSource từ object này
        AudioSource[] sources = GetComponents<AudioSource>();
        bgmSource = sources[0];
        sfxSource = sources[1];

        // Cài đặt và chơi nhạc nền
        if (backgroundMusic != null)
        {
            bgmSource.clip = backgroundMusic;
            bgmSource.loop = true;
            bgmSource.Play();
        }

        // Lắng nghe sự kiện Player chết
        if (playerHealth != null)
        {
            playerHealth.OnDeath += PlayLoseSound;
        }
    }

    void OnDestroy()
    {
        // Hủy đăng ký sự kiện
        if (playerHealth != null)
        {
            playerHealth.OnDeath -= PlayLoseSound;
        }
    }

    /// <summary>
    /// Được gọi bởi sự kiện OnDeath của Player
    /// </summary>
    public void PlayLoseSound()
    {
        // Dừng nhạc nền
        bgmSource.Stop();

        // Phát âm thanh thua
        if (loseSound != null)
        {
            sfxSource.PlayOneShot(loseSound);
        }
    }
}