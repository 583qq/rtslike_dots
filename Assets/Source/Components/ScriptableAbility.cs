using System;
using System.Collections.Generic;

using Unity;
using UnityEngine;

[CreateAssetMenu(fileName="NewAbility", menuName="Project/Unit/Abilities/Ability", order=1)]
public class ScriptableAbility : ScriptableObject
{
    public Guid id = new Guid();
    public string abilityName;

    public Range rangeDistance;

    public float cooldown;
    public float castTime;
}


[Serializable]
public struct Range
{
    public float min;
    public float max;
}