using UnityEngine;
using TMPro;

public class TimePanelManager : MonoBehaviour
{
    public static TimePanelManager Instance;

    [Header("--- UI Text Elements ---")]
    [Tooltip("Kéo Text hiển thị Giờ vào đây")]
    public TextMeshProUGUI timeText; 
    
    [Tooltip("Kéo Text hiển thị Ngày vào đây")]
    public TextMeshProUGUI dayText;  

    [Header("--- Color Theme (BA Optimized) ---")]
    public Color normalTimeColor = new Color(1f, 0.85f, 0f); 
    public Color warningColor = new Color(1f, 0.5f, 0f);     
    public Color dangerColor = Color.red;                  
    public Color bannedColor = new Color(0.5f, 0.5f, 0.5f);

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        UpdateClockUI();
    }

    /// </summary>
    public void UpdateClockUI()
    {
        if (PlayerStats.Instance == null) return;

        int currentDay = PlayerStats.Instance.currentDay;
        int hour24 = PlayerStats.Instance.currentHour;

        // 1. HIỂN THỊ NGÀY (DAY XX/90)
        if (dayText != null)
        {
            dayText.text = string.Format("DAY {0:D2}/90", currentDay).ToUpper(); 
            
            if (currentDay >= 85) dayText.color = dangerColor;
            else if (currentDay >= 70) dayText.color = warningColor;
            else dayText.color = Color.white;
        }

        // 2. HIỂN THỊ GIỜ (12H FORMAT: AM/PM)
        if (timeText != null)
        {
            string suffix = (hour24 >= 12) ? "PM" : "AM";
            int hour12 = hour24 % 12;
            if (hour12 == 0) hour12 = 12; 

            timeText.text = string.Format("{0:D2}:00 {1}", hour12, suffix);
            
            // LOGIC MÀU SẮC THEO TRẠNG THÁI
            // Nếu đang bị Work Ban hoặc Breakdown, đồng hồ chuyển sang xám/đỏ để cảnh báo
            if (PlayerStats.Instance.isWorkBanned || PlayerStats.Instance.lockWorkHours > 0)
            {
                timeText.color = bannedColor; 
            }
            else 
            {
                timeText.color = normalTimeColor; 
            }
        }
    }

    public void SetTimeTextColor(Color newColor)
    {
        if (timeText != null) timeText.color = newColor;
    }
}
