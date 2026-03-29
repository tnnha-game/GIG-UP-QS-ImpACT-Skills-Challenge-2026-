using UnityEngine;

public class SoundBridge : MonoBehaviour
{
    // --- 1. CÁC HÀM CƠ BẢN ---
    public void PlayCommon() => SoundManager.Instance?.PlayCommonClick();
    public void PlayYes() => SoundManager.Instance?.PlayYesClick();
    public void PlayNo() => SoundManager.Instance?.PlayNoClick();
    public void PlayToggle() => SoundManager.Instance?.PlayToggleSound();

    // --- 2. CÁC HÀM GAMEPLAY (PHÂN LOẠI RÕ RÀNG) ---

    // [Dùng cho: Bảng Shop (Smartphone, Laptop, Xe) và Bảng Vital (Mua đồ ăn/hồi máu)]
    // Sẽ phát file 'purchaseSound' trong SoundManager
    public void PlayPurchase() => SoundManager.Instance?.PlayPurchase();

    // [Dùng cho: Bảng Jobs (Nút Apply Job)]
    // Sẽ phát file 'applyStudyClip' (hoặc selectJob) trong SoundManager
    public void PlayApplyJob() => SoundManager.Instance?.PlayApplyStudy();

    // [Dùng cho: Bảng Shop (Nút Study/Học tập)]
    // Sẽ phát file 'applyStudyClip' (hoặc selectJob) trong SoundManager
    public void PlayStudy() => SoundManager.Instance?.PlayApplyStudy();

    // Dùng cho các nút bấm nhỏ (Làm việc, hồi phục)
    public void PlayWorkStudy() => SoundManager.Instance?.PlayWorkStudyVital();
    
    // Dùng cho 3 nút CHUYỂN BẢNG to: Jobs, Shop, Vital
    public void PlayJobsShopVital() => SoundManager.Instance?.PlayJobsShopVital();
    
    // Hàm cũ giữ lại để không lỗi các script khác gọi tên này
    public void PlaySelectJob() => SoundManager.Instance?.PlayApplyStudy();

    // --- 3. CÁC HÀM SỰ KIỆN (Event) ---
    public void PlayEventGood() => SoundManager.Instance?.PlayEventGood();
    public void PlayEventBad() => SoundManager.Instance?.PlayEventBad();
    public void PlayEventWarning() => SoundManager.Instance?.PlayEventWarning();

    // HÀM BỔ SUNG: Để EventManager gọi bằng string (Good, Bad, Warning)
    public void PlayEventSFX(string type)
    {
        switch (type.ToLower())
        {
            case "good": PlayEventGood(); break;
            case "bad": PlayEventBad(); break;
            case "warning": PlayEventWarning(); break;
        }
    }

    // --- 4. CÁC HÀM KẾT THÚC (Ending) ---
    public void PlayEndingElite() => SoundManager.Instance?.PlayEndingStinger("Elite");
    public void PlayEndingStable() => SoundManager.Instance?.PlayEndingStinger("Stable");
    public void PlayEndingOther() => SoundManager.Instance?.PlayEndingStinger("Other");
}