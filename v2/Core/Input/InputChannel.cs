using System;

namespace TBM.MaouSuika.Core.Input
{
    [Flags]
    public enum InputChannel
    {
        None = 0,
        UI = 1 << 0,
        ShooterAim = 1 << 1,
        ShooterFire = 1 << 2,

        Skill = 1 << 3,

        Shooter = ShooterAim | ShooterFire,
        All = UI | ShooterAim | ShooterFire | Skill,
    }
}