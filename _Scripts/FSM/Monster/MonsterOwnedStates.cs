using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File     : MonsterOwnedStates.cs
 * Desc     : Monster의 상태 클래스들을 정의
 * Date     : 2024-06-23
 * Writer   : 정지훈
 */

namespace MonsterOwnedStates
{
    public class Idle : StateOfPlay<MonsterEntity>
    {
        private readonly float _watchTime = 3f;
        private readonly float _rotateSpeed = 15f;

        private float _currentRotataionValue;
        private float _rotateTimer;
        private float _timer;
        private float _turnRotation; // if this == 0 => left, else right
        private bool _isTurn;

        public override void Enter(MonsterEntity entity)
        {
            // 데미지를 맞으면 chasing 상태로 변경
            // 데미지를 맞거나 field of view 에 플레이어를 감지하면 chasing 으로 변경
            entity.Animator.CrossFade(Globals.AnimationName.Empty, 0f);
            entity.Animator.CrossFade(Globals.AnimationName.Idle, 0f);

            _currentRotataionValue = entity.transform.eulerAngles.y;
            _turnRotation = Random.Range(0, 3);
        }

        public override void Execute(MonsterEntity entity)
        {
            _timer += Time.deltaTime;
            _rotateTimer += Time.deltaTime * _rotateSpeed;

            if (_isTurn)
            {
                if (_turnRotation == 1)
                {
                    entity.transform.rotation = Quaternion.Euler(0f, _currentRotataionValue + _rotateTimer, 0f);
                }
                else
                {
                    entity.transform.rotation = Quaternion.Euler(0f, _currentRotataionValue - _rotateTimer, 0f);
                }

                entity.MonsterStatus.MonsterFieldOfView.SettingFieldOfView(entity.transform.eulerAngles.y);
            }

            if (_timer >= _watchTime)
            {
                _timer = 0;
                _rotateTimer = 0f;

                if (_isTurn)
                {
                    _isTurn = false;
                }
                else
                {
                    _isTurn = true;
                }

                Enter(entity);
            }
        }

        public override void Exit(MonsterEntity entity)
        {
            _isTurn = true;
        }
    }

    public class Walk : StateOfPlay<MonsterEntity>
    {
        private Transform _entitiySpawnPoint;
        private Vector3 _entitySpawnPos;
        public override void Enter(MonsterEntity entity)
        {
            // 랜덤 포지션 지정한 곳으로 이동
            entity.Animator.CrossFade(Globals.AnimationName.Walk, 0f);
            entity.Animator.CrossFade(Globals.AnimationName.Empty, 0f);

            _entitiySpawnPoint = entity.SpawnPoint;
            entity.PathFinding.Target = _entitiySpawnPoint;
            _entitySpawnPos = new Vector3(_entitiySpawnPoint.position.x, entity.transform.position.y, _entitiySpawnPoint.position.z);

            entity.Rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        }

        public override void Execute(MonsterEntity entity)
        {
            // 플레이어가 trigger 나가거나. field of view 에 player 감지가 사라졌을 때, 몬스터 영역 벗어났을 때
            // 데미지를 맞거나 field of view 에 플레이어를 감지하면 chasing 으로 변경
            entity.transform.LookAt(_entitySpawnPos);
            entity.PathFinding.UpdatePath();
            // 스폰지역으로 이동

            if (entity.PathFinding.Path.Count <= 1)
            {
                entity.ChangeState(EnumTypes.MonsterState.Idle);
            }
            else if (entity.PathFinding.Path.Count != 0)
            {
                Vector3 targetPos = new Vector3(entity.PathFinding.Path[0].NodePosition.x, entity.transform.position.y, entity.PathFinding.Path[0].NodePosition.z);
                Vector3 currentPos = entity.transform.position;

                float distance = Vector3.Distance(currentPos, targetPos);
                if (distance > 0.1f)
                {
                    Vector3 directionOfPath = targetPos - currentPos;
                    directionOfPath.Normalize();
                    entity.Rigidbody.MovePosition(entity.transform.position + (directionOfPath * entity.MonsterStatus.WalkSpeed * Time.deltaTime));
                }
            }

            entity.MonsterStatus.MonsterFieldOfView.SettingFieldOfView(entity.transform.eulerAngles.y);
        }

        public override void Exit(MonsterEntity entity)
        {
            entity.Rigidbody.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
        }
    }

    public class Chasing : StateOfPlay<MonsterEntity>
    {
        public override void Enter(MonsterEntity entity)
        {
            entity.Animator.CrossFade(Globals.AnimationName.Walk, 0.2f);
            entity.PathFinding.Target = GameManager.Instance.Player;
            entity.Rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        }

