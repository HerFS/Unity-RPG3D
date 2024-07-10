using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File     : DontDestroyObject.cs
 * Desc     : Scene 이동 시 삭제 X
 * Date     : 2024-06-30
 * Writer   : 정지훈
 */

public class DontDestroyObject<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;

            if (transform.root != null && transform.parent != null)
            {
                DontDestroyOnLoad(this.transform.root.gameObject);
            }
            else
            {
                DontDestroyOnLoad(this.gameObject);
            }
        }
        else
        {
            Destroy(this.gameObject, Time.deltaTime);
        }
    }
}
