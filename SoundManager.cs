using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("--- 1. AUDIO SOURCES ---")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("--- 2. BASIC UI CLIPS ---")]
    public AudioClip commonClick;
    public AudioClip yesClick;
    public AudioClip noClick;
    public AudioClip toggleSound; 

    [Header("--- 3. GAMEPLAY CLIPS ---")]
    public AudioClip selectJob;      // Giữ lại để tránh lỗi các script cũ
    public AudioClip workStudyVital; 
    public AudioClip jobsShopVital;  
    public AudioClip purchaseSound;  
    
    [Tooltip("ÂM THANH RIÊNG CHO APPLY JOB VÀ STUDY SHOP")]
    public AudioClip applyStudyClip; 

    [Header("--- 4. EVENT CLIPS ---")]
    public AudioClip eventGood;
    public AudioClip eventBad;
    public AudioClip eventWarning;

    [Header("--- 5. ENDING CLIPS ---")]
    public AudioClip endingElite;    
    public AudioClip endingStable;   
    public AudioClip endingOther;    
    public AudioClip endingDefault;  

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            if (transform.parent == null) DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // --- HÀM DÀNH CHO SHOP & VITAL (PURCHASE) ---
    public void PlayPurchaseSound() 
    {
        if (purchaseSound != null)
        {
            PlaySFX(purchaseSound);
            Debug.Log("<color=yellow>Sound: Purchase Success!</color>");
        }
    }

    // --- HÀM DÀNH CHO APPLY JOB & STUDY (RIÊNG BIỆT) ---
    public void PlayApplyStudy() 
    {
        // Ưu tiên phát applyStudyClip, nếu chưa kéo thì phát tạm selectJob
        AudioClip clipToPlay = (applyStudyClip != null) ? applyStudyClip : selectJob;
        
        if (clipToPlay != null)
        {
            PlaySFX(clipToPlay);
            Debug.Log("<color=cyan>Sound: Apply/Study Success!</color>");
        }
    }

    // --- CÁC HÀM CẦU NỐI (GIỮ ĐỂ KHÔNG LỖI GAME) ---
    public void PlayPurchase() => PlayPurchaseSound(); 
    public void PlaySelectJob() => PlayApplyStudy();

    public void PlayCommonClick() => PlaySFX(commonClick);
    public void PlayYesClick() => PlaySFX(yesClick);
    public void PlayNoClick() => PlaySFX(noClick);
    public void PlayToggleSound() => PlaySFX(toggleSound);
    public void PlayToggleSound(bool s) => PlaySFX(toggleSound); 

    public void PlayWorkStudyVital() => PlaySFX(workStudyVital);
    public void PlayJobsShopVital() => PlaySFX(jobsShopVital);

    public void PlayEventSound() => PlaySFX(eventGood); 
    public void PlayEventSound(object data) => PlaySFX(eventGood); 
    public void PlayEventGood() => PlaySFX(eventGood);
    public void PlayEventBad() => PlaySFX(eventBad);
    public void PlayEventWarning() => PlaySFX(eventWarning);

    public void PlayEndingStinger(string type)
    {
        AudioClip clip = endingDefault;
        if (type == "Elite") clip = endingElite;
        else if (type == "Stable") clip = endingStable;
        else clip = endingOther;
        PlaySFX(clip);
    }

    // HÀM CỐT LÕI
    private void PlaySFX(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }
}