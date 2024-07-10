using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

/*
 * File     : Category.cs
 * Desc     : ScriptablObject
 *            퀘스트 분류의 목적으로 다용도로 사용될 Category
 * Date     : 2024-06-20
 * Writer   : 정지훈
 */

[CreateAssetMenu(menuName = "ScriptableObjects/Quest/Category", fileName = "Category_")]
public class Category : ScriptableObject, IEquatable<Category>
{
    [SerializeField]
    private string _codeName;
    [SerializeField]
    private string _displayName;
    [SerializeField]
    private Color _titleColor;

    public string CodeName => _codeName;
    public string DisplayName => _displayName;
    public Color TitleColor => _titleColor;

    #region Operator
    public bool Equals(Category other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        if (this.GetType() != other.GetType())
        {
            return false;
        }

        return (this._codeName == other.CodeName);
    }

    public override int GetHashCode() => (CodeName, DisplayName).GetHashCode();

    public override bool Equals(object other) => Equals(other as Category);

    public static bool operator ==(Category lhs, string rhs)
    {
        if (lhs is null)
        {
            return ReferenceEquals(rhs, null);
        }

        return ((lhs.CodeName == rhs) || (lhs.DisplayName == rhs));
    }

    public static bool operator !=(Category lhs, string rhs) => !(lhs == rhs);
    #endregion
}
