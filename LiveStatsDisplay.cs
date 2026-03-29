using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LiveStatsDisplay : MonoBehaviour
{
    [Header("--- Sliders ---")]
    [SerializeField] private Slider cashSlider;
    [SerializeField] private Slider energySlider;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Slider stressSlider;
    [SerializeField] private Slider skillSlider;

    [Header("--- Texts ---")]
    [SerializeField] private TextMeshProUGUI cashText;
    [SerializeField] private TextMeshProUGUI energyText;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI stressText;
    [SerializeField] private TextMeshProUGUI skillText;

    [Header("--- Settings ---")]
    public float smoothSpeed = 5f;

    void Update()
    {
        // Kiểm tra PlayerStats tồn tại để tránh lỗi NullReference
        if (PlayerStats.Instance == null) return;
        
        PlayerStats p = PlayerStats.Instance;

        // 1. Cập nhật Slider mượt mà bằng Mathf.Lerp
        // Lưu ý: Đảm bảo trong Unity đã chỉnh Max Value cho các Slider này!
        if (cashSlider) cashSlider.value = Mathf.Lerp(cashSlider.value, p.cash, Time.deltaTime * smoothSpeed);
        if (energySlider) energySlider.value = Mathf.Lerp(energySlider.value, p.energy, Time.deltaTime * smoothSpeed);
        if (healthSlider) healthSlider.value = Mathf.Lerp(healthSlider.value, p.health, Time.deltaTime * smoothSpeed);
        if (stressSlider) stressSlider.value = Mathf.Lerp(stressSlider.value, p.stress, Time.deltaTime * smoothSpeed);
        if (skillSlider) skillSlider.value = Mathf.Lerp(skillSlider.value, p.skill, Time.deltaTime * smoothSpeed);

        // 2. Cập nhật Text số
        if (cashText) cashText.text = $"${p.cash:N0}"; // Hiện định dạng $1,234
        if (energyText) energyText.text = p.energy.ToString("F0"); // Hiện số nguyên (làm tròn)
        if (healthText) healthText.text = p.health.ToString("F0");
        if (stressText) stressText.text = p.stress.ToString("F0");
        if (skillText) skillText.text = p.skill.ToString("F0");
    }
}