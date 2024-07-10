using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File     : IDamageable.cs
 * Desc     : 데미지를 받는 엔티티 인터페이스
 * Date     : 2024-05-03
 * Writer   : 정지훈
 */

public interface IDamageable
{
    IEnumerator Hit();
    float HitDamageCalculation(float damage);
}
