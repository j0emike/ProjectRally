using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class KartController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float acceleration = 25f;
    [SerializeField] private float maxSpeed = 20f;
    [SerializeField] private float reverseSpeed = 8f;
    [SerializeField] private float brakeForce = 40f;

    [Header("Steering")]
    [SerializeField] private float steeringPower = 120f;
    [SerializeField] private float driftSteeringMultiplier = 1.4f;

    [Header("Traction")]
    [Range(0f, 1f)]
    [SerializeField] public float normalTraction = 0.92f;

    [Header("Deceleration")]
    [SerializeField] public float _deceleration;

    [Header("Drift")]
    [Range(0f, 1f)]
    [SerializeField] public float driftTraction = 0.82f;

    // =========================
    // FloorTypes conection by Gonzalinio
    // =========================
    public float NormalTraction { get => normalTraction; set => normalTraction = Mathf.Clamp01(value); }
    public float DriftTraction { get => driftTraction; set => driftTraction = Mathf.Clamp01(value); }
    public float Deceleration { get => _deceleration; set => _deceleration = value; }

    private CharacterController controller;

    // INPUTS
    private Vector2 moveInput;
    private bool driftPressed;
    private bool brakePressed;
    private bool isAutoDrifting;

    // VELOCITY
    private Vector3 velocity;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        HandleMovement();
        MoveKart();
    }

    // =========================
    // INPUT SYSTEM
    // =========================

    public void OnMove(InputAction.CallbackContext context)
    {
        if (isAutoDrifting) return;
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnDrift(InputAction.CallbackContext context)
    {
        if (isAutoDrifting) return;
        driftPressed = context.ReadValueAsButton();
    }

    public void OnBrake(InputAction.CallbackContext context)
    {
        if (isAutoDrifting) return;
        brakePressed = context.ReadValueAsButton();
    }

    public void StartAutoDrifting()
    {
        isAutoDrifting = true;
        driftPressed = true;
        brakePressed = false;
        moveInput = new Vector2(1f, 1f); // Steer right, accelerate forward
    }

    public void StopAutoDrifting()
    {
        isAutoDrifting = false;
        driftPressed = false;
        moveInput = Vector2.zero;
        ResetVelocity();
    }

    // =========================
    // MOVEMENT
    // =========================

    private void HandleMovement()
    {
        float forwardInput = moveInput.y;
        float steerInput = moveInput.x;

        Vector3 forwardVelocity =
            transform.forward *
            Vector3.Dot(velocity, transform.forward);
        //@Sebas, la velocidad lateral es para que haga el Drift
        Vector3 lateralVelocity =
            transform.right *
            Vector3.Dot(velocity, transform.right);

        // =========================
        // ACCELERATION
        // =========================

        if (forwardInput != 0f)
        {
            forwardVelocity +=
                transform.forward *
                forwardInput *
                acceleration *
                Time.deltaTime;
        }

        // =========================
        // BRAKE
        // =========================

        if (brakePressed)
        {
            forwardVelocity = Vector3.MoveTowards(
                forwardVelocity,
                Vector3.zero,
                brakeForce * Time.deltaTime
            );
        }

         // =========================
        // DESACELERACION - JORGE
        // =========================

        if(forwardInput == 0f && !brakePressed)
        {
            forwardVelocity = Vector3.MoveTowards(
                forwardVelocity,
                Vector3.zero,
                _deceleration * Time.deltaTime
            );
        }

        // =========================
        // SPEED LIMIT
        // =========================

        float currentForwardSpeed =
            Vector3.Dot(forwardVelocity, transform.forward);

        currentForwardSpeed = Mathf.Clamp(
            currentForwardSpeed,
            -reverseSpeed,
            maxSpeed
        );

        forwardVelocity =
            transform.forward * currentForwardSpeed;

        // =========================
        // TRACTION / DRIFT
        // =========================

        float traction =
        //Esto es un operador ternario, 
        // si el drift esta presionado se usa la traccion de drift, 
        // sino se usa la traccion normal
            driftPressed ? driftTraction : normalTraction;

        lateralVelocity *= traction;

        // =========================
        // FINAL VELOCITY
        // =========================

        velocity = forwardVelocity + lateralVelocity;

        // =========================
        // STEERING
        // =========================

        //@STV, por aqui revisa lo de la aceleracion, si tienes dudas preguntame @Penia
        float speedPercent =
        //Esto es para que a medida que el kart se acerque a su velocidad maxima, 
        // la capacidad de giro disminuya
            Mathf.Clamp01(
                Mathf.Abs(currentForwardSpeed) / maxSpeed
            );

        float steerMultiplier =
            driftPressed ? driftSteeringMultiplier : 1f;

        float rotation =
            steerInput *
            steeringPower *
            steerMultiplier *
            speedPercent *
            Time.deltaTime;

        transform.Rotate(0f, rotation, 0f);
    }

    // =========================
    // APPLY MOVEMENT
    // =========================

    private void MoveKart()
    {
        controller.Move(velocity * Time.deltaTime);
    }

    // =========================
    // COLLISIONS - Jorge :)
    // =========================

    public Vector3 Velocity => velocity;
    public void ApplyBounce(Vector3 BouncingVelocity)
    {
        Debug.Log("Bouncing"); 
        velocity = BouncingVelocity;
    }

    public void ResetVelocity()
    {
        velocity = Vector3.zero;
    }
}