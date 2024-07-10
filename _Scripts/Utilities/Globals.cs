using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File     : EnumTypes.cs
 * Desc     : 전역 변수들
 * Date     : 2024-06-30
 * Writer   : 정지훈
 */

public static class Globals
{
    public const int EquipmentSlotCount = 7;
    public static class TagName
    {
        public static readonly string Item = "Itme";
        public static readonly string Player = "Player";
        public static readonly string Monster = "Monster";
        public static readonly string Weapon = "Weapon";
        public static readonly string MonsterWeapon = "MonsterWeapon";
        public static readonly string MonsterBullet = "MonsterBullet";
    }

    public static class CursorPath
    {
        public static readonly string Hand = "UI/Cursor/Hand";
        public static readonly string Original = "UI/Cursor/Original";
    }

    public static class AnimationName
    {
        public static readonly string Idle = "Idle";
        public static readonly string Talk = "Talk";
        public static readonly string Walk = "Walk";
        public static readonly string Run = "Run";
        public static readonly string Punch = "Punch";
        public static readonly string Attack = "Attack";
        public static readonly string Empty = "Empty";
        public static readonly string Died = "Died";
    }

    public static class NpcDialogue
    {
        public static readonly string ActiveQuestDialogue = "저 좀 도와주실 수있나요?";
        public static readonly string ConditionQuestDialogue = "퀘스트 수락할 수 있는 상태가 아닙니다.";
        public static readonly string StartQuestDialogue = "도와 주셔서 감사합니다.";
        public static readonly string NoMoreQuestDialouge = "더 이상 도와주실 퀘스트가 없습니다.";
        public static readonly string CompleteQuestDialouge = "도와주시느라 고생하셨습니다.";
    }
}
