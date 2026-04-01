using UnityEngine;

[CreateAssetMenu(fileName = "NewJob", menuName = "Gameplay/JobData")]
public class JobData : ScriptableObject 
{
    [Header("--- Basic Info ---")]
    public string jobID; 
    public string jobName;     
    public Sprite jobImage; 
    
    [Header("--- Event & Ending Logic ---")]
    public bool isOnline;      
    public bool isIllegal;     
    public string jobTier = "Easy";

    [Header("Base Stats (Tính trên mỗi 1 giờ làm việc)")]
    public float pay;          
    public float energyCost;   
    public float stressGain;  
    public float skillGain;    

    // Ẩn duration đi vì mỗi lần Click mặc định là 1 giờ. 
    [HideInInspector] 
    public int duration = 1; 

    [Header("Display Requirement (Mô tả hiện lên UI)")]
    [TextArea(2, 3)]
    public string requirementDescription; 

    [Header("Logic Unlock Requirements (Điều kiện mở khóa)")]
    public int reqSkill;
    public int reqDay;
    public bool reqSmartphone;
    public bool reqEMotorcycle;
    public bool reqBasicLaptop;
    public bool reqProLaptop;
    public bool reqBootcamp;    
    public bool reqDegree;      
    public bool reqCertificate; 
}
