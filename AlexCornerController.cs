using UnityEngine;
using UnityEngine.UI;

public class AlexCornerController : MonoBehaviour
{
    public static AlexCornerController Instance;

    [Header("--- UI Elements ---")]
    [SerializeField] private Image displayImage; // Kéo 'Alex_View_Display_Image' vào đây

    [Header("--- 3 Ảnh Riêng Cho Alex Corner ---")]
    public Sprite img_SharedDorm; // Kéo ảnh Dorm (H01) vào đây
    public Sprite img_Studio;     // Kéo ảnh Studio (H02) vào đây
    public Sprite img_Apartment;  // Kéo ảnh Apartment (H03) vào đây

    private VitalItemData currentHouseData; 

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // Vừa vào game tự động hiện ảnh theo nhà đang ở (mặc định H01)
        RefreshUI();
    }

    /// <summary>
    /// Hàm này tự động quét ID từ PlayerStats và chọn đúng 1 trong 3 ảnh xịn
    /// </summary>
    [ContextMenu("Refresh Visual Now")]
    public void RefreshUI()
    {
        if (PlayerStats.Instance == null || displayImage == null) return;

        // 1. Lấy ID nhà hiện tại (H01, H02, hoặc H03)
        string currentID = PlayerStats.Instance.currentHousingID;

        // 2. Quyết định xem sẽ hiện ảnh nào
        Sprite selectedSprite = null;

        switch (currentID)
        {
            case "H01":
                selectedSprite = img_SharedDorm;
                break;
            case "H02":
                selectedSprite = img_Studio;
                break;
            case "H03":
                selectedSprite = img_Apartment;
                break;
            default:
                selectedSprite = img_SharedDorm; // Phòng hờ nếu ID sai
                break;
        }

        // 3. Hiển thị lên khung tranh
        if (selectedSprite != null)
        {
            displayImage.sprite = selectedSprite;
            displayImage.preserveAspect = true;
            Debug.Log($"<color=cyan>Alex Corner: Đã đổi sang ảnh của {currentID}</color>");
        }

        // 4. Đồng bộ dữ liệu chỉ số (để nút Rest lấy đúng thông số hồi phục)
        UpdateDataReference(currentID);
    }

    // Hàm phụ để lấy dữ liệu chỉ số (NRG, STR...) từ VitalManager
    private void UpdateDataReference(string id)
    {
        if (VitalManager.Instance != null && VitalManager.Instance.allVitalItems != null)
        {
            foreach (var item in VitalManager.Instance.allVitalItems)
            {
                if (item.id == id)
                {
                    currentHouseData = item;
                    break;
                }
            }
        }
    }

    // --- HELPER METHODS CHO NÚT REST ---
    public float GetEnergyBonus() => currentHouseData != null ? currentHouseData.energyEffect : 50f;
    public float GetStressBonus() => currentHouseData != null ? currentHouseData.stressEffect : -10f;
    public float GetHealthBonus() => currentHouseData != null ? currentHouseData.healthEffect : 0f;
}