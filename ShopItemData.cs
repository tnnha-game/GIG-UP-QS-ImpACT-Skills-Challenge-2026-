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
    public int studyHours;   
    public int skillReward;       
    public float energyCost;   
    public float stressGain;      

    [Header("--- Description ---")]
    [TextArea(3, 10)] 
    public string purpose;    

    [Header("--- Runtime Status ---")]
    public bool isOwned = false;  
}
