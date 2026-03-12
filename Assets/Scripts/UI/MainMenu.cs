using UnityEngine;
using UnityEngine.SceneManagement; // Bắt buộc phải có để chuyển scene

public class MainMenuManager : MonoBehaviour
{
    [Header("Tên Scene (Gõ đúng tên file)")]
    public string level1_SceneName = "Level_1";
    public string level2_SceneName = "Level_2";
    public string level3_SceneName = "Level_3_Boss"; // <-- 1. THÊM DÒNG NÀY
    // Thêm các level khác nếu cần...

    [Header("Tham chiếu Panel (Kéo thả)")]
    public GameObject mainMenuPanel; // Kéo MainMenuPanel vào đây
    public GameObject levelSelectPanel; // Kéo LevelSelectPanel vào đây

    void Start()
    {
        // Đảm bảo khi bắt đầu, menu chính hiện và menu chọn màn ẩn
        mainMenuPanel.SetActive(true);
        levelSelectPanel.SetActive(false);
    }

    /// <summary>
    /// Hàm này được gọi bởi nút "Chơi Ngay"
    /// </summary>
    public void PlayGame()
    {
        GameManager.persistentGunData = null; // <-- THÊM DÒNG NÀY
        SceneManager.LoadScene(level1_SceneName);
    }

    /// <summary>
    /// Hàm này được gọi bởi nút "Chọn Màn"
    /// </summary>
    public void ShowLevelSelect()
    {
        // Ẩn menu chính, hiện menu chọn màn
        mainMenuPanel.SetActive(false);
        levelSelectPanel.SetActive(true);
    }

    /// <summary>
    /// Hàm này được gọi bởi nút "Quay Lại" (trong LevelSelectPanel)
    /// </summary>
    public void ShowMainMenu()
    {
        // Ẩn menu chọn màn, hiện menu chính
        mainMenuPanel.SetActive(true);
        levelSelectPanel.SetActive(false);
    }

    /// <summary>
    /// Hàm này được gọi bởi nút "Thoát"
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    // --- CÁC HÀM TẢI LEVEL ---

    public void LoadLevel1()
    {
        SceneManager.LoadScene(level1_SceneName);
    }
    
    public void LoadLevel2()
    {
        SceneManager.LoadScene(level2_SceneName);
    }
    public void LoadLevel3()
    {
        SceneManager.LoadScene(level3_SceneName);
    }
    // Nếu bạn có nhiều level, bạn có thể làm 1 hàm chung
    // public void LoadLevel(string sceneName)
    // {
    //     SceneManager.LoadScene(sceneName);
    // }
}