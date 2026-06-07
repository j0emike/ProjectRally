using System;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : Singleton<IAudioSource>, IAudioSource
{
    [SerializeField] private AudioSource _musicAudioSource;
    [SerializeField] private AudioSource _sfxAudioSource;
    [SerializeField] private AudioSource _engineAudioSource;
    [SerializeField] private AudioDataBase _audioDataBase;
    [SerializeField] private AudioMixer _audioMixer;

    public void ButtonSFX() => PlayOneShot(""); // => (Para escribir en una misma linea funciones que se pueden delegar) *Ahorra lineas de codigo*
    public void ChangeBetweenButtons() => PlayOneShot("ChangeBettwenButtonsv1");
    public void ExitButton() => PlayOneShot("ExitButton");
    public void PlayButton() => PlayOneShot("PlayButton");
    public void SelectButton() => PlayOneShot("SelectButton");

    // SFX para el carro
    public void PlayIdleSFX() => PlayOneShot("KartIdle");
    public void PlayAccelerationSFX() => PlayOneShot("KartAcceleration");

    public void PlayVelocitySFX()
    {
        if(_engineAudioSource.isPlaying) return;

        _engineAudioSource.clip = _audioDataBase.GetAudio("KartVelocity");
        _engineAudioSource.Play();
    }

    public void StopVelocitySFX()
    {
        if (!_engineAudioSource.isPlaying) return;

        _engineAudioSource.Stop();
    }

    public void PlayBreaksSFX() => PlayOneShot("KartBreaks");
    public void PlayCrashSFX() => PlayOneShot("KartCrash");
    public void PlayGlassBreakSFX() => PlayOneShot("GlassBreak");
    public void PlayMoveOnIceSFX() => PlayOneShot("KartIce");
    public void PlayMoveOnSandSFX() => PlayOneShot("KartSand");

    public void PlayLevelMusic(string audioName)
    {
        _musicAudioSource.clip = _audioDataBase.GetAudio(audioName);
        _musicAudioSource.Play();
    }

    public void SetMasterVolume(float volume) => SetMixerVolume("MasterVolume", volume);

    public void SetMusicVolume(float volume) => SetMixerVolume("MusicVolume", volume);
    public void SetSFXVolume(float volume) => SetMixerVolume("SFXVolume", volume);
    private void SetMixerVolume(string v, float volume)
    {
        throw new NotImplementedException();
    }
    private void PlayOneShot(string audioName) // Funcion para evitar redundancia de codigo, ya que se repite en varios metodos
    {
        _sfxAudioSource.PlayOneShot(_audioDataBase.GetAudio(audioName));
    }
}

public interface IAudioSource
{
    //------- Music Audio -------//
    void PlayLevelMusic(string audioName);


    //------- SFX Audio -------//
    void ButtonSFX();
    void ChangeBetweenButtons();
    void SelectButton();
    void ExitButton();
    void PlayButton();

    void PlayIdleSFX();
    void PlayAccelerationSFX();
    void PlayVelocitySFX();
    void PlayBreaksSFX();
    void PlayCrashSFX();
    void PlayGlassBreakSFX();
    void PlayMoveOnIceSFX();
    void PlayMoveOnSandSFX();

    void StopVelocitySFX();


    //------- Volume Controls -------//
    void SetMasterVolume(float volume);
    void SetMusicVolume(float volume);
    void SetSFXVolume(float volume);
}