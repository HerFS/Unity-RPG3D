using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File     : IDamageable.cs
 * Desc     : �������� �޴� ��ƼƼ �������̽�
 * Date     : 2024-05-03
 * Writer   : ������
 */

public interface IDamageable
{
    IEnumerator Hit();
    float HitDamageCalculation(float damage);
}
