using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File     : NpcOwnedStates.cs
 * Desc     : Npc의 상태 클래스들을 정의
 * Date     : 2024-05-30
 * Writer   : 정지훈
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
                // quest, 상점 일때 ui 띄우기 switch 문으로
                // 퀘스트가 존재하면 머ㄹ ㅣ위에 느낌표 완료하면 물음표
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
