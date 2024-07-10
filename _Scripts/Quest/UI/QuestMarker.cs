using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File     : QuestMark.cs
 * Desc     : Npc Äù½ºÆ® Ç¥½Ã UI
 * Date     : 2024-06-18
 * Writer   : Á¤ÁöÈÆ
 */

public class QuestMarker : MonoBehaviour
{
    [HideInInspector]
    public Renderer Renderer;
    [SerializeField]
    public Material QuestionMarkMaterial;
    [SerializeField]
    public Material ExclamationMarkMaterial;

    private void Awake()
    {
        Renderer = GetComponent<Renderer>();
    }
}
