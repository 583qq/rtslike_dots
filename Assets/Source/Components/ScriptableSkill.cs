using System;
using System.Collections.Generic;

using Unity;
using UnityEngine;

[CreateAssetMenu(fileName="NewSkill", menuName="Gameplay Feature/Unit/Skills/Skill", order=1)]
public class ScriptableSkill : ScriptableObject
{
    public Guid id = new Guid();
    public string skillName;

    public Range rangeDistance;

    public float cooldown;
    public float castTime;

    // Skill Effects
    [SerializeReference]
    public List<ISkillEffect> effects;

    // Skill Costs

    [SerializeReference]
    public List<ISkillCost> costs;
}

[Serializable]
public struct Range
{
    public float min;
    public float max;
}