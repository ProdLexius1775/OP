/*using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player;
    private Vector3 offset;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        offset = transform.position - player.transform.position;

    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = player.transform.position + offset;
    }
}*/
using UnityEngine;
public class CameraController: MonoBehaviour
{
    public Transform target;         // The character to follow
    public Vector3 offset = new Vector3(0, 2, -5); // Offset from the target
    public float followSpeed = 5f;   // Smooth speed for following
    public float rotationSpeed = 2f; // Smooth speed for rotation
    void LateUpdate()
    {
        if (target == null) return;
        // Smoothly move camera to target position + offset
        Vector3 desiredPosition = target.position + target.rotation * offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);
        // Smoothly rotate camera to match target’s forward direction
        Quaternion desiredRotation = Quaternion.LookRotation(target.forward, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, rotationSpeed * Time.deltaTime);
    }
}












