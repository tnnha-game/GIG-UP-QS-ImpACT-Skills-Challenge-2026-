using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement; 

public class JobManager : MonoBehaviour
{
    public static JobManager Instance;

    [Header("--- UI PANELS ---")]
    public GameObject jobPanel;       
    public GameObject confirmPopUp; 

    [Header("--- DATA SOURCE ---")]
    public List<JobData> allJobs;    

    [Header("--- UI FOR HOME (3 CARDS) ---")]
    public JobCardUI[] featuredCards; 

    [Header("--- UI FOR JOB BOARD (SCROLLVIEW) ---")]
    public GameObject jobCardPrefab;  
    public Transform contentParent;   

    private SoundBridge soundBridge;
    private JobData selectedJob; 

    void Awake()
    {
        if (Instance == null) Instance = this;
        // Mặc định ẩn các bảng khi mới vào scene
        if (jobPanel != null) jobPanel.SetActive(false);
        if (confirmPopUp != null) confirmPopUp.SetActive(false);
    }

    void Start()
    {
        soundBridge = Object.FindFirstObjectByType<SoundBridge>();
        // Delay nhẹ để PlayerStats kịp khởi tạo Instance
        Invoke("ShowFeaturedJobs", 0.1f);
    }

    // --- SCENE 1: POPUP CONFIRM ---
    public void OpenConfirmPanel(JobData data)
    {
        if (data == null) return;
        selectedJob = data; 

        GameObject popUp = confirmPopUp;
        if (popUp == null)
        {
            Canvas canvas = Object.FindFirstObjectByType<Canvas>();
            if (canvas != null)
            {
                Transform t = canvas.transform.Find("ConfirmPopUp");
                if (t != null) popUp = t.gameObject;
            }
        }

        if (popUp != null)
        {
            if (soundBridge != null) soundBridge.PlayCommon();
            popUp.SetActive(true);
            popUp.transform.SetAsLastSibling();

            Button btnYes = popUp.transform.Find("Btn_YES")?.GetComponent<Button>();
            if (btnYes != null)
            {
                btnYes.onClick.RemoveAllListeners();
                btnYes.onClick.AddListener(OnConfirmYes); 
            }

            Button btnNo = popUp.transform.Find("Btn_NO")?.GetComponent<Button>();
            if (btnNo != null)
            {
                btnNo.onClick.RemoveAllListeners();
                btnNo.onClick.AddListener(() => popUp.SetActive(false));
            }
        }
        else
        {
            OnConfirmYes(); 
        }
    }

    private void OnConfirmYes()
    {
        if (selectedJob != null && PlayerStats.Instance != null)
        {
            PlayerStats.Instance.selectedJob = selectedJob;
            PlayerStats.Instance.currentJobTier = selectedJob.jobTier;
            PlayerStats.Instance.lastJobName = selectedJob.jobName;

            SceneManager.LoadScene("Scene2");
            Debug.Log("<color=green>[JobManager]</color> Scene 1: Chốt Job " + selectedJob.jobName);
        }
    }

    // --- SCENE 2 ---
    public void QuickApplyJob(JobData data)
    {
        if (data == null || PlayerStats.Instance == null) return;

        PlayerStats.Instance.selectedJob = data;
        PlayerStats.Instance.currentJobTier = data.jobTier;
        PlayerStats.Instance.lastJobName = data.jobName;

        // Vẽ lại bảng Job để làm mờ nút vừa chọn ("Current")
        PopulateJobBoard();

        Debug.Log("<color=cyan>[JobManager]</color> Scene 2: Đã nhảy việc sang " + data.jobName);
    }

    // --- LOGIC WORK (1 CLICK = 1 HOUR) ---
    public void ExecuteJobWork(JobData data)
    {
        if (PlayerStats.Instance == null || data == null) return;
        
        PlayerStats.Instance.currentJobTier = data.jobTier;
        PlayerStats.Instance.lastJobName = data.jobName;

        if (data.isOnline)
        {
            float hackChance = PlayerStats.Instance.hasProLaptop ? 0.02f : 0.15f;
            if (Random.value < hackChance)
            {
                if (EventManager.Instance != null) 
                    EventManager.Instance.TriggerEvent("E66", "Security breach!", 6);
                
                PlayerStats.Instance.cash = Mathf.Max(0, PlayerStats.Instance.cash - 20f); 
                PlayerStats.Instance.PassTime(1); 
                return; 
            }
        }

        PlayerStats.Instance.Work(
            data.pay, 
            data.skillGain, 
            (float)data.energyCost, 
            (float)data.stressGain, 
            1,
            data.jobName,   
            data.isIllegal  
        );
    }

