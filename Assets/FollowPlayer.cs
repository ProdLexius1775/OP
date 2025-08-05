using UnityEngine;
public class EnemyFollow : MonoBehaviour
{
    public Transform player; // Assign the Player in the inspector
    public float speed = 3.0f;
    public float stoppingDistance = 1.5f;
    public int damageAmount = 10;
    private void Update()
    {
        if (player == null) return;
        float distance = Vector3.Distance(transform.position, player.position);
        if (distance > stoppingDistance)
        {
            // Move toward the player
            Vector3 direction = (player.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;
            // Face the player
            transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("damage");
            PlayerHealth PlayerHealth = other.GetComponent<PlayerHealth>();
            if (PlayerHealth != null)
            {
                PlayerHealth.TakeDamage(damageAmount);

                PlayerHealth playerHealth = other.GetComponentInParent<PlayerHealth>();
            }
        }
    }
}


























