using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
 * File     : GameManager.cs
 * Desc     : 게임 관리
 * Date     : 2024-06-30
 * Writer   : 정지훈
 */

public class GameManager : Singleton<GameManager>
{
    public Transform Player;

    public bool IsGameStart => SceneManager.GetActiveScene().name != EnumTypes.SceneName.MainMenu.ToString() &&
        SceneManager.GetActiveScene().name != EnumTypes.SceneName.LoadingScene.ToString() &&
        SceneManager.GetActiveScene().name != EnumTypes.SceneName.StartScene.ToString();

    protected override void Awake()
    {
        base.Awake();

        if (Player == null)
        {
            Player = GameObject.Find(Globals.TagName.Player).GetComponent<Transform>();
        }
    }

    private void Start()
    {
        Player.gameObject.SetActive(false);

        LodingSceneController.LoadScene(EnumTypes.SceneName.MainMenu.ToString());
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (SceneManager.GetActiveScene().name == EnumTypes.SceneName.MainMenu.ToString())
        {
            ShowMouse();
        }

        if (SceneManager.GetActiveScene().name == EnumTypes.SceneName.LoadingScene.ToString())
        {
            Player.gameObject.SetActive(false);
        }
        else
        {
            Player.gameObject.SetActive(true);
        }

        if (IsGameStart)
        {
            if (!Player.gameObject.activeSelf)
            {
                Player.gameObject.SetActive(true);
            }
            UIManager.Instance.PlayerInfoPanel.gameObject.SetActive(true);
            DataManager.Instance.PlayerStatus.CurrentScene = scene.name;
            InputManager.Instance.gameObject.SetActive(true);
            HideMouse();
        }
        else
        {
            InputManager.Instance.gameObject.SetActive(false);
        }

        if (UIManager.Instance.InteractionInfoPanel.activeSelf)
        {
            UIManager.Instance.InteractionInfoPanel.SetActive(false);
            InputManager.Instance.IsRun = false;
            InputManager.Instance.MoveVector = Vector2.zero;
        }
    }

    public void ShowMouse()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void HideMouse()
    {
        if (!UIManager.Instance.IsUIActive)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
