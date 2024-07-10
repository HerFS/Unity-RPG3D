using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File     : InitialSuccessValue.cs
 * Desc     : ScriptableObject
 *            �ʱ� ���� �ʿ��� ����Ʈ�� ��� �ʱ� ���� �ʿ��ϹǷ� �ʱ� ���� ���� ���ִ� Ŭ����
 * Date     : 2024-05-06
 * Writer   : ������
 */

public abstract class InitialSuccessValue : ScriptableObject
{
    public abstract int GetValue(Task task);
}
