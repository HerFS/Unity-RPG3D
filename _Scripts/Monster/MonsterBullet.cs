using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File     : MonsterBullet.cs
 * Desc     : ∏ÛΩ∫≈Õ¿« √—æÀ
 * Date     : 2024-06-30
 * Writer   : ¡§¡ˆ»∆
 */

public class MonsterBullet : MonoBehaviour
{
    [SerializeField]
    private MonsterStatus _monsterStatus;
    private Vector3 _disVector;
    private float _timer;

    [SerializeField]
    private float _bulletSpeed;
    [SerializeField]
    private float _destroyTime;
    [SerializeField]
    private float _guidTime;

    [SerializeField]
    private bool _isGuidedBullet;

    public float BulletDamage { get; set; }

    private void OnEnable()
    {
        if (_monsterStatus == null)
        {
            _monsterStatus = GetComponentInParent<MonsterStatus>();
        }

        _disVector = GuiedPosCalculation();
        gameObject.transform.SetParent(null);
    }

    private void FixedUpdate()
    {
        _timer += Time.fixedDeltaTime;

        if (_timer >= _destroyTime)
        {
            _timer = 0;
            _monsterStatus.BulletPool.Free(this.gameObject);
        }

        if (_isGuidedBullet)
        {
            _disVector = GuiedPosCalculation();

            if (_timer >= _guidTime)
            {
                _isGuidedBullet = false;
            }
        }

        transform.position += _disVector * _bulletSpeed * Time.fixedDeltaTime;
    }

    private Vector3 GuiedPosCalculation()
    {
        return new Vector3((GameManager.Instance.Player.position.x - this.transform.position.x),
                                    ((GameManager.Instance.Player.position.y + 1f) - this.transform.position.y),
                                    (GameManager.Instance.Player.position.z - this.transform.position.z)).normalized;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Contains(Globals.TagName.Player))
        {
            DataManager.Instance.PlayerStatus.CurrentHp -= GameManager.Instance.Player.GetComponent<PlayerController>().HitDamageCalculation(BulletDamage);
            _monsterStatus.BulletPool.Free(this.gameObject);
        }
    }
}
