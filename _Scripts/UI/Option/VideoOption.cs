using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VideoOption : MonoBehaviour
{
    private FullScreenMode _screenMode;
    private List<Resolution> _resolutions = new List<Resolution>();
    private StringBuilder _stringBuilder = new StringBuilder();
    private int _resolutionIndex;

    [SerializeField]
    private TMP_Dropdown _resolutionDropdown;
    [SerializeField]
    private Toggle _fullScreenToggle;
    [SerializeField]
    private Button _okButton;

    private void Awake()
    {
        Init();

        _okButton.onClick.AddListener(() =>
        {
            Screen.SetResolution(_resolutions[_resolutionIndex].width, _resolutions[_resolutionIndex].height, _screenMode);
        });
    }

    private void Init()
    {
        int optionIndex = 0;

        _resolutions.AddRange(Screen.resolutions);
        _resolutionDropdown.options.Clear();

        foreach (Resolution item in _resolutions)
        {
            TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData();
            _stringBuilder.Append(item.width);
            _stringBuilder.Append("x");
            _stringBuilder.Append(item.height);
            _stringBuilder.Append(" ");
            option.text = _stringBuilder.ToString();
            _stringBuilder.Clear();
            _resolutionDropdown.options.Add(option);

            if (item.width == Screen.width && item.height == Screen.height)
            {
                _resolutionDropdown.value = optionIndex;
            }

            ++optionIndex;
        }

        _resolutionDropdown.RefreshShownValue();
        _fullScreenToggle.isOn = Screen.fullScreenMode.Equals(FullScreenMode.FullScreenWindow) ? true : false;
    }

    public void DropboxOptionChange(int x)
    {
        _resolutionIndex = x;
    }

    public void FullScreenModeChange(bool isFull)
    {
        _screenMode = isFull ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
    }
}
