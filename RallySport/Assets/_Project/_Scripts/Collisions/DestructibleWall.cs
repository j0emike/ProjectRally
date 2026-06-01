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
        StartCoroutine(BreakWallCorutine(pos, normalPos));
    }

    private IEnumerator BreakWallCorutine(Vector3 pos, Vector3 normalPos)
    {
        //Statement to handle particles instantiation
        if (destructionVFX == null)
        {
            destructionVFX = Instantiate(destructionVFXPrefab, pos, Quaternion.LookRotation(normalPos)).GetComponent<ParticleSystem>();
        }
        gameObject.GetComponent<Renderer>().enabled = false;
        gameObject.GetComponent<BoxCollider>().enabled = false;
        if (bounceVFX != null)
        {
            Destroy(bounceVFX);
        }
        Debug.Log(destructionVFX.main.duration);
        yield return new WaitForSeconds(destructionVFX.main.duration);
        Destroy(destructionVFX.gameObject);
    }
    
    public void BounceWall(Vector3 pos, Vector3 normalPos)
    {
        StartCoroutine(BounceWallCorutine(pos, normalPos));
    }
    
    private IEnumerator BounceWallCorutine(Vector3 pos, Vector3 normalPos)
    {
        Debug.Log("Wall Bounced");
        if (bounceVFX == null)
        {
            bounceVFX = Instantiate(bounceVFXPrefab, pos, Quaternion.LookRotation(normalPos));
            yield return new WaitForSeconds(0.5f);
            bounceVFX.SetActive(false);
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
