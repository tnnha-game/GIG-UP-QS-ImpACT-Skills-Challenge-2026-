using UnityEngine;

public enum VitalType { Activity, Housing }

[CreateAssetMenu(fileName = "NewVitalItem", menuName = "Gameplay/Vital Item")]
public class VitalItemData : ScriptableObject
{
    public string id; 
    public string itemName;
    public Sprite itemIcon;
    public int price;
    public VitalType type;

    [Header("Cho Activity")]
    public float duration; 
    public int energyEffect;
    public int stressEffect;
    public int healthEffect;

    [Header("Cho Housing")]
    [Tooltip("Kéo file HousingData tương ứng (H01, H02...) vào đây")]
    public HousingData housingData; 

    [TextArea]
    public string gameplayLogic;
}
