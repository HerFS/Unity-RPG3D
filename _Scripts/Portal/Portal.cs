using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
 * File     : Portal.cs
 * Desc     : ��Ż ���
 * Date     : 2024-06-30
 * Writer   : ������
 */

public class Portal : MonoBehaviour
{
    [SerializeField]
    private EnumTypes.SceneName _loadScene;
    private SphereCollider _sphereCollider;

    private void Awake()
    {
        _sphereCollider = GetComponent<SphereCollider>();
        _sphereCollider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Contains(Globals.TagName.Player))
        {
            UIManager.Instance.InteractionInfoPanel.SetActive(true);
            UIManager.Instance.InteractionInfoText.text = $"\"{_loadScene}\" �ڷ���Ʈ (F)";
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag.Contains(Globals.TagName.Player) && InputManager.Instance.IsInteraction)
        {
            LodingSceneController.LoadScene(_loadScene.ToString());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag.Contains(Globals.TagName.Player))
        {
            UIManager.Instance.InteractionInfoPanel.SetActive(false);
        }
    }
}
