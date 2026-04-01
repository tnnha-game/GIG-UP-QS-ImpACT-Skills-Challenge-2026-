using UnityEngine;
using UnityEngine.SceneManagement;

public class UniversalSettings : MonoBehaviour
{
    [Header("--- UI Panel ---")]
    [Tooltip("Kéo Panel chứa 2 nút MainMenu/Exit vào đây")]
    public GameObject settingsPanel; 

    public void ToggleSettings()
    {
        if (settingsPanel != null)
        {
            // 1. Đảo ngược trạng thái đóng/mở của Panel
            bool isActive = !settingsPanel.activeSelf;
            settingsPanel.SetActive(isActive);

            // 2. Gọi âm thanh Toggle (glitch_002) từ SoundManager
            if (SoundManager.Instance != null) 
            {
                SoundManager.Instance.PlayToggleSound();
            }

            // 3. Dừng thời gian khi mở bảng (0) và chạy lại khi đóng bảng (1)
            Time.timeScale = isActive ? 0f : 1f;
        }
    }

    // --- HÀM CHO NÚT MAIN MENU ---
    public void BackToMainMenu()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene("Scene1");
    }

    // --- HÀM CHO NÚT EXIT GAME ---
    public void ExitGame()
    {
        Debug.Log("Exiting Game...");
        Application.Quit();
    }
}
