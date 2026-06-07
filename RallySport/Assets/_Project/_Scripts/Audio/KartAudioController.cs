using UnityEngine;

[RequireComponent(typeof(KartController))]
public class KartAudioController : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource _engineAudioSource;
    [SerializeField] private AudioSource _sfxAudioSource;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip _idleClip;
    [SerializeField] private AudioClip _accelerationClip;
    [SerializeField] private AudioClip _velocityClip;
    [SerializeField] private AudioClip _brakesClip;
    [SerializeField] private AudioClip _crashOnConcreteClip;
    [SerializeField] private AudioClip _glassBreakClip;
    [SerializeField] private AudioClip _moveOnIceClip;
    [SerializeField] private AudioClip _moveOnSandClip;

    [Header("Engine Tuning")]
    [SerializeField] private float _minPitch = 0.8f;
    [SerializeField] private float _maxPitch = 2.0f;

    private KartController _kart;
    private float _maxSpeed;

    // Estado para evitar reproducir SFX repetidos
    private bool _wasBreaking = false;
    private bool _wasDrifting = false;

    private enum EngineState { Idle, Accelerating, AtSpeed }
    private EngineState _currentEngineState = EngineState.Idle;

    private void Awake()
    {
        _kart = GetComponent<KartController>();

        // Leer maxSpeed via reflexión para no modificar KartController
        var field = typeof(KartController)
            .GetField("maxSpeed",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);
        _maxSpeed = field != null ? (float)field.GetValue(_kart) : 20f;
    }

    private void Start()
    {
        PlayEngineLoop(_idleClip);
    }

    private void Update()
    {
        float speed = _kart.Velocity.magnitude;
        float speedPercent = Mathf.Clamp01(speed / _maxSpeed);

        HandleEngineAudio(speed, speedPercent);
        HandlePitch(speedPercent);
    }

    // =========================
    // ENGINE LOOP
    // =========================

    private void HandleEngineAudio(float speed, float speedPercent)
    {
        EngineState targetState;

        if (speed < 0.5f)
            targetState = EngineState.Idle;
        else if (speedPercent < 0.6f)
            targetState = EngineState.Accelerating;
        else
            targetState = EngineState.AtSpeed;

        if (targetState == _currentEngineState) return;

        _currentEngineState = targetState;

        switch (_currentEngineState)
        {
            case EngineState.Idle:
                PlayEngineLoop(_idleClip);
                break;
            case EngineState.Accelerating:
                PlayEngineLoop(_accelerationClip);
                break;
            case EngineState.AtSpeed:
                PlayEngineLoop(_velocityClip);
                break;
        }
    }

    private void PlayEngineLoop(AudioClip clip)
    {
        if (_engineAudioSource.clip == clip) return;
        _engineAudioSource.clip = clip;
        _engineAudioSource.loop = true;
        _engineAudioSource.Play();
    }

    // El pitch sube con la velocidad para simular cambios de marcha
    private void HandlePitch(float speedPercent)
    {
        _engineAudioSource.pitch = Mathf.Lerp(_minPitch, _maxPitch, speedPercent);
    }

    // =========================
    // SFX PÚBLICOS (llamar desde KartController o colisiones)
    // =========================

    public void OnBrake(bool isBraking)
    {
        if (isBraking && !_wasBreaking)
            _sfxAudioSource.PlayOneShot(_brakesClip);

        _wasBreaking = isBraking;
    }

    public void OnCrashConcrete() =>
        _sfxAudioSource.PlayOneShot(_crashOnConcreteClip);

    public void OnGlassBreak() =>
        _sfxAudioSource.PlayOneShot(_glassBreakClip);

    public void OnMoveOnIce(bool isOnIce)
    {
        if (isOnIce && !_sfxAudioSource.isPlaying)
        {
            _sfxAudioSource.clip = _moveOnIceClip;
            _sfxAudioSource.loop = true;
            _sfxAudioSource.Play();
        }
        else if (!isOnIce && _sfxAudioSource.clip == _moveOnIceClip)
        {
            _sfxAudioSource.Stop();
        }
    }

    public void OnMoveOnSand(bool isOnSand)
    {
        if (isOnSand && !_sfxAudioSource.isPlaying)
        {
            _sfxAudioSource.clip = _moveOnSandClip;
            _sfxAudioSource.loop = true;
            _sfxAudioSource.Play();
        }
        else if (!isOnSand && _sfxAudioSource.clip == _moveOnSandClip)
        {
            _sfxAudioSource.Stop();
        }
    }
}