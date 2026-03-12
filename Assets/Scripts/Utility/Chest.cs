using UnityEngine;

// Thêm 2 dòng này để đảm bảo Rương luôn có 2 component này
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
public class Chest : MonoBehaviour, IInteractable
{
    [Header("Cài đặt Rương")]
    public bool isKeyChest = false; // Tích nếu đây là rương có chìa

    [Header("Âm thanh (Kéo thả)")]
    public AudioClip openSound; // Tiếng cọt kẹt khi mở rương
    public AudioClip keyAppearSound; // Tiếng "tinh!" khi có chìa khóa
    public AudioClip emptySound; // Tiếng "phụt" khi rương rỗng

    [Tooltip("Delay (giây) trước khi phát âm thanh 'có chìa' hoặc 'rỗng' (để khớp anim)")]
    public float itemSoundDelay = 0.5f; // Nửa giây

    private Animator animator;
    private AudioSource audioSource; // Component để phát âm thanh
    private bool isOpen = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>(); // Lấy component AudioSource
    }

    // Đây là hàm được gọi khi nhấn "E"
    public void Interact()
    {
        if (isOpen) return; // Đã mở rồi thì không chạy nữa

        isOpen = true;
        animator.SetTrigger("Open"); // Chạy animation

        // 1. Phát tiếng "mở rương" (cọt kẹt) ngay lập tức
        if (openSound != null)
        {
            audioSource.PlayOneShot(openSound);
        }

        // 2. Gọi hàm "PlayItemSound" sau 0.5 giây (để chờ anim mở ra)
        Invoke("PlayItemSound", itemSoundDelay); 
    }

    /// <summary>
    /// Hàm này được Invoke gọi sau 0.5 giây
    /// </summary>
    private void PlayItemSound()
    {
        if (isKeyChest)
        {
            // 3. Nếu là rương có chìa -> Phát tiếng "có chìa khóa"
            if (keyAppearSound != null)
            {
                audioSource.PlayOneShot(keyAppearSound); 
            }
            
            // Kích hoạt sự kiện (UI chìa khóa hiện ra)
            GameManager.CollectKey();
        }
        else
        {
            // 4. Nếu là rương rỗng -> Phát tiếng "rương rỗng"
            if (emptySound != null)
            {
                audioSource.PlayOneShot(emptySound);
            }
            Debug.Log("Rương này rỗng!");
        }
    }
}