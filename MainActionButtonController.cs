using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MainActionButtonController : MonoBehaviour
{
    [Header("--- Audio Settings ---")]
    public AudioSource actionAudioSource; 
    public Button studyButton;
    public Button workButton;

    void Update()
    {
        HandleStudyButtonState();
        HandleWorkButtonState();
    }

    private void HandleStudyButtonState()
    {
        if (PlayerStats.Instance == null || studyButton == null) return;

        bool hasCourse = !string.IsNullOrEmpty(PlayerStats.Instance.currentLearningID);
        
        CanvasGroup cg = studyButton.GetComponent<CanvasGroup>();
        if (cg == null) cg = studyButton.gameObject.AddComponent<CanvasGroup>();

        if (hasCourse)
        {
            studyButton.interactable = true;
            cg.alpha = 1f; // Hiện rõ mồn một
        }
        else
        {
            studyButton.interactable = false; 
            cg.alpha = 0.3f; // Hiện mờ mờ 30% (vẫn giữ chỗ, không bị mất)
        }
    }

    private void HandleWorkButtonState()
    {
        if (PlayerStats.Instance == null || workButton == null) return;

        // 1. Kiểm tra trạng thái: Bị Ban (Stress > 90) hoặc đang bị khóa giờ phục hồi
        bool isBanned = PlayerStats.Instance.isWorkBanned || PlayerStats.Instance.lockWorkHours > 0;
    
        // 2. Kiểm tra xem Alex đã chọn việc chưa
        bool hasJob = PlayerStats.Instance.selectedJob != null;

        CanvasGroup cg = workButton.GetComponent<CanvasGroup>();
        if (cg == null) cg = workButton.gameObject.AddComponent<CanvasGroup>();

        // LOGIC: Nút chỉ SÁNG (Alpha 1) khi CÓ việc VÀ KHÔNG bị Ban
        if (hasJob && !isBanned)
        {
            workButton.interactable = true;
            cg.alpha = 1f;
        }
        else
        {
            workButton.interactable = false;
            cg.alpha = 0.3f; // Mờ tịt đúng ý Hà
        }
    }

    // --- NÚT WORK ---
    public void WorkAction()
    {
        // 1. Kiểm tra thực thể & Trạng thái Game
        if (PlayerStats.Instance == null || PlayerStats.Instance.gameEnded) return;

        // 2. LẤY DỮ LIỆU JOB ĐÃ CHỌN 
        JobData selected = PlayerStats.Instance.selectedJob;

        if (selected == null)
        {
            Debug.Log("<color=yellow>[System]</color> Alex chưa chọn việc! Hãy quay lại trang đầu.");
            return;
        }

        // 3. KIỂM TRA LỆNH BAN (Stress > 90 hoặc bị khóa giờ)
        if (PlayerStats.Instance.isWorkBanned || PlayerStats.Instance.lockWorkHours > 0)
        {
            Debug.Log("<color=red>[BAN]</color> Alex đang quá stress hoặc trong thời gian phục hồi!");
            return;
        }

        // 4. KIỂM TRA SINH TỒN (Energy < 10)
        if (PlayerStats.Instance.energy < 10 || PlayerStats.Instance.stress >= 100 || PlayerStats.Instance.health <= 0) 
        {
            PlayerStats.Instance.ApplyStatusRules();
            Debug.Log("<color=red>[Alert]</color> Alex exhausted! Rest or use Vital.");
            return;
        }

        // 5. GỌI LỆNH
        PlayerStats.Instance.Work(
            selected.pay,          
            selected.skillGain,     
            selected.energyCost,   
            selected.stressGain,    
            1,                      
            selected.jobName,      
            selected.isIllegal     
        );

        // 6. PHẢN HỒI ÂM THANH & CẬP NHẬT UI
        if (SoundManager.Instance != null) 
            SoundManager.Instance.PlayWorkStudyVital();
        
        PlayerStats.Instance.UpdateUI();
        
        Debug.Log($"<color=green>[Work]</color> Alex làm {selected.jobName} nhận được ${selected.pay}");
    }

    public void StudyAction()
    {
        if (PlayerStats.Instance == null || PlayerStats.Instance.gameEnded) return;

        // 1. KIỂM TRA ĐĂNG KÝ 
        string currentID = PlayerStats.Instance.currentLearningID;
        if (string.IsNullOrEmpty(currentID))
        {
            Debug.Log("<color=orange>[Study] Alex chưa đăng ký khóa học nào! Hãy vào Shop ngay.</color>");
            return;
        }

        // 2. KIỂM TRA SINH TỒN
        if (PlayerStats.Instance.energy < 10 || PlayerStats.Instance.stress >= 100)
        {
            if (PlayerStats.Instance.energy < 10) PlayerStats.Instance.ApplyStatusRules();
            Debug.Log("<color=red>[Alert]</color> Alex quá mệt để học tiếp!");
            return;
        }

        // 3. LẤY DỮ LIỆU ĐỘNG TỪ SHOP
        float eCost = -1f; 
        float sGain = 1f;
        float skillGainPerHour = 0.5f; 

        if (ShopManager.Instance != null && ShopManager.Instance.allItems != null)
        {
            foreach (var item in ShopManager.Instance.allItems)
            {
                if (item.itemID == currentID)
                {

                    eCost = -item.energyCost; 
                    sGain = item.stressGain;
                    
                    skillGainPerHour = (float)item.skillReward / item.studyHours;
                    break;
                }
            }
        }

        // 4. THỰC THI LỆNH
        PlayerStats.Instance.Study(0f, skillGainPerHour, eCost, sGain, 1);

        // 5. PHẢN HỒI
        if (SoundManager.Instance != null)
            SoundManager.Instance.PlayWorkStudyVital();
        
        PlayerStats.Instance.UpdateUI();

        Debug.Log($"<color=cyan>[Study]</color> {currentID} | Progress: {PlayerStats.Instance.currentCertProgress:F1}% " +
                  $"| Cost: {eCost} Energy | Gain: +{sGain} Stress");
    }

    // --- NÚT REST (1 Click = 1 Giờ nghỉ) ---
    public void RestAction()
    {
        if (PlayerStats.Instance == null || PlayerStats.Instance.gameEnded) return;

        PlayerStats.Instance.RestOneHour();
        
        if (SoundManager.Instance != null) 
            SoundManager.Instance.PlayWorkStudyVital();

        Debug.Log("<color=green>Alex nghỉ ngơi 1 giờ.</color>");
    }
}
