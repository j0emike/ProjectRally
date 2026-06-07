using UnityEngine;

public class WinZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (UILevelEndManager.Instance != null)
            {
                UILevelEndManager.Instance.WinLevel();
            }
            else
            {
                Debug.LogWarning("WinZone: UILevelEndManager.Instance is null! Cannot trigger win.");
            }
        }
    }
}
