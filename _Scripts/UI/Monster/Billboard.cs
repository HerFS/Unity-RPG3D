using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File     : Billboard.cs
 * Desc     : UI가 플레이어를 바라봄
 * Date     : 2024-05-06
 * Writer   : 정지훈
 */

public class Billboard : MonoBehaviour
{
    private void LateUpdate()
    {
        transform.forward = Camera.main.transform.forward;
    }
}
