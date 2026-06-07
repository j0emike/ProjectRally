using UnityEngine;

public class LevelAudio : MonoBehaviour
{
    [SerializeField] private string _musicName;

    void Start()
    {
        AudioManager.Source.PlayLevelMusic(_musicName);
    }
}
