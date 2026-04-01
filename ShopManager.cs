using UnityEngine;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance;

    [Header("--- Setup Shop UI ---")]
    public GameObject shopPanel;        
    public GameObject itemPrefab;       
    public Transform contentParent;    
    public List<ShopItemData> allItems; 

    [Header("--- Systems Connection ---")]
    public PlayerStats stats;          
    public TimePanelManager time;      

    private bool isInitialized = false;

    private void Awake() 
    {
        if (Instance == null) Instance = this;
        if (shopPanel != null) shopPanel.SetActive(false);
    }

    private void Start()
    {
        InitializeShopData();
    }

    private void InitializeShopData()
    {
        if (isInitialized) return;

        if (allItems != null)
        {
            foreach(var item in allItems) 
            {
                if (item != null) item.isOwned = false;
            }
        }

        GenerateShopItems();
        isInitialized = true;
    }

    public void OpenShop()
    {
        if (!isInitialized) InitializeShopData();
        
        if (shopPanel != null) 
        {
            if (shopPanel.transform.parent != null)
                shopPanel.transform.parent.gameObject.SetActive(true);

            shopPanel.SetActive(true); 
            shopPanel.transform.SetAsLastSibling(); 

            RefreshAllUI(); 
        }
    }

    public void CloseShop()
    {
        if (shopPanel != null) shopPanel.SetActive(false);
    }

    private void GenerateShopItems()
    {
        if (contentParent == null || itemPrefab == null || allItems == null) return;

        foreach (Transform child in contentParent) Destroy(child.gameObject);

        foreach (var data in allItems)
        {
            if (data == null) continue;
            GameObject card = Instantiate(itemPrefab, contentParent);
            card.transform.localScale = Vector3.one; 

            ShopItemUI uiScript = card.GetComponent<ShopItemUI>();
            if (uiScript != null) uiScript.Setup(data);
        }
    }

    public void ProcessPurchase(ShopItemData item)
    {
        if (item == null || stats == null) return;

        // 1. Kiểm tra tài chính
        if (stats.cash < item.price)
        {
            Debug.Log("<color=red>[Shop] Không đủ tiền!</color>");
            return;
        }

        // 2. Xử lý theo loại item
        if (item.category == ShopItemData.Category.Education)
        {
            if (!string.IsNullOrEmpty(stats.currentLearningID))
            {
                Debug.Log("<color=yellow>[Shop] Đang có khóa học chưa hoàn thành!</color>");
                return;
            }

            if (CheckIfDegreeOwned(item.itemID))
            {
                Debug.Log("<color=yellow>[Shop] Đã có bằng cấp này</color>");
                return;
            }

            // GIAO DỊCH EDUCATION
            stats.cash -= item.price;
            stats.currentLearningID = item.itemID;
            stats.currentCertProgress = 0f;

            JobData studyJob = ScriptableObject.CreateInstance<JobData>();
            studyJob.jobName = item.itemName;
            studyJob.pay = 0;
            studyJob.energyCost = -item.energyCost; // Tốn năng lượng (âm)
            studyJob.stressGain = item.stressGain;
            studyJob.skillGain = (float)item.skillReward / item.studyHours;

            stats.selectedJob = studyJob;
            
            Debug.Log("<color=green>[Shop] Successfully registerted: </color>" + item.itemName);
        }
        else
        {
            // EQUIPMENT / VEHICLE
            if (CheckActualInventory(item.itemID)) // Check từ PlayerStats thay vì isOwned
            {
                Debug.Log("<color=yellow>[Shop] Owned</color>");
                return;
            }

            stats.cash -= item.price;
            item.isOwned = true;
            UpdatePlayerInventory(item.itemID);
            
            Debug.Log("<color=green>[Shop] Successfully purchased: </color>" + item.itemName);
        }

        // 3. Cập nhật toàn hệ thống
        RefreshAllUI(); 

        if (JobManager.Instance != null) 
            JobManager.Instance.PopulateJobBoard();
    }

    private bool CheckIfDegreeOwned(string id)
    {
        return id switch {
            "ED01" => stats.hasProfessionalCert,
            "ED02" => stats.hasTechBootcamp,
            "ED03" => stats.hasUniversityDegree,
            "ED04" => stats.hasSpecialistCert,
            _ => false
        };
    }

    private bool CheckActualInventory(string id)
    {
        return id switch {
            "EQ01" => stats.hasSmartphone,
            "EQ02" => stats.hasBasicLaptop,
            "EQ03" => stats.hasProLaptop,
            "V01"  => stats.hasEMotorcycle,
            _ => false
        };
    }

    private void UpdatePlayerInventory(string id)
    {
        switch (id)
        {
            case "EQ01": stats.hasSmartphone = true; break;
            case "EQ02": stats.hasBasicLaptop = true; break;
            case "EQ03": stats.hasProLaptop = true; break;
            case "V01":  stats.hasEMotorcycle = true; break;
        }
    }

    public void RefreshAllUI()
    {
        if (stats != null) stats.UpdateUI(); 
        if (time != null) time.UpdateClockUI(); 
        
        ShopItemUI[] uiCards = contentParent.GetComponentsInChildren<ShopItemUI>();
        foreach (var card in uiCards) card.FillUI();
    }
}
