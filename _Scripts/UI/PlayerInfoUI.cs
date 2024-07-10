using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/*
 * File     : PlayerInfoUI.cs
 * Desc     : 플레이어 스탯들의 정보 UI
 * Date     : 2024-05-06
 * Writer   : 정지훈
 */

public class PlayerInfoUI : MonoBehaviour
{
    [Header("Text")]
    public TextMeshProUGUI LevelText;
    public TextMeshProUGUI NameText;
    public TextMeshProUGUI HpText;
    public TextMeshProUGUI MpText;
    public TextMeshProUGUI ExpText;

    [Header("Slider")]
    public Slider HpSlider;
    public Slider MpSlider;
    public Slider ExpSlider;
    public Slider StaminaSlider;
}
