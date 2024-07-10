/*
 * File     : EnumTypes.cs
 * Desc     : 모든 enum들의 모음
 * Date     : 2024-06-30
 * Writer   : 정지훈
 */

namespace EnumTypes
{
    public enum PlayerState
    {
        Idle = 0,
        Talk,
        Walk,
        Run,
        Attack,
        Die
    }

    public enum PlayerAttackDirection
    {
        Right = 0,
        Left
    }

    public enum PlayerCustomize
    {
        Hair = 0,
        Head,
        Beard
    }

    public enum NpcState
    {
        Idle = 0,
        Talk
    }

    public enum NpcType
    {
        Default = 0,
        Quest,
        Shop
    }

    public enum MonsterState
    {
        Idle = 0,
        Walk,
        Chasing,
        Attack,
        Die
    }

    public enum MonsterAttackType
    {
        Default = 0,
        MeleeAttak,
        RangeAttak,
    }

    public enum SceneName
    {
        StartScene,
        LoadingScene,
        Village,
        MainMenu,
        GrassLand,
        Winter,
    }

    public enum ItemType
    {
        Armor = 0,
        Weapon,
        Consumption,
        Default,
    }

    public enum EquipmentType
    {
        Default = 0,
        Helmet,
        Top,
        Bottom,
        Shoes,
        Weapon,
        Gauntlets,
        Cloak
    }

    public enum MouseButton
    {
        Left = 0,
        Right = 1
    }

    public enum LayerIndex
    {
        Player = 3,
        Obstruction = 6,
        Ground,
        Monster,
        Unwalkable
    }
}
