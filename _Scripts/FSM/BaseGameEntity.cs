using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File     : BaseGameEntity.cs
 * Desc     : FSM ������ ����ϸ� ���¿� ���� Animation CrossFade�� ����
 *            ��� Entity���� BaseGameEntity�� ��� ����.
 * Date     : 2024-04-30
 * Writer   : ������
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
