using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VitalUIManager : MonoBehaviour
{
    public static VitalUIManager Instance;

    public Slider energyBar, stressBar, healthBar;
    public TextMeshProUGUI moneyDisplay;

    void Awake() => Instance = this;

    public void RefreshUI()
    {
        if (PlayerStats.Instance == null) return;

        // Cập nhật 3 thanh trượt
        if (energyBar != null) energyBar.value = PlayerStats.Instance.energy / 100f;
        if (stressBar != null) stressBar.value = PlayerStats.Instance.stress / 100f;
        if (healthBar != null) healthBar.value = PlayerStats.Instance.health / 100f;

        // Hiển thị tiền dùng biến 'cash' từ PlayerStats
        if (moneyDisplay != null)
            moneyDisplay.text = "$" + PlayerStats.Instance.cash.ToString("N0");
    }
}
