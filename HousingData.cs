using UnityEngine;

[CreateAssetMenu(fileName = "NewHousing", menuName = "Gameplay/HousingData")]
public class HousingData : ScriptableObject 
{
    [Header("Basic Information")]
    public string houseID;         // H01, H02, H03
    public string houseName;       
    
    [Tooltip("The static image containing both Alex and the room")]
    public Sprite fullRoomWithAlex; 

    [Header("Resting Stats (8h Sleep)")]
    public int energyRecovery;     // +50, +75, +100
    public int healthRecovery;     // +2, +10, +20
    public int stressReduction;    // -10, -30, -60

    [Header("Economics")]
    public int rentCost;           // $0, $1500, $4500

    [Header("Details")]
    [TextArea(3, 10)]
    public string description;     // Description about the lifestyle
}