using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
 * File     : StartPoint.cs
 * Desc     : Scene이 시작됐을 때 시작 위치
 * Date     : 2024-06-30
 * Writer   : 정지훈
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
