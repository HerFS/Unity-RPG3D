using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File     : DamageText.cs
 * Desc     : 몬스터 데미지 텍스트
 * Date     : 2024-06-30
 * Writer   : 정지훈
 */

public class DamageText : MonoBehaviour
{
    [SerializeField]
    private float _destroyTime;
    private Vector3 _offset = new Vector3(0f, 1.5f, 0f);
    private Vector3 _randomValue = new Vector3(0.7f, 0f, 0f);

    void Start()
    {
        Destroy(this.gameObject, _destroyTime);
        
        transform.localPosition += _offset;
        transform.localPosition += new Vector3(Random.Range(-_randomValue.x, _randomValue.x),
            Random.Range(-_randomValue.y, _randomValue.y),
            Random.Range(-_randomValue.z, _randomValue.z));
    }
}
