using UnityEngine;

public class KartCollisionHandler : MonoBehaviour
{   
    // REFERENCES
    private KartController _kartController;
    
    // VARIABLES 
    [Header("Bouncing Settings")]
    [SerializeField] private float _bounceForce;
    
    [Header("Speed Settings")]
    [SerializeField] private float _minSpeedToDestroy;


    private void Awake()
    {
        _kartController = GetComponent<KartController>();
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        DestructibleWall Wall = hit.gameObject.GetComponent<DestructibleWall>();

        if(Wall != null)
        {
            if(_kartController.Velocity.magnitude >= _minSpeedToDestroy)
            {
                Wall.BreakWall(hit.point, hit.normal);
            }
            else
            {
                _kartController.ApplyBounce(Vector3.Reflect(_kartController.Velocity * _bounceForce, hit.normal));
                Wall.BounceWall(hit.point, hit.normal);
            }
        }
    }
}   
