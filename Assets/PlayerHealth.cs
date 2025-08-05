using UnityEngine;
using UnityEngine.UI;
public class PlayerHealth : MonoBehaviour
{
    public Image fillImage;    // Drag your Fill Image here in the Inspector
    public float maxHealth = 100f;
    private float currentHealth;
    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
    }
    // Call this when the player takes damage
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;
        UpdateHealthBar();
        if (currentHealth == 0)
        {
            Die();
        }
    }
    // Call this to heal the player
    public void Heal(float amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;
        UpdateHealthBar();
    }
    void UpdateHealthBar()
    {
        // Fill amount must be between 0 and 1
        fillImage.fillAmount = currentHealth / maxHealth;
    }
    void Update()
    {
        // Press H to simulate taking damage
        if (Input.GetKeyDown(KeyCode.H))
        {
            TakeDamage(10f);  // Reduce health by 10
        }
    }
    void Die()
    {
        Debug.Log("Player died!");
        // Add your death logic here
    }
}






















