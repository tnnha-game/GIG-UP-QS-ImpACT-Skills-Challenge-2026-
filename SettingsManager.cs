using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance;

    private Stack<GameObject> history = new Stack<GameObject>();

    [Header("--- 1. UI PANELS ---")]
    public GameObject settingsPanel; 
    public GameObject tutorialPanel; 
    public GameObject jobSelectPanel;
    public GameObject confirmPopUp;

    [Header("--- 2. DATA ---")]
    private JobData selectedJob; 
    private bool isSceneLoading = false; 

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // Đóng tất cả Panel để hiện màn hình chính (Start Menu)
        if (settingsPanel) settingsPanel.SetActive(false);
        if (tutorialPanel) tutorialPanel.SetActive(false);
        if (jobSelectPanel) jobSelectPanel.SetActive(false);
        if (confirmPopUp) confirmPopUp.SetActive(false);
        
        isSceneLoading = false;
        selectedJob = null;
        Time.timeScale = 1; 
        history.Clear(); // Xóa lịch sử khi game mới bắt đầu
    }

    // --- HỆ THỐNG ĐIỀU HƯỚNG (Navigation System) ---
    public void OpenNewPage(GameObject nextPanel)
    {
        if (nextPanel == null) return;

        // Popup xác nhận hiện đè, không tính vào lịch sử
        if (nextPanel == confirmPopUp)
        {
            nextPanel.SetActive(true);
            return; 
        }

        GameObject currentActive = GetActivePanel();
        
        // Nếu đang ở một trang (như Tutorial), ẩn nó đi và lưu vào Stack
        if (currentActive != null && currentActive != nextPanel)
        {
            currentActive.SetActive(false);
            history.Push(currentActive); 
        }
        
        nextPanel.SetActive(true);
    }

    public void GoBack()
    {
        // 1. Ưu tiên đóng Popup trước
        if (confirmPopUp != null && confirmPopUp.activeSelf)
        {
            if (SoundManager.Instance != null) SoundManager.Instance.PlayNoClick();
            confirmPopUp.SetActive(false);
            isSceneLoading = false; 
            return;
        }

        // 2. Nếu có lịch sử trang, quay lại trang trước
        if (history.Count > 0)
        {
            if (SoundManager.Instance != null) SoundManager.Instance.PlayCommonClick();
            
            GameObject currentActive = GetActivePanel();
            if (currentActive != null) currentActive.SetActive(false);

            GameObject previousPanel = history.Pop(); 
            previousPanel.SetActive(true);
        }
        else
        {
            // 3. Nếu không còn trang nào trong lịch sử, tắt luôn trang hiện tại (về Start Menu)
            GameObject currentActive = GetActivePanel();
            if (currentActive != null)
            {
                if (SoundManager.Instance != null) SoundManager.Instance.PlayCommonClick();
                currentActive.SetActive(false);
            }
        }
    }

    // Hàm nhận diện trang đang mở - Key chính để sửa lỗi Back
    private GameObject GetActivePanel()
    {
        if (settingsPanel && settingsPanel.activeSelf) return settingsPanel;
        if (tutorialPanel && tutorialPanel.activeSelf) return tutorialPanel;
        if (jobSelectPanel && jobSelectPanel.activeSelf) return jobSelectPanel;
        return null;
    }

    // --- LOGIC LUỒNG GAME ---
    public void StartGame() { OpenNewPage(tutorialPanel); }
    public void ShowJobSelection() { OpenNewPage(jobSelectPanel); }

    public void OpenConfirm(JobData job) 
    { 
        if (job == null) return;
        selectedJob = job;
        isSceneLoading = false; 
        if (confirmPopUp) confirmPopUp.SetActive(true); 
    }

    public void ConfirmAndStartGame()
    {
        if (isSceneLoading) return;

        // Chống lỗi rỗng dữ liệu
        if (selectedJob == null && PlayerStats.Instance != null)
            selectedJob = PlayerStats.Instance.selectedJob;

        if (selectedJob == null)
        {
            Debug.LogError("Chưa có JobData! Hà kiểm tra lại việc gán JobCard nhé.");
            return;
        }

        isSceneLoading = true;

        if (PlayerStats.Instance != null)
        {
            PlayerStats.Instance.ResetPlayerForNewGame();
            PlayerStats.Instance.selectedJob = selectedJob;
            PlayerStats.Instance.lastJobName = selectedJob.jobName;
            
            PlayerPrefs.SetString("SelectedJobID", selectedJob.jobID);
            PlayerPrefs.Save();
        }

        if (SoundManager.Instance != null) SoundManager.Instance.PlayYesClick();
        SceneManager.LoadScene("Scene2"); 
    }

    public void ToggleSettings()
    {
        if (settingsPanel == null) return;
        if (settingsPanel.activeSelf) GoBack();
        else OpenNewPage(settingsPanel);
    }

    public void LoadMainMenu() { SceneManager.LoadScene("Scene1"); }

    public void QuitGame()
    {
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif 
    }
}