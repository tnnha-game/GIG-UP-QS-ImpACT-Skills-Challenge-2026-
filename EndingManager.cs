using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class EndingManager : MonoBehaviour
{
    public static EndingManager Instance;

    [System.Serializable]
    public class EndingUI
    {
        public string endingName;      
        public GameObject panelObject; 

        [Header("Chỉ kéo các ô chứa thông số biến động")]
        public TextMeshProUGUI cashText;    
        public TextMeshProUGUI skillText;   
        public TextMeshProUGUI healthText;  
        public TextMeshProUGUI careerText;  
        public TextMeshProUGUI legalText;   
    }

    [Header("--- 6 Manual Endings ---")]
    public EndingUI burnout;
    public EndingUI greed;
    public EndingUI elite;
    public EndingUI stable;
    public EndingUI struggle; 
    public EndingUI bankrupt;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // --- HÀM GỌI ĐÍCH DANH ENDING (Dùng khi game over giữa chừng) ---
    public void ShowEnding(string type)
    {
        Debug.Log($"<color=cyan>[EndingManager]</color> Kích hoạt Panel: <b>{type}</b>");
        
        HideAllPanels();

        EndingUI targetUI = null;
        switch (type)
        {
            case "Burnout": targetUI = burnout; break;
            case "Greed": targetUI = greed; break;
            case "Elite": targetUI = elite; break;
            case "Stable": targetUI = stable; break;
            case "Struggle": targetUI = struggle; break;
            case "Bankrupt": targetUI = bankrupt; break;
            default: targetUI = struggle; break;
        }

        ActivateEndingUI(targetUI, type);
    }

    // --- HÀM KIỂM TRA TỰ ĐỘNG (Gọi khi hết 90 ngày) ---
    public void CheckForEnding()
    {
        if (PlayerStats.Instance == null) return;
        PlayerStats p = PlayerStats.Instance;

        // --- BƯỚC 0: NGHIỆM THU DỰ ÁN (FINAL SPRINT BONUS) ---
        // Chỉ cộng tiền nếu Alex sống sót (Health > 0 và Stress < 100)
        if (p.isFinalSprintActive && p.health > 0 && p.stress < 100)
        {
            p.cash += 3000;
            Debug.Log("<color=green>[SYSTEM]</color> Final Sprint Bonus $3,000 added to account!");
        }
        
        // 1. Check Survival (Ưu tiên số 1 - Sinh tồn)
        if (p.health <= 0 || p.stress >= 100) { ShowEnding("Burnout"); return; }
    
        // 2. Check Đạo đức (Ưu tiên số 2 - Integrity)
        // Nếu có vi phạm đạo đức, dù giàu đến đâu cũng vào Greed
        if (p.isTainted) { ShowEnding("Greed"); return; }

        // 3. Check Thành tựu (Elite -> Stable -> Bankrupt -> Struggle)
        // ELITE: Master of all metrics
        if (p.cash >= 5000 && p.health >= 70 && p.stress <= 30 && p.skill >= 1000 && p.currentJobTier == "Hard") 
        {
            ShowEnding("Elite");
        }
        // STABLE: Solid foundation
        else if (p.cash >= 2000 && p.health >= 50 && p.skill >= 500 && (p.currentJobTier == "Medium" || p.currentJobTier == "Hard")) 
        {
            ShowEnding("Stable");
        }
        // BANKRUPT: Out of cash
        else if (p.cash <= 0)
        {
            ShowEnding("Bankrupt");
        }
        // STRUGGLE: The default cycle
        else 
        {
            ShowEnding("Struggle");
        }
    }

    private void ActivateEndingUI(EndingUI ui, string type)
    {
        if (ui == null || ui.panelObject == null) 
        {
            Debug.LogError($"<color=red>[EndingManager]</color> LỖI: Panel {type} chưa được gán trong Inspector!");
            return;
        }

        // Dừng game ngay lập tức
        Time.timeScale = 0; 

        // Hiển thị Panel
        ui.panelObject.SetActive(true);
        ui.panelObject.transform.SetAsLastSibling(); 

        // Đổ dữ liệu từ PlayerStats vào UI
        PlayerStats p = PlayerStats.Instance;
        if (p != null)
        {
            if (ui.cashText) ui.cashText.text = $"Final Balance: ${p.cash:F1}";
            if (ui.skillText) ui.skillText.text = $"Total Skill: {p.skill}";
            
            if (ui.healthText) 
            {
                string status = "Stable";
                if (p.health <= 0 || p.stress >= 100) status = "Collapsed";
                else if (p.health < 40) status = "Weakened";
                ui.healthText.text = $"Vitality Record: {status}";
            }
            
            if (ui.careerText) ui.careerText.text = $"Final Job: {p.lastJobName}"; 

            if (ui.legalText) 
            {
                ui.legalText.text = p.isTainted ? "Legal Record: TAINTED" : "Legal Record: CLEAN";
                ui.legalText.color = p.isTainted ? Color.purple : Color.purple;
            }
        }

        // Phát âm thanh nếu có SoundManager
        if (SoundManager.Instance != null) SoundManager.Instance.PlayEndingStinger(type);
    }

    private void HideAllPanels()
    {
        if (burnout.panelObject) burnout.panelObject.SetActive(false);
        if (greed.panelObject) greed.panelObject.SetActive(false);
        if (elite.panelObject) elite.panelObject.SetActive(false);
        if (stable.panelObject) stable.panelObject.SetActive(false);
        if (struggle.panelObject) struggle.panelObject.SetActive(false);
        if (bankrupt.panelObject) bankrupt.panelObject.SetActive(false);
    }

    public void RestartGame()
    {
        Time.timeScale = 1; 
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void BackToMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }
}