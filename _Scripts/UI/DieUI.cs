using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * File     : DieUI.cs
 * Desc     : 죽었을 때 나타나는 UI
 * Date     : 2024-06-30
 * Writer   : 정지훈
 */

public class DieUI : MonoBehaviour
{
    public Button VillageRestartButton;
    public Button CurrentRestartButton;

    private void Awake()
    {
        VillageRestartButton.onClick.AddListener(() =>
        {
            PlayerEntity.ChangeState(EnumTypes.PlayerState.Idle);

            this.gameObject.SetActive(false);
            DataManager.Instance.PlayerStatus.CurrentHp = DataManager.Instance.PlayerStatus.MaxHp;
            LodingSceneController.LoadScene(EnumTypes.SceneName.Village.ToString());
            InputManager.Instance.gameObject.SetActive(true);
            Time.timeScale = 1f;
        });

        CurrentRestartButton.onClick.AddListener(() =>
        {
            uint restartPrice = 1000;

            if (DataManager.Instance.PlayerStatus.Money >= restartPrice)
            {
                PlayerEntity.ChangeState(EnumTypes.PlayerState.Idle);

                this.gameObject.SetActive(false);
                DataManager.Instance.PlayerStatus.Money -= restartPrice;
                DataManager.Instance.PlayerStatus.CurrentHp = DataManager.Instance.PlayerStatus.MaxHp;
                LodingSceneController.LoadScene(DataManager.Instance.PlayerData.CurrentScene);
                InputManager.Instance.gameObject.SetActive(true);
                Time.timeScale = 1f;
            }
        });
    }
}
