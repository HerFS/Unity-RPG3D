using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File     : Singleton.cs
 * Desc     : �̱��� ����
 * Date     : 2024-04-30
 * Writer   : ������
 */

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = (T)FindObjectOfType(typeof(T));

                if (_instance == null)
                {
                    GameObject newObj = new GameObject(typeof(T).Name, typeof(T));
                    _instance = newObj.GetComponent<T>();
                }
            }

            return _instance;
        }
    }

    /// <summary>
    /// �Ļ� Ŭ�������� base.Setup() ȣ��
    /// </summary>
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
