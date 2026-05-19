using UnityEngine;

public class TestingClase : MonoBehaviour
{
    [SerializeField] private float _acceleration;

    private float _currentAcceleration;

    private void Start()
    {
        _currentAcceleration = _acceleration;
    }
}
