using UnityEngine;

public class TestingClase : MonoBehaviour
{
    [SerializeField] private float _acceleration;
    [SerializeField] private float _totalAcceleration;

    private float _currentAcceleration;
    private float _maxAcceleration;

    private void Start()
    {
        _maxAcceleration = _totalAcceleration;
        _currentAcceleration = _maxAcceleration;
    }
}
