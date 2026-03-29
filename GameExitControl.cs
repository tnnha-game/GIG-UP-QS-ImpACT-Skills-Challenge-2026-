using UnityEngine;
using UnityEngine.SceneManagement; // Thư viện để chuyển cảnh

public class GameExitControl : MonoBehaviour
{
    // Hàm dành cho nút MAIN MENU
    public void GoToMainMenu()
    {
        // Nó sẽ load Scene1 (màn hình có chữ GIG-UP)
        SceneManager.LoadScene("Scene1"); 
    }

    // Hàm dành cho nút EXIT GAME
    public void QuitGame()
    {
        // Lệnh thoát ứng dụng khi đã xuất file (Build)
        Application.Quit();
        
        // Dòng này để bạn check trong Unity Editor xem nút có ăn không
        Debug.Log("Đã bấm nút thoát game!"); 
    }
}