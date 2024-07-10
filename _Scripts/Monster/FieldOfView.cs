using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File     : FieldOfView.cs
 * Desc     : 몬스터들이 플레이어 감지
 * Date     : 2024-06-16
 * Writer   : 정지훈
 */

[RequireComponent(typeof(SphereCollider))]
public class FieldOfView : MonoBehaviour
{
    public delegate void PlayerDetectChangedHandler(FieldOfView fieldOfView, bool currentPlayerDetection, bool prevPlayerDetection);

    [SerializeField,  Range(0, 20)]
    public float _radius;
    [SerializeField, Range(0, 360)]
    public float _angle;

    [SerializeField]
    private bool _isPlayerDetection;
    private float _theta;
    float _posX;
    float _posZ;

    private Vector3 _forwardDir;
    private Vector3 _rightDir;
    private Vector3 _leftDir;

    private MonsterEntity _monsterEntity;
    public SphereCollider DectectionRadius { get; private set; }
    public float Radius
    {
        get { return _radius; }
        private set { _radius = value; }
    }

    public event PlayerDetectChangedHandler onPlayerDetectChagned;

    private int _targetMask;
    private int _obstructionMask;

    public bool IsPlayerDetection
    {
        get
        {
            return _isPlayerDetection;
        }

        set
        {
            bool prevPlayerDetection = _isPlayerDetection;
            _isPlayerDetection = value;

            if (_isPlayerDetection)
            {
                onPlayerDetectChagned?.Invoke(this, _isPlayerDetection, prevPlayerDetection);
            }
        }
    }

    private void Awake()
    {
        _monsterEntity = GetComponent<MonsterEntity>();
        DectectionRadius = GetComponent<SphereCollider>();

        onPlayerDetectChagned += (fieldOfView, currentPlayerDetection, prevPlayerDetection) =>
        {
            if (currentPlayerDetection &&
            _monsterEntity.StateMachine.CurrentState != _monsterEntity.States[(int)EnumTypes.MonsterState.Die] &&
            _monsterEntity.StateMachine.CurrentState != _monsterEntity.States[(int)EnumTypes.MonsterState.Attack])
            {
                _monsterEntity.ChangeState(EnumTypes.MonsterState.Chasing);
            }
        };
    }

    private void Start()
    {
        DectectionRadius.isTrigger = true;
        DectectionRadius.radius = _radius;

        _targetMask = 1 << (int)EnumTypes.LayerIndex.Player;
        _obstructionMask = 1 << (int)EnumTypes.LayerIndex.Obstruction;

        SettingFieldOfView(transform.eulerAngles.y);
    }

    // need to delete
    private void Update()
    {
        Debug.DrawRay(transform.position, _forwardDir, Color.yellow);
        Debug.DrawRay(transform.position, _rightDir, Color.red);
        Debug.DrawRay(transform.position, _leftDir, Color.red);
    }

    // need to delete
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, _radius);
    }

    public void SettingFieldOfView(float forwardAngle)
    {
        _posX = Mathf.Sin(forwardAngle * Mathf.Deg2Rad);
        _posZ = Mathf.Cos(forwardAngle * Mathf.Deg2Rad);
        _forwardDir = new Vector3(_posX, 0f, _posZ);

        _theta = forwardAngle + _angle;
        _rightDir = DirFromAngle();

        _theta = forwardAngle - _angle;
        _leftDir = DirFromAngle();
    }

    private Vector3 DirFromAngle()
    {
        float posX = Mathf.Sin(_theta * Mathf.Deg2Rad) * _radius;
        float posY = Mathf.Cos(_theta * Mathf.Deg2Rad) * _radius;

        return new Vector3(posX, 0f, posY);
    }

    public void TargetDetection()
    {
        // transform.position 중심으로 _radius 반경 내에서 targetMask 를 가지고 있는 오브젝트 배열로 저장

        Collider[] targets = Physics.OverlapSphere(transform.position, _radius, _targetMask);

        foreach (var target in targets)
        {
            // 타겟 포지션
            Vector3 targetPos = target.transform.position;
            // 타겟과의 거리
            Vector3 targetDir = (targetPos - transform.position).normalized;
            // 타겟과의 각도
            // Dot 는 2개 벡터의 각도를 구하기 => _forwardDir 와 targetDir 사이의 각도
            // Acos 는 Cos을 제거하는 역 메서드 원래 theta는 radian이여서 radian을 degree로 변경
            float targetAngle = Mathf.Acos(Vector3.Dot(_forwardDir, targetDir)) * Mathf.Rad2Deg;

            if (targetAngle <= _angle && !Physics.Raycast(transform.position, targetDir, _radius, _obstructionMask) && _monsterEntity.MonsterSpawner.IsPlayerDetected)
            {
                if (!IsPlayerDetection)
                {
                    IsPlayerDetection = true;
                }

                Debug.DrawLine(transform.position, targetPos, Color.blue);
            }
        }
    }
}
