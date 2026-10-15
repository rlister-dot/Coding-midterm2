using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform player; // Assign your player's Transform in the Inspector
    private Vector3 offset;                    // Stores the initial offset distance

    void Start()
    {
        // Calculate the initial distance between the camera and the player
        offset = transform.position - player.position;
    }

    void LateUpdate()
    {
        // Update the camera position every frame
        transform.position = player.position + offset;
    }
}
