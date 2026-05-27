using UnityEngine;

public class AudioManager : Singleton<IAudioSource>, IAudioSource
{
    [SerializeField] private AudioSource _musicAudioSource;
    [SerializeField] private AudioSource _sfxAudioSource;
    [SerializeField] private AudioDataBase _audioDataBase;

    public void ButtonSFX() => PlayOneShot(""); // => (Para escribir en una misma linea funciones que se pueden delegar) *Ahorra lineas de codigo*

    public void ChangeBetweenButtons() => PlayOneShot("");

    public void ExitButton() => PlayOneShot("");

    public void PlayButton() => PlayOneShot("");


    public void PlayLevelMusic(string audioName)
    {
 
    }

    public void SelectButton() => PlayOneShot("SelectButton");

    public void SetMasterVolume(float volume)
    {
 
    }

    public void SetMusicVolume(float volume)
    {
 
    }

    public void SetSFXVolume(float volume)
    {
 
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


    //------- Volume Controls -------//
    void SetMasterVolume(float volume);
    void SetMusicVolume(float volume);
    void SetSFXVolume(float volume);
}