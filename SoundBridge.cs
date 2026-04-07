using UnityEngine;

public class SoundBridge : MonoBehaviour
{
    // --- 1. CÁC HÀM CƠ BẢN ---
    public void PlayCommon() => SoundManager.Instance?.PlayCommonClick();
    public void PlayYes() => SoundManager.Instance?.PlayYesClick();
    public void PlayNo() => SoundManager.Instance?.PlayNoClick();
    public void PlayToggle() => SoundManager.Instance?.PlayToggleSound();

    // --- 2. CÁC HÀM GAMEPLAY ---

    // [Dùng cho: Bảng Shop (Smartphone, Laptop, Xe) và Bảng Vital]
    // Sẽ phát file 'purchaseSound' trong SoundManager
    public void PlayPurchase() => SoundManager.Instance?.PlayPurchase();

    // [Dùng cho: Bảng Jobs (Nút Apply Job)]
    // Sẽ phát file 'applyStudyClip' (hoặc selectJob) trong SoundManager
    public void PlayApplyJob() => SoundManager.Instance?.PlayApplyStudy();

    // [Dùng cho: Bảng Shop (Nút Study)]
    // Sẽ phát file 'applyStudyClip' (hoặc selectJob) trong SoundManager
    public void PlayStudy() => SoundManager.Instance?.PlayApplyStudy();

    // Dùng cho các nút bấm nhỏ (Làm việc, hồi phục)
    public void PlayWorkStudy() => SoundManager.Instance?.PlayWorkStudyVital();
    
    // Dùng cho 3 nút CHUYỂN BẢNG: Jobs, Shop, Vital
    public void PlayJobsShopVital() => SoundManager.Instance?.PlayJobsShopVital();
    
    public void PlaySelectJob() => SoundManager.Instance?.PlayApplyStudy();

    // --- 3. CÁC HÀM EVENTS ---
    public void PlayEventGood() => SoundManager.Instance?.PlayEventGood();
    public void PlayEventBad() => SoundManager.Instance?.PlayEventBad();
    public void PlayEventWarning() => SoundManager.Instance?.PlayEventWarning();

    public void PlayEventSFX(string type)
    {
        switch (type.ToLower())
        {
            case "good": PlayEventGood(); break;
            case "bad": PlayEventBad(); break;
            case "warning": PlayEventWarning(); break;
        }
    }

    // --- 4. CÁC HÀM ENDINGS ---
    public void PlayEndingElite() => SoundManager.Instance?.PlayEndingStinger("Elite");
    public void PlayEndingStable() => SoundManager.Instance?.PlayEndingStinger("Stable");
    public void PlayEndingOther() => SoundManager.Instance?.PlayEndingStinger("Other");
}
