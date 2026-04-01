using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance;

    [Header("--- UI Panel & Static Buttons ---")]
    public GameObject eventPanel;      
    public GameObject blackOverlay;    
    public Image backgroundDisplay;    
    public Image titleImageDisplay;   
    public TextMeshProUGUI messageText; 

    [Space(10)]
    public GameObject understandButton; 
    public GameObject acceptButton;    
    public GameObject declineButton;    

    [Header("--- 8 Sets of Sprites ---")]
    [Tooltip("Kéo 8 tấm ảnh Nền tương ứng (0-7)")]
    public Sprite[] eventBackgrounds; 
    [Tooltip("Kéo 8 tấm ảnh Chữ Tiêu Đề tương ứng (0-7)")]
    public Sprite[] eventTitles;      

    private int currentEventIndex;
    private SoundBridge bridge;

    // Toàn bộ nội dung Message Content & Impact
    private string[] eventContents = {
        "Due to a sudden spike in local property taxes and urban demand, your monthly rent has been adjusted.\n\n<color=#FF4444><b>Impact:</b></color> -$150. If Cash < $150: Forced move to Shared Dorm (H01).",
        "The rapid deployment of AI-automation has saturated the market for entry-level tasks.\n\n<color=#FF4444><b>Impact:</b></color> Wage -40% Permanently for Jobs J01 - J05. (Forces upskilling).",
        "The platform is rolling out a major core update, causing global visibility fluctuations.\n\n<color=#FF4444><b>Impact:</b></color> 5% Chance. Income -15% for 3 days. Affects J08 - J16 only.",
        "The economy entered the Digital Transformation phase. Companies are bidding for AI talent.\n\n<color=#44FF44><b>Impact:</b></color> Wage +40% for Jobs J10 - J14.",
        "A global conglomerate needs a Lead Architect to finish a critical system before year-end.\n\n<color=#FFFF44><b>If ACCEPT:</b></color> Bonus +$3,000, Energy x3 usage, Stress +30 per session. Survive to Day 90, or the $3,000 bonus is void.",
        "Overwork and poor self-management have taken a serious toll on your health.\n\n<color=#FF4444><b>Impact:</b></color> Energy recovery -40%. Must pay $500 Medical Fee to clear.",
        "A critical security flaw was exploited. Hackers hijacked your payment tunnel.\n\n<color=#FF4444><b>Impact:</b></color> Income = $0 for current session, Stress +30. (Risk: Basic 15%, Pro 2%).",
        "Extreme stress and lack of recovery have led to a mental collapse.\n\n<color=#FF4444><b>Impact:</b></color> 1-day Work Ban, Health -10, -$100 Medical fees."
    };

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        bridge = FindFirstObjectByType<SoundBridge>();
    }

    public void TriggerEvent(string id, string msg, int index)
    {
        // Kiểm tra an toàn index
        if (index < 0 || index >= 8) return;
        
        // Nếu đang mở bảng rồi thì không mở thêm
        if (eventPanel.activeSelf) return; 

        currentEventIndex = index;

        // 1. Thay đổi hình ảnh (Nền và Tiêu đề) dựa theo Index
        if (index < eventBackgrounds.Length) backgroundDisplay.sprite = eventBackgrounds[index];
        if (index < eventTitles.Length) titleImageDisplay.sprite = eventTitles[index];

        // 2. Đổ nội dung chữ (Hỗ trợ Rich Text để Impact có màu)
        messageText.color = Color.white; // Đảm bảo chữ trắng trên nền tối
        messageText.text = eventContents[index];

        // 3. Điều khiển hiển thị Nút (Understand vs Accept/Decline)
        bool isSprint = (index == 4); 
        if (understandButton != null) understandButton.SetActive(!isSprint);
        if (acceptButton != null) acceptButton.SetActive(isSprint);
        if (declineButton != null) declineButton.SetActive(isSprint);

        // 4. Hiển thị UI và đưa lên trên cùng
        if (blackOverlay != null) {
            blackOverlay.SetActive(true);
            blackOverlay.transform.SetAsLastSibling();
        }
        
        eventPanel.SetActive(true);
        eventPanel.transform.SetAsLastSibling();

        PlaySfxForEvent(index);
        Time.timeScale = 0f; // Dừng game
    }

    // --- CÁC HÀM XỬ LÝ NÚT BẤM (Gán vào OnClick trong Inspector) ---
    public void OnClickUnderstand() { ExecuteImpact(); CloseEvent(); }
    public void OnClickAccept() { ExecuteImpact(); CloseEvent(); }
    public void OnClickDecline() { CloseEvent(); }

    private void CloseEvent()
    {
        if (bridge != null) bridge.PlayCommon();
        
        eventPanel.SetActive(false);
        if (blackOverlay != null) blackOverlay.SetActive(false);
        
        Time.timeScale = 1f; // Chạy lại game
    }

    private void ExecuteImpact()
    {
        PlayerStats p = PlayerStats.Instance;
        if (p == null) return;

        // Giữ nguyên toàn bộ logic Case 0-7 của Hà
        switch (currentEventIndex)
        {
            case 0: // Rent Hike
                if (p.cash >= 150) p.cash -= 150;
                else p.currentHousingID = "H01";
                break;
            case 1: p.wageMultiplier_LowTier = 0.6f; break;
            case 2: p.incomeMultiplier_MidTier = 0.85f; p.wageMultiplier_HighTier = 0.85f; p.algoShiftDaysLeft = 3; break;
            case 3: p.wageMultiplier_HighTier = 1.4f; break;
            case 4: // Final Sprint
                p.isFinalSprintActive = true; 
                p.stress = Mathf.Clamp(p.stress + 30, 0, 100); 
                break;
            case 5: // Health Audit
                if (p.cash >= 500) { p.cash -= 500; p.healthAuditMultiplier = 1.0f; }
                else { p.cash = 0; p.UpdateUI(); if (EndingManager.Instance != null) EndingManager.Instance.ShowEnding("Burnout"); }
                break;
            case 6: p.isSessionIntercepted = true; p.stress = Mathf.Clamp(p.stress + 30, 0, 100); break;
            case 7: p.isWorkBanned = true; p.cash = Mathf.Max(0, p.cash - 100); p.health = Mathf.Clamp(p.health - 10, 0, 100); break;
        }
        p.UpdateUI();
    }

    private void PlaySfxForEvent(int index)
    {
        if (bridge == null) bridge = FindFirstObjectByType<SoundBridge>();
        if (bridge == null) return;

        switch (index)
        {
            case 3: case 4: bridge.PlayEventGood(); break;
            case 5: case 6: case 7: case 2: bridge.PlayEventWarning(); break;
            default: bridge.PlayEventBad(); break;
        }
    }
}
