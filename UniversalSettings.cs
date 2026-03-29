using UnityEngine;
using UnityEngine.SceneManagement;

public class UniversalSettings : MonoBehaviour
{
    [Header("--- UI Panel ---")]
    [Tooltip("Kéo Panel chứa 2 nút MainMenu/Exit vào đây")]
    public GameObject settingsPanel; 

    // --- HÀM CHO NÚT BÁNH RĂNG ---
    // Gán hàm này vào Event OnClick của nút Bánh Răng ở TẤT CẢ các Scene
    public void ToggleSettings()
    {
        if (settingsPanel != null)
        {
            // 1. Đảo ngược trạng thái đóng/mở của Panel
            bool isActive = !settingsPanel.activeSelf;
            settingsPanel.SetActive(isActive);

            // 2. Gọi âm thanh Toggle (glitch_002) từ SoundManager
            // Lưu ý: Tên hàm phải khớp 100% với SoundManager.cs
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
        // Luôn Reset lại thời gian về 1 trước khi chuyển Scene để tránh bị đứng game
        Time.timeScale = 1f; 
        SceneManager.LoadScene("Scene1"); // Đảm bảo "Scene1" đúng tên Scene của bạn
    }

    // --- HÀM CHO NÚT EXIT GAME ---
    public void ExitGame()
    {
        Debug.Log("Exiting Game...");
        Application.Quit();
    }
}