using UnityEngine;
using TMPro;
using System.Collections;

public class PopupGuideManager : MonoBehaviour
{
    public static PopupGuideManager Instance;

    [Header("--- UI REFERENCES ---")]
    public GameObject guidelineFrame; 
    public TextMeshProUGUI textDisplay; 

    [Header("--- SETTINGS ---")]
    public float displayDuration = 10f;

    private bool isDisplaying = false;
    private bool[] hasTriggeredOnce = new bool[20]; 
    private bool[] isInDangerZone = new bool[20];

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        
        if (guidelineFrame != null) guidelineFrame.SetActive(false);
    }

    public void CheckConditions(PlayerStats p)
    {
        if (isDisplaying || p.gameEnded) return;

        // --- NHÓM 1: VƯỢT NGƯỠNG (Pop lại khi tái phạm) ---
        HandleThreshold(1, p.health < 30, "Health is your primary capital; it is the only asset that powers your ability to earn. If you bankrupted your body, no amount of Skill or Cash can buy back the time lost in a hospital bed.");
        HandleThreshold(2, p.energy < 20, "Treat your energy like a high-interest loan, not a free gift from the game. Overworking is a debt against your future; if you don't pay it back with rest today, your body will collect it with interest tomorrow.");
        HandleThreshold(3, p.stress > 80, "High stress is a silent tax on your productivity that clouds your long-term vision. A broken mind cannot calculate the path to a successful life, forcing you to make desperate choices.");
        HandleThreshold(7, p.cash < 50, "Cash flow is the oxygen of your survival; never let your lifestyle expenses outpace your daily energy levels. If outgoing costs exceed effort, you will suffocate in debt before you ever reach the finish line.");

        // --- NHÓM 2: HIỆN 1 LẦN DUY NHẤT ---
        HandleOnce(4, p.currentDay > 20 && p.currentJobTier == "Easy", "The journey from $6/hr to $160/hr is less about working more hours and more about growing your inner value. While hard work is the foundation, your Skills are the wings.");
        HandleOnce(5, p.cash >= 1200 && p.skill < 150, "Even the most advanced tools are just silent objects if you lack the knowledge to bring them to life. While equipment accelerates pace, Education is the engine.");
        HandleOnce(6, p.skill > 500, "Knowledge is the only global asset that never depreciates, even when the market shifts. Your Education remains a permanent shield against economic inequality.");
        HandleOnce(8, p.cash < 100 && p.currentHour == 0, "The $0.5 hourly fee is a constant reminder that time is a depleting resource you must manage wisely. Every hour spent idle is a lost investment, as the cost of living never sleeps.");
        HandleOnce(9, p.currentDay == 40 && !p.isTainted, "True success is not just about how fast you reach the top, but how much of yourself you keep intact along the way. Integrity is the only foundation for a truly balanced and wonderful life.");
        HandleOnce(10, p.isTainted == true, "Easy money from illegal acts comes with a hidden tax on your future. Some stains on your record are permanent, locking the doors to stable success.");
        HandleOnce(11, p.currentDay == 1, "Welcome! Your life is begin now. Every click is a transaction: you need to balance 5 core live stats. Remember to utilize your resources in an effective way. Everything still ahead. Good luck!");
        HandleOnce(12, p.skill >= 110 && p.currentJobTier == "Medium", "Having high skills but staying in middle-value jobs is a waste of human capital. It's time to leverage your education to reach Elite positions before time runs out.");
    }

    private void HandleThreshold(int id, bool condition, string msg)
    {
        if (condition) {
            if (!isInDangerZone[id]) {
                isInDangerZone[id] = true;
                StartCoroutine(ShowPopupRoutine(msg));
            }
        } else {
            isInDangerZone[id] = false; 
        }
    }

    private void HandleOnce(int id, bool condition, string msg)
    {
        if (condition && !hasTriggeredOnce[id]) {
            hasTriggeredOnce[id] = true; 
            StartCoroutine(ShowPopupRoutine(msg));
        }
    }

    IEnumerator ShowPopupRoutine(string message)
    {
        isDisplaying = true;
        textDisplay.text = message;
        guidelineFrame.SetActive(true);
        yield return new WaitForSeconds(displayDuration);
        guidelineFrame.SetActive(false);
        isDisplaying = false;
    }
}
