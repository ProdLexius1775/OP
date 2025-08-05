using UnityEngine;
using UnityEngine.UI;
public class HealthBar : MonoBehaviour
{
    public Image fillImage;
    // Set health value between 0.0 (empty) and 1.0 (full)
    public void SetHealth(float healthNormalized)
    {
        fillImage.fillAmount = Mathf.Clamp01(healthNormalized);
    }
}
















   




