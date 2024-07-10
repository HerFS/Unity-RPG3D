using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File     : Billboard.cs
 * Desc     : UI�� �÷��̾ �ٶ�
 * Date     : 2024-05-06
 * Writer   : ������
 */

public class Billboard : MonoBehaviour
{
    private void LateUpdate()
    {
        transform.forward = Camera.main.transform.forward;
    }
}
