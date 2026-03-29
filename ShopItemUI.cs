using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItemUI : MonoBehaviour
{
    private ShopItemData itemData;

    [Header("--- UI References ---")]
    public Image iconImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI priceText;
    public TextMeshProUGUI purposeText;
    
    [Header("--- Education Info ---")]
    public GameObject studyInfoParent; 
    public TextMeshProUGUI studyHoursValueText;  
    public TextMeshProUGUI skillRewardValueText;
    public TextMeshProUGUI energyValueText; 
    public TextMeshProUGUI stressValueText;

    [Header("--- Button Config ---")]
    public Button actionButton;
    public Sprite purchaseButtonSprite; 
    public Sprite studyButtonSprite;

    private SoundBridge soundBridge;

    void Start()
    {
        if (soundBridge == null)
            soundBridge = Object.FindFirstObjectByType<SoundBridge>();
    }

    public void Setup(ShopItemData data)
    {
        itemData = data;
        FillUI();
    }

    public void FillUI()
    {
        if (itemData == null || PlayerStats.Instance == null) return;

        // Kiểm tra sở hữu và khả năng chi trả
        bool isOwned = CheckActualOwnership(itemData.itemID);
        bool canAfford = PlayerStats.Instance.cash >= itemData.price;

        // --- 1. THÔNG TIN CƠ BẢN (Luôn luôn hiện) ---
        if (nameText != null) nameText.text = itemData.itemName;
        if (purposeText != null) purposeText.text = itemData.purpose;
        if (iconImage != null) iconImage.sprite = itemData.itemIcon;

        // --- 2. CHỈ SỐ STATS (Energy/Stress) - HIỆN RÕ NHÃN (LABEL) ---
        if (energyValueText != null)
        {
            // Ghi rõ nhãn "Energy:" để không bị nhầm với các con số khác
            energyValueText.text = "Energy: -" + itemData.energyCost.ToString();
            energyValueText.gameObject.SetActive(true); 
        }

        if (stressValueText != null)
        {
            // Ghi rõ nhãn "Stress:"
            stressValueText.text = "Stress: +" + itemData.stressGain.ToString();
            stressValueText.gameObject.SetActive(true);
        }

        // --- 3. LOGIC HIỆN/ẨN THEO CATEGORY ---
        if (itemData.category == ShopItemData.Category.Education)
        {
            // Nếu là bằng cấp: Hiện bảng chứa Hours và Skill Reward
            if (studyInfoParent != null) studyInfoParent.SetActive(true);
            
            if (studyHoursValueText != null) 
                studyHoursValueText.text = "Time: " + itemData.studyHours.ToString() + " Hours";
            
            if (skillRewardValueText != null)
                skillRewardValueText.text = "Reward: +" + itemData.skillReward.ToString() + " Skill";
        }
        else
        {
            // Nếu là Đồ vật: Tắt bảng Hours/Skill để thẻ Smartphone trông gọn hơn
            if (studyInfoParent != null) studyInfoParent.SetActive(false);
        }

        // --- 4. XỬ LÝ TRẠNG THÁI NÚT BẤM VÀ GIÁ TIỀN ---
        if (isOwned)
        {
            // Đã sở hữu hoặc đã tốt nghiệp
            if (priceText != null) 
            {
                priceText.text = (itemData.category == ShopItemData.Category.Education) ? "GRADUATED" : "OWNED";
                priceText.color = Color.gray;
            }
            
            actionButton.interactable = false; 
            if (actionButton.image != null)
                actionButton.image.color = new Color(0.4f, 0.4f, 0.4f, 0.8f); 
        }
        else
        {
            // Kiểm tra xem Alex có đang trong quá trình học môn này không
            bool isLearning = (PlayerStats.Instance.currentLearningID == itemData.itemID);

            if (priceText != null) 
            {
                if (isLearning) {
                    priceText.text = "LEARNING...";
                    priceText.color = Color.yellow;
                }
                else {
                    // Hiện chữ Price để rõ ràng hơn
                    priceText.text = "Price: $" + itemData.price.ToString("N0");
                    // Đổi màu đỏ nếu không đủ tiền - Business Analytics: Cảnh báo ngân sách!
                    priceText.color = canAfford ? Color.white : Color.red;
                }
            }
            
            // Nút bấm chỉ bật khi không học và đủ tiền
            actionButton.interactable = !isLearning && canAfford;
            
            if (actionButton.image != null)
            {
                // Thay đổi Sprite nút: Nút "Học" cho bằng cấp, nút "Mua" cho đồ vật
                actionButton.image.sprite = (itemData.category == ShopItemData.Category.Education) ? studyButtonSprite : purchaseButtonSprite;
                
                // Hiệu ứng màu sắc trực quan
                if (isLearning) 
                    actionButton.image.color = new Color(0.5f, 1f, 0.5f); // Màu xanh lá nhẹ
                else if (!canAfford) 
                    actionButton.image.color = new Color(1f, 0.5f, 0.5f); // Màu đỏ nhẹ
                else 
                    actionButton.image.color = Color.white;
            }
        }
    }

    public void OnClick()
    {
        if (itemData == null || PlayerStats.Instance == null) return;

        if (itemData.category == ShopItemData.Category.Education)
        {
            if (PlayerStats.Instance.cash < itemData.price)
            {
                soundBridge?.PlayNo();
                return;
            }

            // Tạo JobData tạm thời để hệ thống Work/Study nhận diện
            JobData studyJob = ScriptableObject.CreateInstance<JobData>();
            studyJob.jobName = itemData.itemName;
            studyJob.pay = 0;
            
            // Lấy trực tiếp từ itemData để đồng nhất dữ liệu
            studyJob.energyCost = -itemData.energyCost; 
            studyJob.stressGain = itemData.stressGain;  
            studyJob.skillGain = (float)itemData.skillReward / itemData.studyHours;

            PlayerStats.Instance.selectedJob = studyJob;
            PlayerStats.Instance.currentLearningID = itemData.itemID;
            PlayerStats.Instance.currentCertProgress = 0f;

            soundBridge?.PlayApplyJob(); 
        }
        else
        {
            if (PlayerStats.Instance.cash >= itemData.price)
            {
                PlayerStats.Instance.cash -= itemData.price;
                SetOwnership(itemData.itemID);
                soundBridge?.PlayPurchase();
            }
            else
            {
                soundBridge?.PlayNo();
            }
        }

        PlayerStats.Instance.UpdateUI();
        if (ShopManager.Instance != null) ShopManager.Instance.RefreshAllUI();
    }

    private bool CheckActualOwnership(string id)
    {
        if (PlayerStats.Instance == null || string.IsNullOrEmpty(id)) return false;
        return id switch
        {
            "EQ01" => PlayerStats.Instance.hasSmartphone,
            "EQ02" => PlayerStats.Instance.hasBasicLaptop,
            "EQ03" => PlayerStats.Instance.hasProLaptop,
            "V01" => PlayerStats.Instance.hasEMotorcycle,
            "ED01" => PlayerStats.Instance.hasProfessionalCert,
            "ED02" => PlayerStats.Instance.hasTechBootcamp,
            "ED03" => PlayerStats.Instance.hasUniversityDegree,
            "ED04" => PlayerStats.Instance.hasSpecialistCert,
            _ => false
        };
    }

    private void SetOwnership(string id)
    {
        if (PlayerStats.Instance == null) return;
        switch (id)
        {
            case "EQ01": PlayerStats.Instance.hasSmartphone = true; break;
            case "EQ02": PlayerStats.Instance.hasBasicLaptop = true; break;
            case "EQ03": PlayerStats.Instance.hasProLaptop = true; break;
            case "V01": PlayerStats.Instance.hasEMotorcycle = true; break;
            case "ED01": PlayerStats.Instance.hasProfessionalCert = true; break;
            case "ED02": PlayerStats.Instance.hasTechBootcamp = true; break;
            case "ED03": PlayerStats.Instance.hasUniversityDegree = true; break;
            case "ED04": PlayerStats.Instance.hasSpecialistCert = true; break;
        }
    }
}