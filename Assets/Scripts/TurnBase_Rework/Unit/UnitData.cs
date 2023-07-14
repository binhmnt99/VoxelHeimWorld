using System.Collections.Generic;
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitData", menuName = "ScriptableObjects/Unit Data", order = 1)]
public class UnitData : ScriptableObject
{
    [SerializeField] private List<UnitStat> unitStats;
    [SerializeField] private List<UnitAbility> unitAbilities;

    public UnitStat GetStat(int id)
    {
        return unitStats.Find(stat => stat.id == id);
    }

    public UnitAbility GetAbility(int id)
    {
        return unitAbilities.Find(ability => ability.id == id);
    }
}
[Serializable]
public struct UnitStat : IEquatable<UnitStat>
{
    public int id;
    public string statName;
    public float value;

    public UnitStat(int _id, string _statName, float _value)
    {
        this.id = _id;
        this.statName = _statName;
        this.value = _value;
    }

    public override bool Equals(object obj)
    {
        return obj is UnitStat unitStat &&
        id == unitStat.id &&
        statName == unitStat.statName &&
        value == unitStat.value;
    }

    // override object.GetHashCode
    public override int GetHashCode()
    {
        return HashCode.Combine(id, statName, value);
    }

    public bool Equals(UnitStat other)
    {
        return this == other;
    }

    public override string ToString()
    {
        return $"id: {id};stat: {statName}; value: {value};";
    }

    public static bool operator ==(UnitStat a, UnitStat b)
    {
        return a.id == b.id && a.statName == b.statName && a.value == b.value;
    }

    public static bool operator !=(UnitStat a, UnitStat b)
    {
        return !(a == b);
    }
}
[Serializable]
public struct UnitAbility : IEquatable<UnitAbility>
{
    public int id;
    public string abilityName;
    public int abilityCost;
    public List<UnitStat> abilityStats;

    public UnitStat GetStat(int id)
    {
        return abilityStats.Find(stat => stat.id == id);
    }

    public UnitAbility(int _id, string _abilityName, int _abilityCost, List<UnitStat> _abilityStats)
    {
        this.id = _id;
        this.abilityName = _abilityName;
        this.abilityCost = _abilityCost;
        this.abilityStats = _abilityStats;
    }

    public override bool Equals(object obj)
    {
        return obj is UnitAbility unitAbility &&
        id == unitAbility.id &&
        abilityName == unitAbility.abilityName &&
        abilityCost == unitAbility.abilityCost &&
        abilityStats == unitAbility.abilityStats;
    }

    // override object.GetHashCode
    public override int GetHashCode()
    {
        return HashCode.Combine(id, abilityName, abilityCost, abilityStats);
    }

    public bool Equals(UnitAbility other)
    {
        return this == other;
    }

    public override string ToString()
    {
        return $"id: {id}; ability: {abilityName}; cost: {abilityCost};";
    }

    public static bool operator ==(UnitAbility a, UnitAbility b)
    {
        return a.id == b.id && a.abilityName == b.abilityName && a.abilityCost == b.abilityCost && a.abilityStats == b.abilityStats;
    }

    public static bool operator !=(UnitAbility a, UnitAbility b)
    {
        return !(a == b);
    }
}
