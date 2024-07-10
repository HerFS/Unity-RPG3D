using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File     : EntitySpawner.cs
 * Desc     : Entity들을 생성 - 수정 필요
 * Date     : 2024-04-30
 * Writer   : 정지훈
 */

[RequireComponent(typeof(SphereCollider), typeof(ObjectPool))]
public class MonsterSpawner : MonoBehaviour
{
    private delegate void MonsterSpawnHandler(MonsterSpawner entitySpawner, int spawnCount);

    [Header("Setting")]
    [Range(0, 30)]
    public int InitialCreationCount;
    [SerializeField]
    private float _monsterRespawnInterval;
    [SerializeField]
    private float _spawnRadius;

    private int _currentMonsterSpawnCount;
    private SphereCollider _spawnRange;

    private event MonsterSpawnHandler onMonsterSpawn;

    [HideInInspector]
    public ObjectPool MonsterPool;
    [field : SerializeField]
    public bool IsPlayerDetected { get; private set; }

    public int CurrentMonsterSpawnCount
    {
        get
        {
            return _currentMonsterSpawnCount;
        }

        set
        {
            _currentMonsterSpawnCount = value;

            if (_currentMonsterSpawnCount != InitialCreationCount)
            {
                onMonsterSpawn?.Invoke(this, _currentMonsterSpawnCount);
            }
        }
    }

    private void Awake()
    {
        _spawnRange = GetComponent<SphereCollider>();
        MonsterPool = GetComponent<ObjectPool>();

        _spawnRange.isTrigger = true;
        _spawnRange.radius = _spawnRadius;

        onMonsterSpawn += (monsterSpawner, monsterCount) =>
        {
            StartCoroutine(MonsterRegen());
        };
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Contains(Globals.TagName.Player))
        {
            IsPlayerDetected = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag.Contains(Globals.TagName.Player))
        {
            IsPlayerDetected = false;
        }
    }

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        for (int i = 0; i < InitialCreationCount; ++i)
        {
            GameObject obj = MonsterPool.Allocate();
            obj.transform.position = ReSpawnPosition();
        }
    }

    private Vector3 ReSpawnPosition()
    {
        Vector3 originPosition = _spawnRange.transform.position;
        float rangeX = _spawnRange.bounds.size.x;
        float rangeZ = _spawnRange.bounds.size.z;

        rangeX = Random.Range(((rangeX / 2) * -0.5f), (rangeX / 2.5f));
        rangeZ = Random.Range(((rangeZ / 2) * -0.5f), (rangeZ / 2.5f));
        Vector3 randomPosition = new Vector3(rangeX, 0f, rangeZ);
        Vector3 respawnPosition = originPosition + randomPosition;

        return respawnPosition;
    }

    private IEnumerator MonsterRegen()
    {
        yield return new WaitForSeconds(_monsterRespawnInterval);

        GameObject obj = MonsterPool.Allocate();
        obj.transform.position = ReSpawnPosition();

        _currentMonsterSpawnCount++;
    }
}
