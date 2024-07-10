using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File     : InitialSuccessValue.cs
 * Desc     : ScriptableObject
 *            초기 값이 필요한 퀘스트인 경우 초기 값이 필요하므로 초기 값을 전달 해주는 클래스
 * Date     : 2024-05-06
 * Writer   : 정지훈
 */

public abstract class InitialSuccessValue : ScriptableObject
{
    public abstract int GetValue(Task task);
}
