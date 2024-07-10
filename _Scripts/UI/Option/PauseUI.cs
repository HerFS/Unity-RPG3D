using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseUI : MonoBehaviour
{
    public Button PlayButton;
    public Button OptionButton;
    public Button MainMenuButton;
    public Button ExitButton;

    private void Start()
    {
        PlayButton.onClick.AddListener(() =>
        {
            Time.timeScale = 1f;
            InputManager.Instance.gameObject.SetActive(true);
            this.gameObject.SetActive(false);
            GameManager.Instance.HideMouse();
        });

        OptionButton.onClick.AddListener(() =>
        {
            InputManager.Instance.UIObject.Push(UIManager.Instance.OptionPanel);
            UIManager.Instance.OptionPanel.SetActive(true);
        });

        MainMenuButton.onClick.AddListener(() =>
        {
            Time.timeScale = 1f;
            UIManager.Instance.HidePanel();
            LodingSceneController.LoadScene(EnumTypes.SceneName.MainMenu.ToString());
        });

        ExitButton.onClick.AddListener(() =>
        {
            Application.Quit();
        });
    }
}