    public void OpenJobBoard()
    {
        if (jobPanel != null)
        {
            if (jobPanel.transform.parent != null && !jobPanel.transform.parent.gameObject.activeSelf)
                jobPanel.transform.parent.gameObject.SetActive(true);
            jobPanel.SetActive(true);
            jobPanel.transform.SetAsLastSibling();
            PopulateJobBoard();
        }
    }

    public void CloseJobBoard() { if (jobPanel != null) jobPanel.SetActive(false); }

    public void ShowFeaturedJobs()
    {
        if (allJobs == null || allJobs.Count == 0) return;
        for (int i = 0; i < featuredCards.Length; i++)
        {
            if (featuredCards[i] != null && i < allJobs.Count)
                featuredCards[i].SetupCard(allJobs[i], CheckJobUnlock(allJobs[i]));
        }
    }

    public void PopulateJobBoard()
    {
        // 1. Kiểm tra an toàn (Safety Check) - Tránh lỗi NullReference
        if (contentParent == null || jobCardPrefab == null || allJobs == null) 
        {
            Debug.LogWarning("<color=yellow>[JobManager]</color> Warning: Thiếu Prefab hoặc ContentParent trong Inspector!");
            return;
        }
    
        // 2. Data Cleaning
        foreach (Transform child in contentParent) 
        { 
            Destroy(child.gameObject); 
        }

        // 3. UX Optimization
        List<JobData> displayList = new List<JobData>(allJobs);
    
        displayList.Sort((a, b) => {
            bool aUnlocked = CheckJobUnlock(a);
            bool bUnlocked = CheckJobUnlock(b);
        
            if (aUnlocked && !bUnlocked) return -1;
            if (!aUnlocked && bUnlocked) return 1;
        
            return GetID(a.jobID).CompareTo(GetID(b.jobID));
        });

        // 4. KHỞI TẠO CARD
        foreach (JobData job in displayList)
        {
            if (job == null || string.IsNullOrEmpty(job.jobID)) continue;
        
            // Lọc ID >= 4 cho bảng phụ ở Scene 2
            if (GetID(job.jobID) >= 4) 
            {
                GameObject newCard = Instantiate(jobCardPrefab, contentParent);
            
                newCard.transform.localScale = Vector3.one;
                newCard.transform.localPosition = Vector3.zero;

                JobCardUI ui = newCard.GetComponent<JobCardUI>();
                if (ui != null) 
                {
                ui.SetupCard(job, CheckJobUnlock(job));
                }
            }
        }

        // 5. CẬP NHẬT GIAO DIỆN 
        Canvas.ForceUpdateCanvases();
        if (contentParent.GetComponent<UnityEngine.UI.ContentSizeFitter>() != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(contentParent.GetComponent<RectTransform>());
        }
    
        Debug.Log("<color=green>[JobManager]</color> Đã nạp lại bảng Job dựa trên Skill của Alex.");
    }

    public bool CheckJobUnlock(JobData job)
    {
        PlayerStats p = PlayerStats.Instance;
        if (p == null) return true;

        if (p.skill < job.reqSkill) return false;
        if (job.reqSmartphone && !p.hasSmartphone) return false;
        if (job.reqEMotorcycle && !p.hasEMotorcycle) return false;
        if (job.reqBasicLaptop && !p.hasBasicLaptop) return false;
        if (job.reqProLaptop && !p.hasProLaptop) return false;
        if (job.reqDegree && !p.hasUniversityDegree) return false;

        if (p.currentDay < job.reqDay) return false;
        
        return true; 
    }

    private int GetID(string id)
    {
        if (string.IsNullOrEmpty(id)) return 0;
        string numericPart = System.Text.RegularExpressions.Regex.Replace(id, "[^0-9]", "");
        int res;
        return int.TryParse(numericPart, out res) ? res : 0;
    }
}
