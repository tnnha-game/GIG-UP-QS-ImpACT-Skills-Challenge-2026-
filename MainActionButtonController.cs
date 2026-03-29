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
        // Mỗi khung hình, game sẽ tự check: Có đang học không?
        HandleStudyButtonState();
        HandleWorkButtonState();
    }

    private void HandleStudyButtonState()
    {
        if (PlayerStats.Instance == null || studyButton == null) return;

        bool hasCourse = !string.IsNullOrEmpty(PlayerStats.Instance.currentLearningID);
        
        // Dùng CanvasGroup là cách an toàn nhất để làm mờ mà không mất nút
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
    
        // 2. Kiểm tra xem Alex đã chọn việc chưa (để sáng/mờ ngay từ đầu)
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

    // --- NÚT WORK (Bản nâng cấp: Đồng bộ dữ liệu từ Scene 1) ---
    public void WorkAction()
    {
        // 1. Kiểm tra thực thể & Trạng thái Game
        if (PlayerStats.Instance == null || PlayerStats.Instance.gameEnded) return;

        // 2. LẤY DỮ LIỆU JOB ĐÃ CHỌN (Ôm từ Scene 1 sang)
        JobData selected = PlayerStats.Instance.selectedJob;

        // Phòng hờ trường hợp Alex "đi lạc" mà chưa có việc làm
        if (selected == null)
        {
            Debug.Log("<color=yellow>[System]</color> Alex chưa chọn việc! Hãy quay lại trang đầu.");
            return;
        }

        // 3. KIỂM TRA LỆNH CẤM (Stress > 90 hoặc bị khóa giờ)
        if (PlayerStats.Instance.isWorkBanned || PlayerStats.Instance.lockWorkHours > 0)
        {
            Debug.Log("<color=red>[BAN]</color> Alex đang quá stress hoặc trong thời gian phục hồi!");
            return;
        }

        // 4. KIỂM TRA SINH TỒN (Chốt chặn Energy < 10)
        if (PlayerStats.Instance.energy < 10 || PlayerStats.Instance.stress >= 100 || PlayerStats.Instance.health <= 0) 
        {
            PlayerStats.Instance.ApplyStatusRules();
            Debug.Log("<color=red>[Alert]</color> Alex kiệt sức! Hãy nghỉ ngơi hoặc dùng Vital.");
            return;
        }

        // 5. GỌI LỆNH THỰC THI (Lấy thông số trực tiếp từ file JobData)
        // Dòng này sẽ tự động cộng 7.5, 8 hoặc 6 tùy vào Job chọn
        PlayerStats.Instance.Work(
            selected.pay,           // Lương (Ví dụ: 7.5)
            selected.skillGain,      // Skill cộng thêm
            selected.energyCost,    // Năng lượng tốn
            selected.stressGain,    // Stress tăng
            1,                      // Mặc định 1 click = 1 giờ
            selected.jobName,       // Tên công việc thực tế
            selected.isIllegal      // Check việc trái phép
        );

        // 6. PHẢN HỒI ÂM THANH & CẬP NHẬT UI
        if (SoundManager.Instance != null) 
            SoundManager.Instance.PlayWorkStudyVital();
        
        PlayerStats.Instance.UpdateUI();
        
        // Debug nhẹ để kiểm tra tiền nhảy đúng không
        Debug.Log($"<color=green>[Work]</color> Alex làm {selected.jobName} nhận được ${selected.pay}");
    }

    public void StudyAction()
    {
        if (PlayerStats.Instance == null || PlayerStats.Instance.gameEnded) return;

        // 1. KIỂM TRA ĐĂNG KÝ (Cửa chặn ID)
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

        // 3. LẤY DỮ LIỆU ĐỘNG TỪ SHOP (Để đồng nhất với Inspector Hà đã nhập)
        float eCost = -1f; // Giá trị an toàn mặc định
        float sGain = 1f;
        float skillGainPerHour = 0.5f; 

        // Tìm đúng món đồ Alex đang học trong danh sách của Shop để lấy chỉ số thực
        if (ShopManager.Instance != null && ShopManager.Instance.allItems != null)
        {
            foreach (var item in ShopManager.Instance.allItems)
            {
                if (item.itemID == currentID)
                {
                    // Chuyển đổi: Trong Shop Hà nhập số dương (ví dụ: 5), 
                    // nhưng khi học phải trừ nên ta thêm dấu trừ.
                    eCost = -item.energyCost; 
                    sGain = item.stressGain;
                    
                    // Skill cộng thêm mỗi click (trung bình lấy thưởng chia cho số giờ)
                    // Hoặc Hà có thể để một con số cố định như 0.5f nếu muốn
                    skillGainPerHour = (float)item.skillReward / item.studyHours;
                    break;
                }
            }
        }

        // 4. THỰC THI (Gọi sang PlayerStats)
        // costH = 0 vì đã đóng tiền ở Shop rồi.
        PlayerStats.Instance.Study(0f, skillGainPerHour, eCost, sGain, 1);

        // 5. PHẢN HỒI
        if (SoundManager.Instance != null)
            SoundManager.Instance.PlayWorkStudyVital();
        
        PlayerStats.Instance.UpdateUI();

        // Log tiến độ để Hà dễ cân đối game (Game Balancing)
        Debug.Log($"<color=cyan>[Study]</color> {currentID} | Progress: {PlayerStats.Instance.currentCertProgress:F1}% " +
                  $"| Cost: {eCost} Energy | Gain: +{sGain} Stress");
    }

    // --- NÚT REST (1 Click = 1 Giờ nghỉ) ---
    public void RestAction()
    {
        if (PlayerStats.Instance == null || PlayerStats.Instance.gameEnded) return;

        // Nút REST luôn hoạt động kể cả khi energy < 10 
        // Đây là "phao cứu sinh" cuối cùng của Alex
        PlayerStats.Instance.RestOneHour();
        
        if (SoundManager.Instance != null) 
            SoundManager.Instance.PlayWorkStudyVital();

        Debug.Log("<color=green>Alex nghỉ ngơi 1 giờ.</color>");
    }
}