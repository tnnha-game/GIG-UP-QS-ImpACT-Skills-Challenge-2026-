using UnityEngine;
using System.Collections.Generic;

public class VitalManager : MonoBehaviour
{
    public static VitalManager Instance;

    [Header("--- UI PANELS ---")]
    public GameObject vitalPanel;    

    [Header("--- UI Config ---")]
    public Transform contentContainer; 
    public GameObject vitalPrefab;    

    [Header("--- Data List ---")]
    public List<VitalItemData> allVitalItems; 

    private List<VitalItemUI> spawnedUIItems = new List<VitalItemUI>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        if (vitalPanel != null) vitalPanel.SetActive(false);
    }

    // --- MỞ BẢNG VITAL ---
    public void OpenVitalPanel()
    {
        if (vitalPanel != null)
        {
            // 1. Nếu UI Overlay đang tắt thì bật lên
            if (vitalPanel.transform.parent != null && !vitalPanel.transform.parent.gameObject.activeSelf)
            {
                vitalPanel.transform.parent.gameObject.SetActive(true);
            }

            // 2. Bật bảng Vital và đưa lên trên cùng
            vitalPanel.SetActive(true);
            vitalPanel.transform.SetAsLastSibling();

            // 3. Vẽ hoặc cập nhật danh sách VITAL
            GenerateVitalUI();

            Debug.Log("<color=green>Vital Manager: Đã mở bảng Vital!</color>");
        }
    }

    public void CloseVitalPanel()
    {
        if (vitalPanel != null) vitalPanel.SetActive(false);
    }

    // --- TẠO HOẶC CẬP NHẬT UI ---
    public void GenerateVitalUI()
    {
        if (contentContainer == null || vitalPrefab == null) return;

        if (spawnedUIItems.Count > 0)
        {
            foreach (VitalItemUI ui in spawnedUIItems)
            {
                if (ui != null) ui.RefreshButtonState();
            }
            return;
        }

        foreach (Transform child in contentContainer) {
            Destroy(child.gameObject);
        }
        spawnedUIItems.Clear();

        if (allVitalItems != null)
        {
            foreach (VitalItemData data in allVitalItems)
            {
                if (data == null) continue;
                GameObject item = Instantiate(vitalPrefab, contentContainer);
                item.transform.localScale = Vector3.one; 

                VitalItemUI uiScript = item.GetComponent<VitalItemUI>();
                if (uiScript != null)
                {
                    uiScript.Setup(data);
                    spawnedUIItems.Add(uiScript);
                }
            }
        }
    }

    // --- HÀM XỬ LÝ MUA HÀNG ---
    public void ExecutePurchase(VitalItemData data)
    {
        if (PlayerStats.Instance == null || data == null) return;

        // 1. Kiểm tra điều kiện Bar (W08)
        if (data.id == "W08" && PlayerStats.Instance.currentDay != 40 && PlayerStats.Instance.currentDay != 60)
        {
            Debug.Log("<color=red>Quán Bar đang đóng cửa!</color>");
            return;
        }

        // 2. Kiểm tra tiền mặt 
        if (PlayerStats.Instance.cash < data.price)
        {
            Debug.Log("<color=yellow>Không đủ tiền mặt!</color>");
            return;
        }

        // 3. Thực hiện trừ tiền
        PlayerStats.Instance.cash -= data.price;

        // 4. Phân loại hành động
        if (data.type == VitalType.Housing)
        {
            // MUA NHÀ: Cập nhật ID mới vào PlayerStats
            PlayerStats.Instance.currentHousingID = data.id;

            if (AlexCornerController.Instance != null)
            {
                AlexCornerController.Instance.RefreshUI();
            }

            Debug.Log("<color=cyan>Đã đổi chỗ ở sang: </color>" + data.itemName);
        }
        else
        {
            // MUA ĐỒ ĂN/VẬT PHẨM: Cộng chỉ số trực tiếp (giới hạn 0-100)
            PlayerStats.Instance.energy = Mathf.Clamp(PlayerStats.Instance.energy + data.energyEffect, 0, 100);
            PlayerStats.Instance.stress = Mathf.Clamp(PlayerStats.Instance.stress + data.stressEffect, 0, 100);
            PlayerStats.Instance.health = Mathf.Clamp(PlayerStats.Instance.health + data.healthEffect, 0, 120);
            
            AddGameTime(data.duration);
            Debug.Log("<color=white>Đã sử dụng: </color>" + data.itemName);
        }

        // 5. Cập nhật giao diện toàn hệ thống
        PlayerStats.Instance.UpdateUI();
        if (TimePanelManager.Instance != null) TimePanelManager.Instance.UpdateClockUI();

        // Âm thanh khi mua thành công (Nếu bạn có SoundManager)
        if (SoundManager.Instance != null) SoundManager.Instance.PlayWorkStudyVital();

        // Vẽ lại bảng Vital để khóa nút món đồ vừa mua (nếu là nhà)
        GenerateVitalUI();
    }

    // --- HÀM TRÔI THỜI GIAN ---
    void AddGameTime(float hours)
    {
        if (PlayerStats.Instance == null) return;
        
        PlayerStats.Instance.currentHour += Mathf.RoundToInt(hours);
        
        while (PlayerStats.Instance.currentHour >= 24)
        {
            PlayerStats.Instance.currentHour -= 24;
            PlayerStats.Instance.currentDay++;
        }
    }
}
