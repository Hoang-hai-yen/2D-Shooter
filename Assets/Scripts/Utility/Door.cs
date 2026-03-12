using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections; // Cần cho Coroutine

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
public class Door : MonoBehaviour, IInteractable
{
    [Header("Cài đặt Cửa")]
    public string nextSceneName = "Level_2"; // Dùng cho cửa thường
    
    [Tooltip("Tích vào nếu đây là cửa cuối cùng")]
    public bool isFinalDoor = false;
    
    [Tooltip("Tên của Scene Phá Đảo")]
    public string gameClearSceneName = "GameClear"; // Tên Scene bạn tạo ở Bước 1
    
    public float delayBeforeLoad = 1.5f;

    [Header("Âm thanh (Kéo thả)")]
    public AudioClip openSound;
    public AudioClip lockedSound;
    
    private Animator animator;
    private AudioSource audioSource;
    private bool isOpen = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    public void Interact()
    {
        if (isOpen) return;

        if (GameManager.hasKey)
        {
            // --- CÓ CHÌA KHÓA ---
            isOpen = true;
            animator.SetTrigger("Open");
            if (openSound != null) audioSource.PlayOneShot(openSound);
            
            if (isFinalDoor)
            {
                // Đây là cửa cuối -> Tải Scene GameClear
                // (Không cần dùng Coroutine nữa vì chúng ta reset Time.timeScale)
                Invoke("LoadGameClearScene", delayBeforeLoad);
            }
            else
            {
                // Đây là cửa thường -> Chuyển màn
                Invoke("LoadNextLevel", delayBeforeLoad);
            }
        }
        else
        {
            // --- KHÔNG CÓ CHÌA KHÓA ---
            if (lockedSound != null) audioSource.PlayOneShot(lockedSound);
            Debug.Log("CỬA ĐANG KHÓA!");
        }
    }

    private void LoadNextLevel()
    {
        SceneManager.LoadScene(nextSceneName);
    }

    // --- HÀM MỚI ĐỂ TẢI SCENE THẮNG ---
    private void LoadGameClearScene()
    {
        // Reset thời gian (phòng trường hợp game đang pause)
        Time.timeScale = 1f; 
        GameManager.isPaused = false;
        
        SceneManager.LoadScene(gameClearSceneName);
    }
}