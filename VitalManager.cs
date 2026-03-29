using UnityEngine;
using System.Collections.Generic;

public class VitalManager : MonoBehaviour
{
    public static VitalManager Instance;

    [Header("--- UI PANELS ---")]
    public GameObject vitalPanel;      // Kéo cái Panel bảng Vital vào đây

    [Header("--- UI Config ---")]
    public Transform contentContainer; // Container (Content của ScrollView)
    public GameObject vitalPrefab;     // Prefab của ô vật phẩm

    [Header("--- Data List ---")]
    public List<VitalItemData> allVitalItems; 

    // Danh sách lưu các ô UI đã tạo để tối ưu hiệu suất (Refresh thay vì Destroy)
    private List<VitalItemUI> spawnedUIItems = new List<VitalItemUI>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        // Tắt bảng Vital lúc đầu để không che màn hình
        if (vitalPanel != null) vitalPanel.SetActive(false);
    }

    // --- MỞ BẢNG VITAL ---
    public void OpenVitalPanel()
    {
        if (vitalPanel != null)
        {
            // 1. Nếu cha (UI Overlay) đang tắt thì bật lên
            if (vitalPanel.transform.parent != null && !vitalPanel.transform.parent.gameObject.activeSelf)
            {
                vitalPanel.transform.parent.gameObject.SetActive(true);
            }

            // 2. Bật bảng Vital và đưa lên trên cùng
            vitalPanel.SetActive(true);
            vitalPanel.transform.SetAsLastSibling();

            // 3. Vẽ hoặc cập nhật danh sách đồ ăn/nhà cửa
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

        // Nếu đã sinh ra UI rồi thì chỉ Refresh trạng thái nút bấm (để hiện chữ Owned/Mua)
        if (spawnedUIItems.Count > 0)
        {
            foreach (VitalItemUI ui in spawnedUIItems)
            {
                if (ui != null) ui.RefreshButtonState();
            }
            return;
        }

        // Nếu là lần đầu tiên mở, dọn dẹp và tạo mới các ô
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

        // 1. Kiểm tra điều kiện Quán Bar (W08)
        if (data.id == "W08" && PlayerStats.Instance.currentDay != 40 && PlayerStats.Instance.currentDay != 60)
        {
            Debug.Log("<color=red>Quán Bar đang đóng cửa!</color>");
            return;
        }

        // 2. Kiểm tra tiền mặt (cash)
        if (PlayerStats.Instance.cash < data.price)
        {
            Debug.Log("<color=yellow>Không đủ tiền mặt!</color>");
            return;
        }

        // 3. Thực hiện trừ tiền
        PlayerStats.Instance.cash -= data.price;

        // 4. Phân loại hành động (Nhà ở vs Đồ ăn)
        if (data.type == VitalType.Housing)
        {
            // MUA NHÀ: Cập nhật ID mới vào PlayerStats
            PlayerStats.Instance.currentHousingID = data.id;

            // --- LỆNH TỰ GIÁC: Báo cho Alex Corner đổi sang 1 trong 3 ảnh xịn ---
            if (AlexCornerController.Instance != null)
            {
                AlexCornerController.Instance.RefreshUI();
            }

            Debug.Log("<color=cyan>Đã đổi chỗ ở sang: </color>" + data.itemName);
            // Không trôi thời gian (0h) theo ý bạn
        }
        else
        {
            // MUA ĐỒ ĂN/VẬT PHẨM: Cộng chỉ số trực tiếp (giới hạn 0-100)
            PlayerStats.Instance.energy = Mathf.Clamp(PlayerStats.Instance.energy + data.energyEffect, 0, 100);
            PlayerStats.Instance.stress = Mathf.Clamp(PlayerStats.Instance.stress + data.stressEffect, 0, 100);
            PlayerStats.Instance.health = Mathf.Clamp(PlayerStats.Instance.health + data.healthEffect, 0, 120);
            
            // Trôi thời gian theo thời gian sử dụng món đồ đó
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