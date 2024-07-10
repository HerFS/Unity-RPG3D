using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/*
 * File     : MainMenuUI.cs
 * Desc     : 시작 화면 UI
 * Date     : 2024-06-30
 * Writer   : 정지훈
 */

public class MainMenuUI : MonoBehaviour
{
    private readonly string _statusFilePath = Application.dataPath + "/Resources/PlayerData.json";
    private readonly float _moveSpeed = 5f;

    [HideInInspector]
    public bool IsCustomized;
    [HideInInspector]
    public Vector3 InitialPositionValue;
    [HideInInspector]
    public Vector3 InitialRotationValue;

    [SerializeField]
    private Transform _lookTransform;


    [Header("Panel")]
    public GameObject MainMenuPanel;

    [Header("Button")]
    [SerializeField]
    private Button _playButton;
    [SerializeField]
    private Button _optionButton;
    [SerializeField]
    private Button _howToPlayButton;
    [SerializeField]
    private Button _exitButton;

    private void Start()
    {
        InitialPositionValue = Camera.main.transform.position;
        InitialRotationValue = Camera.main.transform.eulerAngles;

        _playButton.onClick.AddListener(() =>
        {
            if (File.Exists(_statusFilePath))
            {
                LodingSceneController.LoadScene(DataManager.Instance.PlayerData.CurrentScene);
            }
            else
            {
                // 캐릭터 커스텀 창으로 이동
                if (!GameManager.Instance.Player.gameObject.activeSelf)
                {
                    GameManager.Instance.Player.gameObject.SetActive(true);
                }

                MainMenuPanel.SetActive(false);
                GameManager.Instance.Player.eulerAngles = new Vector3(0f, 90f, 0f);
                UIManager.Instance.CharacterCreatePanel.SetActive(true);
                IsCustomized = true;
            }
        });

        _optionButton.onClick.AddListener(() =>
        {
            UIManager.Instance.OptionPanel.SetActive(true);
        });

        _exitButton.onClick.AddListener(() =>
        {
            Application.Quit();
        });
    }

    private void Update()
    {
        if (IsCustomized)
        {
            Camera.main.transform.LookAt(_lookTransform);
            Camera.main.transform.position = Vector3.MoveTowards(Camera.main.transform.position, _lookTransform.position, _moveSpeed * Time.deltaTime);
        }
    }
}
