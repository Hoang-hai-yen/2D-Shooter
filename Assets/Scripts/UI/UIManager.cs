using UnityEngine;
using UnityEngine.UI; // Rất quan trọng, cần cho UI (Image, Slider...)
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem; // <-- THÊM DÒNG NÀY
public class UIManager : MonoBehaviour
{
    [Header("Gán từ Hierarchy")]
    public Health playerHealth;      // Kéo Player vào đây
    public Image healthFillImage;    // Kéo object "HealthBar_Fill" vào đây
    [Header("Pause Menu (Gán từ Hierarchy)")]
    public GameObject pauseMenuPanel; // Kéo "PauseMenuPanel" vào đây
    public GameObject loseMenuPanel; // <-- 1. THÊM BIẾN NÀY
    public string mainMenuSceneName = "MainMenu";
    [Header("Game UI (Gán từ Hierarchy)")]
    public GameObject keyIcon;
    [Header("Game Clear (Gán từ Hierarchy)")]
    public GameObject gameClearPanel; // Kéo "GameClearPanel" vào đây
    void Start()
    {
        if (playerHealth == null)
        {
            Debug.LogError("Chưa gán Player Health cho UIManager!");
            return;
        }

        // Đăng ký để lắng nghe sự kiện OnHealthChanged từ script Health
        playerHealth.OnHealthChanged += UpdateHealthBar;
        playerHealth.OnDeath += ShowLosePanel;
        // Cập nhật máu lần đầu tiên (phòng trường hợp máu không đầy)
        UpdateHealthBar(playerHealth.currentHealth, playerHealth.maxHealth);
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }
        if (loseMenuPanel != null) loseMenuPanel.SetActive(false); // <-- 3. THÊM DÒNG NÀY
        if (gameClearPanel != null) gameClearPanel.SetActive(false);
        GameManager.OnKeyCollected += ShowKeyIcon;

        // Ẩn icon chìa khóa khi bắt đầu
        if (keyIcon != null)
        {
            keyIcon.SetActive(false);
        }
    }
    void OnDestroy()
    {
        // Hủy đăng ký khi UIManager bị phá hủy (để tránh lỗi)
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged -= UpdateHealthBar;
            playerHealth.OnDeath -= ShowLosePanel;
        }
        GameManager.OnKeyCollected -= ShowKeyIcon;
        
    }

    /// <summary>
    /// Hàm này được gọi tự động mỗi khi OnHealthChanged được kích hoạt
    /// </summary>
    private void UpdateHealthBar(int current, int max)
    {
        if (healthFillImage != null)
        {
            // Tính toán tỉ lệ máu (một số từ 0.0 đến 1.0)
            float fillAmount = (float)current / (float)max;

            // Gán tỉ lệ đó cho thanh máu (Image Fill)
            healthFillImage.fillAmount = fillAmount;
        }
    }
    void Update()
    {
    if (Keyboard.current.qKey.wasPressedThisFrame) 
       {
            if (GameManager.isPaused)
            {
                ResumeGame(); // Nếu đang pause -> Bỏ pause
            }
            else
            {
                PauseGame(); // Nếu đang chơi -> Pause
            }
        }
    }
    public void PauseGame()
    {
        GameManager.isPaused = true;
        Time.timeScale = 0f; // Dừng thời gian (vật lý, animation...)
        pauseMenuPanel.SetActive(true); // Bật menu
    }

    public void ResumeGame()
    {
        GameManager.isPaused = false;
        Time.timeScale = 1f; // Chạy lại thời gian
        pauseMenuPanel.SetActive(false); // Tắt menu
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        GameManager.isPaused = false;
        GameManager.persistentGunData = null; // <-- THÊM DÒNG NÀY
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void Retry()
    {
        Time.timeScale = 1f;
        GameManager.isPaused = false;
        GameManager.persistentGunData = null; // <-- THÊM DÒNG NÀY
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    private void ShowKeyIcon()
    {
        if (keyIcon != null)
        {
            keyIcon.SetActive(true);
        }
    }
    public void ShowLosePanel()
    {
        pauseMenuPanel.SetActive(false); // Ẩn menu pause (nếu đang bật)
        loseMenuPanel.SetActive(true); // Bật menu "You Lose"
        
        Time.timeScale = 0f; // Dừng game
        GameManager.isPaused = true; // Báo cho các script khác biết là game đã dừng
    }
    public void ShowGameClearPanel()
    {
        Debug.Log("PANEL: 1. Đang chạy ShowGameClearPanel...");

        // Ẩn các UI khác
        if (healthFillImage != null)
        {
            if (healthFillImage.transform.parent != null)
            {
                healthFillImage.transform.parent.gameObject.SetActive(false); 
            }
        }
        if (keyIcon != null)
        {
            keyIcon.SetActive(false);
        }
        
        Debug.Log("PANEL: 2. Đã ẩn UI cũ.");

        // Hiện Panel
        if (gameClearPanel != null)
        {
            Debug.Log("PANEL: 3. Đã tìm thấy Panel, đang SetActive(true)!");
            gameClearPanel.SetActive(true);
        }
        else
        {
            // Lỗi này không thể xảy ra vì bạn đã gán rồi
            Debug.LogError("PANEL: LỖI! gameClearPanel BỊ NULL!");
        }
        
        // Dừng game
        Time.timeScale = 0f;
        GameManager.isPaused = true; 
        Debug.Log("PANEL: 4. Đã dừng game. Panel đáng lẽ phải hiện.");
    }
    /// <summary>
    /// Hàm này được gọi bởi nút "Chơi Lại"
    /// </summary>
}