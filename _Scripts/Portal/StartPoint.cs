using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
 * File     : StartPoint.cs
 * Desc     : Scene�� ���۵��� �� ���� ��ġ
 * Date     : 2024-06-30
 * Writer   : ������
 */

public class StartPoint : MonoBehaviour
{
    [SerializeField]
    private EnumTypes.SceneName _currentScene;
    [SerializeField]
    private Transform _startPoint;

    private void Start()
    {
        if (_currentScene.ToString() == SceneManager.GetActiveScene().name)
        {
            GameManager.Instance.Player.position = _startPoint.position;
        }
    }
}
