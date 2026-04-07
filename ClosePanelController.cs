using UnityEngine;

public class ClosePanelController : MonoBehaviour
{
    [Header("--- UI Setup ---")]
    [Tooltip("Kéo TẤT CẢ các Panel (Shop, Job, Vital...) vào danh sách này")]
    public GameObject[] allPanels; 

    [Tooltip("Kéo main scene vào đây để hiện lại")]
    public GameObject mainHUD;

    /// </summary>
    public void CloseEverything()
    {
        // 1. Scan qua danh sách và tắt mọi bảng đang mở
        if (allPanels != null && allPanels.Length > 0)
        {
            foreach (GameObject panel in allPanels)
            {
                if (panel != null) 
                {
                    panel.SetActive(false);
                }
            }
        }

        // 2. Hiện lại màn hình chính
        if (mainHUD != null)
        {
            mainHUD.SetActive(true);
        }

        // 3. Trả lại thời gian thực cho game
        Time.timeScale = 1; 

        Debug.Log("Đã đóng tất cả các bảng và quay về HUD chính!");
    }

    // Phím tắt ESC để thoát nhanh
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseEverything();
        }
    }
}
