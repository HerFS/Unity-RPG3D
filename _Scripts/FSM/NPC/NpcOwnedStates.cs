using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File     : NpcOwnedStates.cs
 * Desc     : Npc�� ���� Ŭ�������� ����
 * Date     : 2024-05-30
 * Writer   : ������
 */

namespace NpcOwnedStates
{
    public class Idle : StateOfPlay<NpcEntity>
    {
        public override void Enter(NpcEntity entity)
        {
            entity.Animator.CrossFade(Globals.AnimationName.Idle, 0f);
        }

        public override void Execute(NpcEntity entity)
        {

        }

        public override void Exit(NpcEntity entity)
        {

        }
    }

    public class Talk : StateOfPlay<NpcEntity>
    {
        public override void Enter(NpcEntity entity)
        {
            entity.Animator.CrossFade(Globals.AnimationName.Talk, 0f);
        }

        public override void Execute(NpcEntity entity)
        {
            if (entity.DialogSystem.UpdateDialog())
            {
                // quest, ���� �϶� ui ���� switch ������
                // ����Ʈ�� �����ϸ� �Ӥ� ������ ����ǥ �Ϸ��ϸ� ����ǥ
                entity.ChangeState(EnumTypes.NpcState.Idle);
            }
        }

        public override void Exit(NpcEntity entity)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            PlayerEntity.ChangeState(EnumTypes.PlayerState.Idle);
        }
    }
}
