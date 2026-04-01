using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VitalItemUI : MonoBehaviour
{
    [Header("--- UI Elements ---")]
    public Image iconImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI priceText; 
    public TextMeshProUGUI statsText; 
    
    [Header("--- Button Settings ---")]
    public Button purchaseButton; 

    [Header("--- Audio ---")]
    private SoundBridge soundBridge;

    private Image buttonImage;    
    private VitalItemData currentData;

    void Start()
    {
        if (soundBridge == null)
        {
            soundBridge = Object.FindFirstObjectByType<SoundBridge>();
        }
    }

    public void Setup(VitalItemData data)
    {
        currentData = data;
        
        if (purchaseButton != null) 
            buttonImage = purchaseButton.GetComponent<Image>();

        if (nameText != null) 
        {
            nameText.text = data.itemName;
            nameText.color = Color.white;
        }
        
        if (iconImage != null) 
        {
            iconImage.sprite = data.itemIcon;
        }
        
        if (priceText != null) 
        {
            priceText.text = data.price > 0 ? "Price: $" + data.price.ToString("N0") : "Price: FREE";
            priceText.color = Color.white;
        }

        if (statsText != null)
        {
            statsText.color = Color.white;
            
            // Dùng Mathf.Abs để không bị double dấu -- hoặc ++
            string nrg = (data.energyEffect >= 0 ? "+" : "-") + Mathf.Abs(data.energyEffect);
            string str = (data.stressEffect >= 0 ? "+" : "-") + Mathf.Abs(data.stressEffect);
            string hlth = (data.healthEffect >= 0 ? "+" : "-") + Mathf.Abs(data.healthEffect);

            if (data.type == VitalType.Housing)
            {
                statsText.text = "Sleep Reward (8 consecutive hours):\n" +
                                 "Energy: " + nrg + " | Stress: " + str + " | Health: " + hlth;
            }
            else
            {
                statsText.text = "Duration: " + data.duration + " hours\n" +
                                 "Energy: " + nrg + " | Stress: " + str + " | Health: " + hlth;
            }
        }

        RefreshButtonState();

        purchaseButton.onClick.RemoveAllListeners();
        purchaseButton.onClick.AddListener(OnPurchaseButtonClicked);
    }

    private void OnPurchaseButtonClicked()
    {
        if (VitalManager.Instance != null && PlayerStats.Instance != null)
        {
            // Kiểm tra điều kiện Bar (W08) vào đúng ngày 40 và 60
            if (currentData.id == "W08")
            {
                int day = PlayerStats.Instance.currentDay;
                if (day != 40 && day != 60)
                {
                    soundBridge?.PlayNo();
                    return;
                }
            }

            if (PlayerStats.Instance.cash >= currentData.price)
            {
                soundBridge?.PlayPurchase();

                if (currentData.type == VitalType.Housing)
                {
                    // LOGIC NHÀ: Lưu ID và Stats để PlayerStats.RestOneHour xử lý
                    PlayerStats.Instance.cash -= currentData.price;
                    PlayerStats.Instance.currentHousingID = currentData.id;
                    
                    PlayerStats.Instance.currentSleepEnergy = currentData.energyEffect;
                    PlayerStats.Instance.currentSleepStress = currentData.stressEffect;
                    PlayerStats.Instance.currentSleepHealth = currentData.healthEffect;
                }
                else
                {
                    // LOGIC VẬT PHẨM: Dùng dấu += để máy tự tính toán đại số với số âm/dương từ Prefab
                    PlayerStats.Instance.cash -= currentData.price;
                    
                    PlayerStats.Instance.energy += currentData.energyEffect;
                    PlayerStats.Instance.stress += currentData.stressEffect;
                    PlayerStats.Instance.health += currentData.healthEffect;
                    
                    PlayerStats.Instance.PassTime(currentData.duration);
                }

                PlayerStats.Instance.UpdateUI();
                VitalManager.Instance.GenerateVitalUI();
            }
            else
            {
                soundBridge?.PlayNo();
            }
        }
    }

    public void RefreshButtonState()
    {
        if (PlayerStats.Instance == null || purchaseButton == null || buttonImage == null) return;

        purchaseButton.interactable = true;
        buttonImage.color = Color.white;

        if (currentData.id == "W08")
        {
            int day = PlayerStats.Instance.currentDay;
            if (day != 40 && day != 60)
            {
                purchaseButton.interactable = false;
                buttonImage.color = new Color(0.3f, 0.3f, 0.3f, 0.8f);
                if (priceText != null) priceText.text = "CLOSED";
                return;
            }
        }

        if (currentData.type == VitalType.Housing)
        {
            if (PlayerStats.Instance.currentHousingID == currentData.id)
            {
                purchaseButton.interactable = false;
                buttonImage.color = new Color(0.4f, 0.4f, 0.4f, 0.8f); 
                if (priceText != null) priceText.text = "CURRENT";
            }
        }
    }
}
