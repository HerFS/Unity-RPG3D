using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File     : PlayerOwnedStates.cs
 * Desc     : Player의 상태 클래스들을 정의
 * Date     : 2024-06-30
 * Writer   : 정지훈
 */

namespace PlayerOwnedStates
{
    public class Idle : StateOfPlay<PlayerEntity>
    {
        public override void Enter(PlayerEntity entity)
        {
            entity.Animator.CrossFade(Globals.AnimationName.Idle, 0.2f);
        }

        public override void Execute(PlayerEntity entity)
        {
            if (Input.GetMouseButtonDown((int)EnumTypes.MouseButton.Left) &&
                !Cursor.visible &&
                GameManager.Instance.IsGameStart)
            {
                PlayerEntity.ChangeState(EnumTypes.PlayerState.Attack);
            }

            if (InputManager.Instance.MoveVector.magnitude != 0f)
            {
                PlayerEntity.ChangeState(EnumTypes.PlayerState.Walk);
            }
        }

        public override void Exit(PlayerEntity entity)
        {
            
        }
    }

    public class Talk : StateOfPlay<PlayerEntity>
    {
        public override void Enter(PlayerEntity entity)
        {
            entity.Animator.CrossFade(Globals.AnimationName.Idle, 0.2f);
            InputManager.Instance.gameObject.SetActive(false);
        }

        public override void Execute(PlayerEntity entity)
        {

        }

        public override void Exit(PlayerEntity entity)
        {
            InputManager.Instance.gameObject.SetActive(true);
        }
    }

    public class Walk : StateOfPlay<PlayerEntity>
    {
        public override void Enter(PlayerEntity entity)
        {
            entity.Animator.CrossFade(Globals.AnimationName.Walk, 0f);
        }

        public override void Execute(PlayerEntity entity)
        {
            if (Input.GetMouseButtonDown((int)EnumTypes.MouseButton.Left) &&
                !Cursor.visible &&
                GameManager.Instance.IsGameStart)
            {
                PlayerEntity.ChangeState(EnumTypes.PlayerState.Attack);
            }

            if (InputManager.Instance.MoveVector.magnitude == 0f)
            {
                PlayerEntity.ChangeState(EnumTypes.PlayerState.Idle);
            }
            else if (!(entity.IsHausted) && InputManager.Instance.IsRun)
            {
                PlayerEntity.ChangeState(EnumTypes.PlayerState.Run);
            }
        }

        public override void Exit(PlayerEntity entity)
        {

        }
    }

    public class Run : StateOfPlay<PlayerEntity>
    {
        public override void Enter(PlayerEntity entity)
        {
            entity.Animator.CrossFade(Globals.AnimationName.Run, 0.1f);
        }

        public override void Execute(PlayerEntity entity)
        {
            if (Input.GetMouseButtonDown((int)EnumTypes.MouseButton.Left) &&
                !Cursor.visible && 
                GameManager.Instance.IsGameStart)
            {
                PlayerEntity.ChangeState(EnumTypes.PlayerState.Attack);
            }

            if (InputManager.Instance.MoveVector.magnitude == 0f)
            {
                PlayerEntity.ChangeState(EnumTypes.PlayerState.Idle);
            }
            else if (entity.IsHausted || !InputManager.Instance.IsRun)
            {
                PlayerEntity.ChangeState(EnumTypes.PlayerState.Walk);
            }
        }

        public override void Exit(PlayerEntity entity)
        {

        }
    }

    public class Attack : StateOfPlay<PlayerEntity>
    {
        public override void Enter(PlayerEntity entity)
        {
            DataManager.Instance.Equipment.WeaponSlot.IconImage.raycastTarget = false;

            if (DataManager.Instance.Equipment.WeaponSlot.IsEquipped && entity.Animator.GetCurrentAnimatorStateInfo(1).IsName("Empty")) // 무기 장착
            {
                entity.Animator.CrossFade(Globals.AnimationName.Attack + (entity.AttackCount + 1), 0f);
                AttackCountCalculation(entity);
            }
            else if (entity.Animator.GetCurrentAnimatorStateInfo(1).IsName(Globals.AnimationName.Empty))// 무기 장착 X
            {
                entity.Animator.CrossFade(Globals.AnimationName.Punch + (entity.AttackCount + 1), 0f);
                AttackCountCalculation(entity);
            }
        }

        public override void Execute(PlayerEntity entity)
        {
            if (InputManager.Instance.MoveVector.magnitude == 0f)
            {
                PlayerEntity.ChangeState(EnumTypes.PlayerState.Idle);
            }
            else if (InputManager.Instance.IsRun)
            {
                PlayerEntity.ChangeState(EnumTypes.PlayerState.Run);
            }
            else
            {
                PlayerEntity.ChangeState(EnumTypes.PlayerState.Walk);
            }
        }
        public override void Exit(PlayerEntity entity)
        {
            DataManager.Instance.Equipment.WeaponSlot.IconImage.raycastTarget = true;
        }
        private void AttackCountCalculation(PlayerEntity entity)
        {
            if (entity.AttackCount >= 2)
            {
                entity.AttackCount = 0;
            }
            else
            {
                entity.AttackCount++;
            }
        }
    }

    public class Die : StateOfPlay<PlayerEntity>
    {
        private float _timer;
        private float _animationTime;

        public override void Enter(PlayerEntity entity)
        {
            _animationTime = entity.DieAnimation.length;
            entity.Animator.CrossFade(Globals.AnimationName.Died, 0f);
            entity.Animator.CrossFade(Globals.AnimationName.Empty, 0f);

            GameManager.Instance.ShowMouse();
            InputManager.Instance.gameObject.SetActive(false);
        }

        public override void Execute(PlayerEntity entity)
        {
            _timer += Time.deltaTime;

            if (_timer >= _animationTime)
            {
                Time.timeScale = 0f;

                if (!UIManager.Instance.DiedPanel.gameObject.activeSelf)
                {
                    UIManager.Instance.DiedPanel.gameObject.SetActive(true);
                }
                _timer = 0f;
            }
        }

        public override void Exit(PlayerEntity entity)
        {
            entity.Animator.CrossFade(Globals.AnimationName.Idle, 0f);
                        UIManager.Instance.BloodUI.gameObject.SetActive(false);
        }
    }
}
