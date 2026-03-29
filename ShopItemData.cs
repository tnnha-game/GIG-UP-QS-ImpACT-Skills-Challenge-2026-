using UnityEngine;

[CreateAssetMenu(fileName = "New Shop Item", menuName = "Shop/Item Data")]
public class ShopItemData : ScriptableObject
{
    public enum Category { Equipment, Vehicle, Education }

    [Header("--- Basic Info ---")]
    public string itemID;         
    public string itemName;       
    public Category category;     
    public Sprite itemIcon;       
    public int price;             

    [Header("--- For Education Only ---")]
    public int studyHours;        // Đổi sang int cho Clicker (6, 15, 30...)
    public int skillReward;       
    public float energyCost;      // Ví dụ: 1, 2, 3 (Trừ năng lượng mỗi click)
    public float stressGain;      // Ví dụ: 1, 2, 3 (Tăng stress mỗi click)

    [Header("--- Description ---")]
    [TextArea(3, 10)] 
    public string purpose;        // "Mở khóa J12, J13, J14..."

    [Header("--- Runtime Status ---")]
    public bool isOwned = false;  
}