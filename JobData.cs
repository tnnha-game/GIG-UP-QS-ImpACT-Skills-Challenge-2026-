using UnityEngine;

[CreateAssetMenu(fileName = "NewJob", menuName = "Gameplay/JobData")]
public class JobData : ScriptableObject 
{
    [Header("--- Basic Info ---")]
    public string jobID; 
    public string jobName;      // Tên công việc (VD: Shipper, IT Senior)
    public Sprite jobImage; 
    
    [Header("--- Event & Ending Logic ---")]
    public bool isOnline;       // Tích nếu làm trên mạng (tính tỉ lệ bị Hack)
    public bool isIllegal;      // TÍCH VÀO ĐÂY nếu là việc phạm pháp để ra Ending GREED
    public string jobTier = "Easy"; // Nhập: Easy, Mid, Pro, hoặc Elite

    [Header("Base Stats (Tính trên mỗi 1 giờ làm việc)")]
    public float pay;           // Lương nhận được sau 1h
    public float energyCost;    // Năng lượng mất đi sau 1h
    public float stressGain;    // Căng thẳng tăng thêm sau 1h
    public float skillGain;     // Kỹ năng tăng thêm sau 1h

    // Ẩn duration đi vì mỗi lần Click mặc định là 1 giờ. 
    // Giữ lại biến này để không làm vỡ các script tính toán thời gian khác của bạn.
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