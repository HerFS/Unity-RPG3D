using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

/*
 * File     : CreateCharacter.cs
 * Desc     : Ä³¸¯ÅÍ »ý¼º
 * Date     : 2024-06-30
 * Writer   : Á¤ÁöÈÆ
 */

public class CreateCharacter : MonoBehaviour
{
    private MainMenuUI _mainMenuUI;

    [SerializeField]
    private TMP_InputField _nameInputField;
    [SerializeField]
    private Button _okButton;
    [SerializeField]
    private Button _backButton;


    private void Start()
    {
        _nameInputField.characterLimit = 10;

        _okButton.onClick.AddListener(() =>
        {
            string Check = Regex.Replace(_nameInputField.text, @"[^a-zA-Z0-9°¡-ÆR]", "", RegexOptions.Singleline);

            if (2 <= _nameInputField.text.Length && _nameInputField.text.Length <= 10 && _nameInputField.text.Equals(Check) == true)
            {
                DataManager.Instance.CreatePlayerStatusFile(_nameInputField.text);

                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                LodingSceneController.LoadScene(EnumTypes.SceneName.Village.ToString());
                UIManager.Instance.CharacterCreatePanel.SetActive(false);
            }
            else
            {
                _nameInputField.GetComponent<Animator>().SetTrigger("On");
            }
        });

        _backButton.onClick.AddListener(() =>
        {
            if (_mainMenuUI == null)
            {
                _mainMenuUI = FindObjectOfType<MainMenuUI>();
            }
            UIManager.Instance.CharacterCreatePanel.SetActive(false);
            _mainMenuUI.MainMenuPanel.SetActive(true);
            _mainMenuUI.IsCustomized = false;

            Camera.main.transform.position = _mainMenuUI.InitialPositionValue;
            Camera.main.transform.eulerAngles = _mainMenuUI.InitialRotationValue;
        });
    }
}
