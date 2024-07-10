using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/*
 * File     : TaskDescriptor.cs
 * Desc     : 대화 시스템
 * Date     : 2024-06-19
 * Writer   : 정지훈
 */

public class TaskDescriptor : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _text;

    [SerializeField]
    private Color _normalColor;
    [SerializeField]
    private Color _taskCompletionColor;
    [SerializeField]
    private Color _taskSuccessCountColor;
    [SerializeField]
    private Color _strikeThroughColor;

    public void UpdateText(string text)
    {
        this._text.fontStyle = FontStyles.Normal;
        this._text.text = text;
    }

    public void UpdateText(Task task)
    {
        _text.fontStyle = FontStyles.Normal;

        if (task.IsComplete)
        {
            var colorCode = ColorUtility.ToHtmlStringRGB(_taskCompletionColor);
            _text.text = BuildText(task, colorCode, colorCode);
        }
        else
        {
            _text.text = BuildText(task, ColorUtility.ToHtmlStringRGB(_normalColor), ColorUtility.ToHtmlStringRGB(_taskSuccessCountColor));
        }
    }

    public void UpdateTextUsingStrikeThrough(Task task)
    {
        var colorCode = ColorUtility.ToHtmlStringRGB(_strikeThroughColor);
        _text.fontStyle = FontStyles.Strikethrough;
        _text.text = BuildText(task, colorCode, colorCode);
    }

    private string BuildText(Task task, string textColorCode, string successCountColorCode)
    {
        return $"<color=#{textColorCode}> - {task.Description} <color=#{successCountColorCode}> {task.CurrentSuccess}</color> / {task.NeedSuccessToComplete} </color>";
    }
}
