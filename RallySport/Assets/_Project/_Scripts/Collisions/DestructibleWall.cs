using UnityEngine;

public class DestructibleWall : MonoBehaviour
{
    [SerializeField] private GameObject destructionVFXPrefab;
    private ParticleSystem destructionVFX;
    public void BreakWall(Vector3 pos)
    {
        Debug.Log("Wall Destroyed");
        
        //Statement to handle particles instantiation
        if (destructionVFX == null)
        {
            destructionVFX = Instantiate(destructionVFXPrefab, pos, Quaternion.identity).GetComponent<ParticleSystem>();
        }
        else
        {
            destructionVFX.transform.position = pos;
            destructionVFX.Play();
        }
        Destroy(gameObject);
    }
}
