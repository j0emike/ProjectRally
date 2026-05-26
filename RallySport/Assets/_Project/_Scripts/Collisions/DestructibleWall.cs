using System.Collections;
using UnityEngine;

public class DestructibleWall : MonoBehaviour
{
    [SerializeField] private GameObject destructionVFXPrefab;
    [SerializeField] private GameObject bounceVFXPrefab;
    private ParticleSystem destructionVFX;
    private GameObject bounceVFX;
    public void BreakWall(Vector3 pos, Vector3 normalPos)
    {
        Debug.Log("Wall Destroyed");
        
        //Statement to handle particles instantiation
        if (destructionVFX == null)
        {
            destructionVFX = Instantiate(destructionVFXPrefab, pos, Quaternion.LookRotation(normalPos)).GetComponent<ParticleSystem>();
        }
        else
        {
            destructionVFX.transform.position = pos;
            destructionVFX.transform.rotation = Quaternion.LookRotation(normalPos);
            destructionVFX.Play();
        }
        Destroy(gameObject);
    }

    public void BounceWall(Vector3 pos, Vector3 normalPos)
    {
        StartCoroutine(BounceWallCorutine(pos, normalPos));
    }
    
    public IEnumerator BounceWallCorutine(Vector3 pos, Vector3 normalPos)
    {
        Debug.Log("Wall Bounced");
        if (bounceVFX == null)
        {
            bounceVFX = Instantiate(bounceVFXPrefab, pos, Quaternion.LookRotation(normalPos));
        }
        else
        {
            bounceVFX.SetActive(true);
            bounceVFX.transform.position = pos;
            bounceVFX.transform.rotation = Quaternion.LookRotation(normalPos);
            yield return new WaitForSeconds(0.5f);
            bounceVFX.SetActive(false);
        }
    }
}
