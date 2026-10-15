using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public Transform respawnPoint;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CharacterController controller = other.GetComponent<CharacterController>();
            if (controller != null)
            {
                controller.enabled = false;
            }

            other.transform.position = respawnPoint.position;

            if (controller != null)
            {
                controller.enabled = true;
            }
        }
    }
}