        public override void Execute(MonsterEntity entity)
        {
            // 플레이어를 쳐다보고 쫓아감
            // 플레이어와 근접하면 attack 으로 변경
            entity.Distance = Vector3.Distance(entity.transform.position, entity.Target.position);

            entity.transform.LookAt(new Vector3(GameManager.Instance.Player.position.x, entity.transform.position.y, GameManager.Instance.Player.position.z));
            entity.PathFinding.UpdatePath();

            if (entity.Distance <= entity.MonsterStatus.AttackRange)
            {
                entity.ChangeState(EnumTypes.MonsterState.Attack);
            }
            else
            {
                if (entity.PathFinding.Path.Count <= 1)
                {
                    entity.ChangeState(EnumTypes.MonsterState.Attack);
                }
                else if (entity.PathFinding.Path.Count != 0)
                {
                    Vector3 targetPos = new Vector3(entity.PathFinding.Path[0].NodePosition.x, entity.transform.position.y, entity.PathFinding.Path[0].NodePosition.z);
                    Vector3 currentPos = entity.transform.position;

                    float distance = Vector3.Distance(currentPos, targetPos);
                    if (distance > 0.1f)
                    {
                        Vector3 directionOfPath = targetPos - currentPos;
                        directionOfPath.Normalize();
                        entity.Rigidbody.MovePosition(entity.transform.position + (directionOfPath * entity.MonsterStatus.WalkSpeed * Time.deltaTime));
                    }
                }

                entity.MonsterStatus.MonsterFieldOfView.SettingFieldOfView(entity.transform.eulerAngles.y);
            }
        }

        public override void Exit(MonsterEntity entity)
        {
            entity.Rigidbody.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
        }
    }

    public class Attack : StateOfPlay<MonsterEntity>
    {
        private float _timer;
        public override void Enter(MonsterEntity entity)
        {
            entity.Animator.CrossFade(Globals.AnimationName.Idle, 0f);
            entity.Animator.CrossFade(Globals.AnimationName.Attack, 0f);

            entity.Animator.SetFloat(entity.AttackSpeed, entity.MonsterStatus.AttackSpeed);
            entity.Rigidbody.constraints = RigidbodyConstraints.FreezeAll;
            if (entity.MonsterStatus.WeaponCollider)
            {
                entity.MonsterStatus.WeaponCollider.enabled = true;
            }
        }

        public override void Execute(MonsterEntity entity)
        {
            _timer += Time.deltaTime;

            entity.transform.LookAt(new Vector3(GameManager.Instance.Player.position.x, entity.transform.position.y, GameManager.Instance.Player.position.z));
            entity.MonsterStatus.MonsterFieldOfView.SettingFieldOfView(entity.transform.eulerAngles.y);
            entity.Distance = Vector3.Distance(entity.transform.position, entity.Target.position);

            if (_timer > (entity.AttackAnimation.length / entity.MonsterStatus.AttackSpeed))
            {
                _timer = 0f;
                if (entity.Distance >= entity.MonsterStatus.AttackRange)
                {
                    entity.ChangeState(EnumTypes.MonsterState.Chasing);
                }
                else
                {
                    Enter(entity);
                }
            }
        }

        public override void Exit(MonsterEntity entity)
        {
            entity.Animator.CrossFade(Globals.AnimationName.Empty, 0f);
            if (entity.MonsterStatus.WeaponCollider)
            {
                entity.MonsterStatus.WeaponCollider.enabled = false;
            }
        }
    }

    public class Die : StateOfPlay<MonsterEntity>
    {
        private readonly float _destroyTime = 2f;
        private float _timer;
        private float _animationTime;

        public override void Enter(MonsterEntity entity)
        {
            entity.onDead.Invoke();
            _animationTime = entity.DieAnimation.length;
            entity.Rigidbody.constraints = RigidbodyConstraints.FreezeAll;
            entity.Animator.CrossFade(Globals.AnimationName.Died, 0f);
            entity.Animator.CrossFade(Globals.AnimationName.Empty, 0f);
            entity.HideInfoUI();
        }

        public override void Execute(MonsterEntity entity)
        {
            _timer += Time.deltaTime;

            if (_timer >= _animationTime)
            {
                if (_timer >= (_animationTime + _destroyTime))
                {
                    entity.MonsterSpawner.CurrentMonsterSpawnCount--;
                    entity.MonsterSpawner.MonsterPool.Free(entity.gameObject);
                    _timer = 0f;
                }
            }
        }

        public override void Exit(MonsterEntity entity)
        {

        }
    }
}
