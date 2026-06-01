using UnityEngine;

public class HazardCollider : MonoBehaviour
{
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            if (collider.TryGetComponent<PlayerRespawn>(out PlayerRespawn respawn))
            {
                respawn.Die();
            }
        }
    }
}
