using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File     : EnumTypes.cs
 * Desc     : ���� ������
 * Date     : 2024-06-30
 * Writer   : ������
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
        public static readonly string ActiveQuestDialogue = "�� �� �����ֽ� ���ֳ���?";
        public static readonly string ConditionQuestDialogue = "����Ʈ ������ �� �ִ� ���°� �ƴմϴ�.";
        public static readonly string StartQuestDialogue = "���� �ּż� �����մϴ�.";
        public static readonly string NoMoreQuestDialouge = "�� �̻� �����ֽ� ����Ʈ�� �����ϴ�.";
        public static readonly string CompleteQuestDialouge = "�����ֽô��� ����ϼ̽��ϴ�.";
    }
}
