using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance;

    [Header("--- 0. UI REFERENCES ---")]
    public TextMeshProUGUI cashText;   
    public TextMeshProUGUI skillText;  
    public TextMeshProUGUI energyText; 
    public TextMeshProUGUI stressText; 
    public TextMeshProUGUI healthText; 
    public TextMeshProUGUI dayText;    
    public Slider cashSlider;
    public Slider energySlider;
    public Slider healthSlider;
    public Slider stressSlider;
    public Slider skillSlider;

    [Header("--- 1. FINANCE ---")]
    public float cash = 100f; 
    public const float HOURLY_FIXED_COST = 0.5f; 

    [Header("--- 2. VITALITY ---")]
    [Range(0, 100)] public float energy = 100f; 
    [Range(0, 100)] public float health = 100f; 
    [Range(0, 100)] public float stress = 0f;   
    
    // Giữ lại để fix lỗi cho VitalItemUI
    public float currentSleepEnergy;
    public float currentSleepStress;
    public float currentSleepHealth; 

    [Header("--- 3. PROGRESSION ---")]
    public float skill = 0; 
    private float skillProgress = 0f; 
    public int currentDay = 1;
    public int currentHour = 8; 

    [Header("--- 4. HOUSING & INVENTORY ---")]
    public string currentHousingID = "H01"; 
    public VitalItemData currentHousingData; 
    public bool hasSmartphone, hasBasicLaptop, hasProLaptop, hasEMotorcycle;

    [Header("--- 5. DEGREES ---")]
    public string currentLearningID = ""; 
    public float currentCertProgress = 0f; 
    public bool hasProfessionalCert, hasTechBootcamp, hasUniversityDegree, hasSpecialistCert;

    [Header("--- 6. STATUS & LOGIC ---")]
    public string currentJobTier = "Easy"; 
    public string lastJobName = "Unemployed"; 
    public bool isTainted = false;           
    public bool gameEnded = false;          
    public int hoursWorkedToday = 0; 
    public int lockWorkHours = 0; 
    public JobData selectedJob;
    // --- LOGIC TỰ ĐỘNG KẾT NỐI UI KHI CHUYỂN SCENE ---
    private void OnEnable() { SceneManager.sceneLoaded += OnSceneLoaded; }
    private void OnDisable() { SceneManager.sceneLoaded -= OnSceneLoaded; }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 1. Tìm tất cả các Canvas
        Canvas[] allCanvases = Object.FindObjectsByType<Canvas>(FindObjectsSortMode.None);

        foreach (Canvas c in allCanvases)
        {
            // 2. Bỏ qua Canvas Endings (Nếu tên chứa chữ Endings)
            if (c.gameObject.name.Contains("Endings")) continue; 

            // 3. Dùng "FindDeepChild" để tìm dù UI nằm sâu trong nhiều lớp Panel
            cashText   = FindDeepChild(c.transform, "CashValueText")?.GetComponent<TextMeshProUGUI>();
            energyText = FindDeepChild(c.transform, "EnergyValueText")?.GetComponent<TextMeshProUGUI>();
            stressText = FindDeepChild(c.transform, "StressValueText")?.GetComponent<TextMeshProUGUI>();
            healthText = FindDeepChild(c.transform, "HealthValueText")?.GetComponent<TextMeshProUGUI>();
            skillText  = FindDeepChild(c.transform, "SkillValueText")?.GetComponent<TextMeshProUGUI>();
            dayText    = FindDeepChild(c.transform, "DayText")?.GetComponent<TextMeshProUGUI>();

            // Tương tự cho Slider
            cashSlider   = FindDeepChild(c.transform, "CashSlider")?.GetComponent<Slider>();
            energySlider = FindDeepChild(c.transform, "EnergySlider")?.GetComponent<Slider>();
            healthSlider = FindDeepChild(c.transform, "HealthSlider")?.GetComponent<Slider>();
            stressSlider = FindDeepChild(c.transform, "StressSlider")?.GetComponent<Slider>();
            skillSlider  = FindDeepChild(c.transform, "SkillSlider")?.GetComponent<Slider>();
        }

        UpdateUI(); // Cập nhật ngay con số lẻ lên màn hình
    }

    // Sửa tên hàm này thành FindDeepChild cho khớp với lệnh gọi ở trên nhé
    Transform FindDeepChild(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name) return child;
            Transform result = FindDeepChild(child, name);
            if (result != null) return result;
        }
        return null;
    }

    [Header("--- 7. EVENT VARIABLES ---")]
    public float wageMultiplier_LowTier = 1.0f;   
    public float wageMultiplier_HighTier = 1.0f;  
    public float incomeMultiplier_MidTier = 1.0f; 
    public float healthAuditMultiplier = 1.0f;    
    public bool isFinalSprintActive = false;      
    public bool isWorkBanned = false;             
    public bool isSessionIntercepted = false;     
    public int algoShiftDaysLeft = 0; 
    public int algoCooldownDays = 0; 

    void Awake()
    {
        if (Instance == null) 
        { 
            Instance = this; 
            DontDestroyOnLoad(gameObject); 
            Time.timeScale = 1f; 
        }
        else 
        { 
            // Nếu đã có Instance rồi mới ở Scene 2 sẽ tự hủy để không ghi đè dữ liệu
            Destroy(gameObject); 
            return; 
        }
    }   

    void Start() { UpdateUI(); }

    public void UpdateUI()
    {
        if (cashText != null) cashText.text = "$" + cash.ToString("F2");
        if (skillText != null) skillText.text = "Skill: " + skill.ToString("F1");
        if (energyText != null) energyText.text = "Energy: " + energy.ToString("F1");
        if (stressText != null) stressText.text = "Stress: " + stress.ToString("F1");
        if (healthText != null) healthText.text = "Health: " + health.ToString("F1");
        if (dayText != null) dayText.text = string.Format("DAY {0:D2}/90", currentDay);

        if (cashSlider != null) { cashSlider.maxValue = 10000; cashSlider.value = cash; }
        if (healthSlider != null) healthSlider.value = health;
        if (stressSlider != null) stressSlider.value = stress;
        if (skillSlider != null) { skillSlider.maxValue = 2000; skillSlider.value = (float)skill; }

        if (TimePanelManager.Instance != null) TimePanelManager.Instance.UpdateClockUI();
        if (PopupGuideManager.Instance != null) PopupGuideManager.Instance.CheckConditions(this);
    }

    public void ApplyStatusRules()
    {
        if (gameEnded) return;
        
        // Điều kiện Burnout/Phá sản
        if (health <= 0 || stress >= 100) { TriggerEnding("Burnout"); return; }
        if (cash <= 0) { TriggerEnding("Bankrupt"); return; }

        // Logic Work Ban: Cấm khi > 90, chỉ mở lại khi đã nghỉ ngơi xuống < 80
        if (stress > 90) 
        {
            isWorkBanned = true;
        }
        else if (isWorkBanned && stress < 80) // Chỉ mở khóa khi stress thực sự giảm sâu
        {
            isWorkBanned = false;
        }
        
        // Cơ chế Ngất xỉu
        if (energy <= 0) { HandlePlayerCollapse(); return; }

        // Kiểm tra Health Audit (Logic gốc của Hà)
        if (health < 20 && healthAuditMultiplier >= 1.0f) {
            if (EventManager.Instance != null)
                EventManager.Instance.TriggerEvent("The Health Audit", "Health critical! Recovery -40%. Fee: $500.", 5);
        }

        energy = Mathf.Clamp(energy, 0, 100);
        stress = Mathf.Clamp(stress, 0, 100);
        health = Mathf.Clamp(health, 0, 100);
    }

    private void HandlePlayerCollapse()
    {
        Debug.Log("<color=red>[Critical]</color> Alex collapsed from exhaustion!");

        health = Mathf.Clamp(health - 20f, 0, 100);
        energy = 0f; // Tỉnh dậy vẫn kiệt sức để ép dùng Vital
        stress = Mathf.Clamp(stress + 10f, 0, 100);
        currentDay++; 
        hoursWorkedToday = 0; 

        if (EventManager.Instance != null)
        {
            EventManager.Instance.TriggerEvent("BLACKOUT", "You fainted! 24 hours passed. Health -20. Buy Vitals to recover!", 99);
        }

        UpdateUI();
        if (health <= 0 || stress >= 100) TriggerEnding("Burnout");
    }

    public void Work(float payH, float skillH, float energyH, float stressH, int hours, string jobName, bool illegal)
    {
        if (gameEnded || lockWorkHours > 0 || isWorkBanned) return; 
        lastJobName = jobName;
        if (illegal) isTainted = true;

        // Logic Cyber Breach (Giữ nguyên của Hà)
        bool isTechJob = (jobName.Contains("Developer") || jobName.Contains("Analyst") || jobName.Contains("Digital"));
        if (isTechJob) {
            float breachRisk = hasProLaptop ? 0.02f : 0.15f; 
            if (Random.value < breachRisk) {
                if (EventManager.Instance != null)
                    EventManager.Instance.TriggerEvent("Cyber Breach", "A critical security flaw was exploited...", 6);
                stress = Mathf.Clamp(stress + 30, 0, 100);
                isSessionIntercepted = true; 
            }
        }

        for (int i = 0; i < hours; i++)
        {
            if (energy <= 0 && energyH < 0) break;
            hoursWorkedToday++;
            
            float hourlyPay = payH;
            if (currentJobTier == "Easy") hourlyPay *= wageMultiplier_LowTier;
            else if (currentJobTier == "Mid") hourlyPay *= incomeMultiplier_MidTier;
            else if (currentJobTier == "Hard") hourlyPay *= wageMultiplier_HighTier;
            
            if (isSessionIntercepted) hourlyPay = 0;
            
            cash += hourlyPay;
            energy += (isFinalSprintActive && energyH < 0) ? (energyH * 3f) : energyH;
            stress += (hoursWorkedToday > 8 && stressH > 0) ? (stressH * 2f) : stressH;
            if (isFinalSprintActive) stress += (30f / hours);

            AddSkillProgress(skillH);
            PassTime(1);
        }
        isSessionIntercepted = false; 
    }

    public void Study(float costH, float skillH, float energyH, float stressH, int hours)
    {
        if (gameEnded) return;
        for (int i = 0; i < hours; i++)
        {
            if (energy <= 0 && energyH < 0) break;
            
            energy += energyH; 
            stress += stressH;
            AddSkillProgress(skillH);

            if (!string.IsNullOrEmpty(currentLearningID))
            {
                float target = (currentLearningID == "ED01") ? 100f : (currentLearningID == "ED02") ? 250f : 500f;
                currentCertProgress += (skillH / target) * 100f;
                if (currentCertProgress >= 100f) CheckAndUnlockDegree();
            }
            PassTime(1);
        }
    }

    private void CheckAndUnlockDegree()
    {
        float skillBonus = 0f; // Biến tạm để tính lượng Skill thưởng

        // Xác định loại bằng và mức thưởng Skill tương ứng
        if (currentLearningID == "ED01") { 
            hasProfessionalCert = true; 
            skillBonus = 100f; 
        }
        else if (currentLearningID == "ED02") { 
            hasTechBootcamp = true; 
            skillBonus = 250f; 
        }
        else if (currentLearningID == "ED03") { 
            hasUniversityDegree = true; 
            skillBonus = 500f; // Bằng Đại học thưởng lớn nhất
        }
        else if (currentLearningID == "ED04") { 
            hasSpecialistCert = true; 
            skillBonus = 200f; 
        }
        
        // 1. CỘNG SKILL THƯỞNG (ROI từ giáo dục)
        skill = Mathf.Clamp(skill + skillBonus, 0, 2000);
        
        Debug.Log($"<color=cyan>[Education]</color> Chúc mừng! Nhận bằng {currentLearningID}. Thưởng ngay: +{skillBonus} Skill!");

        // 2. RESET TRẠNG THÁI (Để khóa nút Study)
        currentLearningID = ""; 
        currentCertProgress = 0f;
        
        // 3. CẬP NHẬT UI TOÀN CỤC
        UpdateUI(); 
    }

    public void RestOneHour()
    {
        if (gameEnded) return;
        UpdateHousingData();
        
        // Hồi phục dựa trên Tier nhà (Logic chia 8 để khớp với 8h ngủ)
        if (currentHousingData != null)
        {
            energy += (currentHousingData.energyEffect / 8f) * healthAuditMultiplier;
            stress += (currentHousingData.stressEffect / 8f); 
            health += (currentHousingData.healthEffect / 8f);
        }
        else 
        { 
            energy += (6.25f * healthAuditMultiplier); 
            stress -= 1.25f; 
        }
        
        PassTime(1);
    }

    public void PassTime(float hours)
    {
        if (gameEnded) return;
        int h = Mathf.RoundToInt(hours);
        for (int i = 0; i < h; i++)
        {
            currentHour++;
            cash -= HOURLY_FIXED_COST; 
            if (lockWorkHours > 0) lockWorkHours--;
            if (stress > 80) health -= (5f / 24f); 
            
            if (currentHour >= 24)
            {
                currentHour = 0; currentDay++;
                hoursWorkedToday = 0; isWorkBanned = false; 
                CheckDailyEvents(currentDay);
                if (currentDay > 90) { EvaluateFinalEnding(); return; }
                UpdateEventTimers();   
            }
        }
        ApplyStatusRules(); UpdateUI();
    }

    private void CheckDailyEvents(int day)
    {
        if (EventManager.Instance == null) return;
        switch (day)
        {
            case 15: EventManager.Instance.TriggerEvent("The Rent Hike", "Rent adjusted. Impact: -$150.", 0); break;
            case 30: 
                EventManager.Instance.TriggerEvent("Automation Shock", "Wage -40% (J01-J05).", 1);
                wageMultiplier_LowTier = 0.6f; break;
            case 75: 
                EventManager.Instance.TriggerEvent("Tech Breakthrough", "Hard Jobs J10-J14 +40% wage!", 3);
                wageMultiplier_HighTier = 1.4f; break;
            case 85: EventManager.Instance.TriggerEvent("The Final Sprint", "Bonus +$3,000.", 4); break;
        }

        if (day >= 31 && day <= 84 && algoShiftDaysLeft <= 0 && algoCooldownDays <= 0) 
        {
            if (Random.value < 0.05f) 
            {
                EventManager.Instance.TriggerEvent("Algorithm Shift", "Online Income -15% (3 days).", 2);
                incomeMultiplier_MidTier = 0.85f; wageMultiplier_HighTier = 0.85f; algoShiftDaysLeft = 3;
            }
        }
    }

    private void UpdateHousingData()
    {
        if (VitalManager.Instance != null && VitalManager.Instance.allVitalItems != null)
        {
            foreach (var item in VitalManager.Instance.allVitalItems)
                if (item.id == currentHousingID) { currentHousingData = item; return; }
        }
    }

    private void AddSkillProgress(float amount)
    {
        skillProgress += amount;
        if (skillProgress >= 1f) { 
            int p = Mathf.FloorToInt(skillProgress); 
            skill = Mathf.Clamp(skill + p, 0, 2000); 
            skillProgress -= p; 
        }
    }

    public void EvaluateFinalEnding()
    {
        if (gameEnded) return;
        if (isTainted) { TriggerEnding("Greed"); return; }
        if (cash >= 5000 && health >= 70 && skill >= 1000) { TriggerEnding("Elite"); return; }
        TriggerEnding("Struggle");
    }

    private void TriggerEnding(string type)
    {
        if (gameEnded) return;
        gameEnded = true; Time.timeScale = 0f; 
        if (EndingManager.Instance != null) EndingManager.Instance.ShowEnding(type);
    }
    
    private void UpdateEventTimers()
    {
        if (algoShiftDaysLeft > 0) 
        {
            algoShiftDaysLeft--;
            if (algoShiftDaysLeft <= 0) 
            { 
                incomeMultiplier_MidTier = 1.0f; wageMultiplier_HighTier = 1.0f; algoCooldownDays = 10; 
            }
        } 
        else if (algoCooldownDays > 0) algoCooldownDays--;
    }

    public void ResetPlayerForNewGame()
    {
        cash = 100f; energy = 100f; health = 100f; stress = 0f; skill = 0; skillProgress = 0f;
        currentDay = 1; currentHour = 8; currentLearningID = ""; currentCertProgress = 0f;
        hasSmartphone = false; hasBasicLaptop = false; hasProLaptop = false; hasEMotorcycle = false;
        hasProfessionalCert = false; hasTechBootcamp = false; hasUniversityDegree = false; hasSpecialistCert = false;
        currentHousingID = "H01"; isTainted = false; gameEnded = false; isWorkBanned = false;
        hoursWorkedToday = 0; lockWorkHours = 0; lastJobName = "Unemployed";
        wageMultiplier_LowTier = 1.0f; wageMultiplier_HighTier = 1.0f; incomeMultiplier_MidTier = 1.0f;
        healthAuditMultiplier = 1.0f; isFinalSprintActive = false; isSessionIntercepted = false;
        algoShiftDaysLeft = 0; algoCooldownDays = 0;
        Time.timeScale = 1f; UpdateUI();
    }
}