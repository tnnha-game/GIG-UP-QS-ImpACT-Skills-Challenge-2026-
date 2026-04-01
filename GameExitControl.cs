using UnityEngine;
using UnityEngine.SceneManagement; 

public class GameExitControl : MonoBehaviour
{
    // Hàm dành cho nút MAIN MENU
    public void GoToMainMenu()
    {
        // load Scene1 (màn hình có chữ GIG-UP)
        SceneManager.LoadScene("Scene1"); 
    }

    // EXIT GAME
    public void QuitGame()
    {
        // Lệnh thoát ứng dụng khi đã xuất file (Build)
        Application.Quit();
        
        Debug.Log("Đã bấm nút thoát game!"); 
    }
}
