using UnityEngine;
using UnityEngine.SceneManagement;

public class GameClearManager : MonoBehaviour
{
    public string mainMenuSceneName = "MainMenu";

    /// <summary>
    /// Hàm này được gọi bởi nút "Về Menu Chính"
    /// </summary>
    public void GoToMainMenu()
    {
        // Reset lại mọi thứ trước khi về menu
        Time.timeScale = 1f;
        GameManager.isPaused = false;
        GameManager.persistentGunData = null; // Reset súng
        GameManager.hasKey = false; // Reset chìa khóa

        SceneManager.LoadScene(mainMenuSceneName);
    }
}