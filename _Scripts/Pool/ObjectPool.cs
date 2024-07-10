using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File     : ObjectPool.cs
 * Desc     : 오브젝트 풀
 * Date     : 2024-06-01
 * Writer   : 정지훈
 */

public class ObjectPool : MonoBehaviour
{
    private Queue<GameObject> _poolObjectQueue;

    [SerializeField]
    private GameObject _prefab;
    [SerializeField]
    public int _maxObjectCount;

    private void Awake()
    {
        _poolObjectQueue = new Queue<GameObject>();
        Initialize();
    }

    private void Initialize()
    {
        for (int i = 0; i < _maxObjectCount; ++i)
        {
            GameObject newObj = Instantiate(_prefab);
            newObj.SetActive(false);
            newObj.transform.SetParent(this.transform);
            _poolObjectQueue.Enqueue(newObj);
        }
    }

    public GameObject Allocate()
    {
        if (_poolObjectQueue.Count == 0)
        {
            Initialize();
        }

        GameObject obj = _poolObjectQueue.Dequeue();
        obj.SetActive(true);

        return obj;
    }

    public void Free(GameObject obj)
    {
        obj.gameObject.SetActive(false);
        _poolObjectQueue.Enqueue(obj);
    }
}
