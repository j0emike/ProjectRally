using UnityEngine;
using UnityEngine.UI;

public class TestButton : MonoBehaviour
{
    private Button _button;


    void Awake()
    {

        _button = GetComponent<Button>();
        
    }

    void Start()
    {
        _button.onClick.AddListener(SelectButton);
    }


    void SelectButton()
    {
        AudioManager.Source.SelectButton();
    }
    void ExitButton()
    {
        AudioManager.Source.ExitButton();
    }
    void StartButton()
    {
        AudioManager.Source.PlayButton();
    }
    void ChangeBettwenButtons()
    {
        AudioManager.Source.ChangeBetweenButtons();
    }
}
