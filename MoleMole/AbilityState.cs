namespace MoleMole
{
    using System;

    [Flags]
    public enum AbilityState
    {
        AttackSpeedDown = 0x400,
        AttackSpeedUp = 0x8000,
        Bleed = 8,
        BlockAnimEventAttack = 0x800000,
        Burn = 0x40,
        Count = 0x1b,
        CritUp = 0x40000,
        Endure = 0x2000,
        Fragile = 0x1000,
        Frozen = 0x100,
        Immune = 0x80000,
        Invincible = 1,
        Limbo = 2,
        MaxMoveSpeed = 0x100000,
        MoveSpeedDown = 0x200,
        MoveSpeedUp = 0x4000,
        None = 0,
        Paralyze = 0x20,
        Poisoned = 0x80,
        PowerUp = 0x10000,
        ReflectBullet = 0x2000000,
        Shielded = 0x20000,
        SlowWhenFrozenOrParalyze = 0x4000000,
        Stun = 0x10,
        TargetLocked = 0x200000,
        Tied = 0x400000,
        Undamagable = 0x1000000,
        Weak = 0x800,
        WitchTimeSlowed = 4
    }
}

