using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File     : BaseGameEntity.cs
 * Desc     : FSM 패텬을 사용하며 상태에 따라 Animation CrossFade를 실행
 *            모든 Entity들은 BaseGameEntity를 상속 받음.
 * Date     : 2024-04-30
 * Writer   : 정지훈
 */

[RequireComponent(typeof(Animator))]
public abstract class BaseGameEntity : MonoBehaviour
{
    [HideInInspector]
    public Animator Animator;

    public virtual void Setup()
    {
        Animator = GetComponent<Animator>();
    }

    public abstract void Updated();
}
