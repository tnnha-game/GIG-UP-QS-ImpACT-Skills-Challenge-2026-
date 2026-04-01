using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class JobCardUI : MonoBehaviour
{
    [Header("--- 1. DATA ---")]
    public JobData debugJobData; 

    [Header("--- 2. UI REFERENCES ---")]
    public TextMeshProUGUI titleText;    
    public TextMeshProUGUI statsText;    
    public TextMeshProUGUI requirementText; 
    public Image jobDisplayImage; 

    [Header("--- 3. STATIC BUTTON ---")]
    public GameObject applyBtnObject; 

    [Header("--- 4. LOCK SYSTEM ---")]
    public GameObject lockPanel; 

    private SoundBridge soundBridge;
    private JobData currentJobData;
    private bool currentIsUnlocked;

    void Start()
    {
        if (soundBridge == null)
            soundBridge = Object.FindFirstObjectByType<SoundBridge>();

        // Tự động nạp dữ liệu khi Start
        if (debugJobData != null) 
        {
            SetupCard(debugJobData, true);
        }
    }

    public void SetupCard(JobData data, bool isUnlocked)
    {
        currentJobData = data;
        if (data == null) return;

        // 1. LOGIC CHECK: Kiểm tra trình độ Alex
        bool canActuallyWork = isUnlocked && CheckLogicRequirements(data);
        currentIsUnlocked = canActuallyWork;

        // 2. HIỂN THỊ THÔNG TIN 
        if (titleText != null) titleText.text = data.jobName;
    
        if (statsText != null)
        {
            statsText.text = $"Pay: ${data.pay:F1}/h\n" +
                $"Energy: -{Mathf.Abs(data.energyCost):F1}/h\n" +
                $"Stress: +{Mathf.Abs(data.stressGain):F1}/h\n" +
                $"Skill: +{Mathf.Abs(data.skillGain):F1}/h";
        }

        if (requirementText != null)
        {
            requirementText.text = string.IsNullOrEmpty(data.requirementDescription) ? "Req: None" : "Req: " + data.requirementDescription;
            // Chữ yêu cầu sẽ đỏ lên nếu chưa đủ trình độ
            requirementText.color = canActuallyWork ? Color.white : new Color(1f, 0.4f, 0.4f); 
        }

        if (jobDisplayImage != null && data.jobImage != null) 
            jobDisplayImage.sprite = data.jobImage;

        // 3. LOCK SYSTEM: Hiện màn hình khóa nếu chưa đủ trình độ
        if (lockPanel != null) 
        {
            lockPanel.SetActive(!canActuallyWork);
        }
    
        // 4. XỬ LÝ NÚT BẤM (APPLY BUTTON)
        if (applyBtnObject != null)
        {
            Button targetBtn = applyBtnObject.GetComponent<Button>();
            Image btnImage = applyBtnObject.GetComponent<Image>();

            if (targetBtn != null) 
            {
                // Kiểm tra xem đây có phải công việc hiện tại không
                bool isSelected = (PlayerStats.Instance != null && PlayerStats.Instance.selectedJob == data);

                // Vô hiệu hóa nếu là việc hiện tại hoặc chưa đủ trình độ
                targetBtn.interactable = !isSelected && canActuallyWork;

                if (btnImage != null)
                {
                    // Nếu là việc hiện tại: Làm mờ nút (0.4 alpha)
                    btnImage.color = new Color(1f, 1f, 1f, isSelected ? 0.4f : 1f); 
                }

                // Gán sự kiện click
                targetBtn.onClick.RemoveAllListeners();
                targetBtn.onClick.AddListener(OnCardClick);
            }
        }
    }

    private bool CheckLogicRequirements(JobData data)
    {
        PlayerStats p = PlayerStats.Instance;
        if (p == null) return true;
    
        // 1. Check Skill (Dùng đúng biến reqSkill trong JobData.cs)
        if (p.skill < data.reqSkill) return false; 

        // 2. Check Ngày mở Job
        if (p.currentDay < data.reqDay) return false;

        // 3. Check Đồ đạc
        if (data.reqSmartphone && !p.hasSmartphone) return false;
        if (data.reqEMotorcycle && !p.hasEMotorcycle) return false;
        if (data.reqBasicLaptop && !p.hasBasicLaptop) return false;
        if (data.reqProLaptop && !p.hasProLaptop) return false;

        // 4. Check Bằng cấp
        if (data.reqBootcamp && !p.hasTechBootcamp) return false;
        if (data.reqDegree && !p.hasUniversityDegree) return false;
        if (data.reqCertificate && !p.hasProfessionalCert) return false;
    
        return true;
    }

    public void OnCardClick()
    {
        if (currentJobData == null) return;

        if (soundBridge != null) soundBridge.PlayCommon(); 

        string currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        if (currentSceneName == "Scene1")
        {
            // --- SCENE 1: Gọi JobManager mở Popup xác nhận ---
            if (JobManager.Instance != null)
            {
                JobManager.Instance.OpenConfirmPanel(currentJobData);
            }
            else
            {
                // Backup nếu Instance null
                JobManager jm = Object.FindFirstObjectByType<JobManager>();
                if (jm != null) jm.OpenConfirmPanel(currentJobData);
            }
        }
        else
        {
            // --- SCENE 2 ---
            Debug.Log("<color=green>[Job System]</color> Scene 2: Đã đổi sang " + currentJobData.jobName);

            if (PlayerStats.Instance != null)
            {
                PlayerStats.Instance.selectedJob = currentJobData;
                PlayerStats.Instance.currentJobTier = currentJobData.jobTier;
                PlayerStats.Instance.lastJobName = currentJobData.jobName;
            }

            // Vẽ lại bảng Job để nút vừa bấm tự mờ đi ngay lập tức
            if (JobManager.Instance != null)
            {
                JobManager.Instance.PopulateJobBoard();
            }
            else
            {
                JobManager jm = Object.FindFirstObjectByType<JobManager>();
                if (jm != null) jm.PopulateJobBoard();
            }
        }
    }
}
