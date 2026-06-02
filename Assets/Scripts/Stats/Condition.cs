namespace SunderedCrown.Stats
{
    /// <summary>
    /// 5e-style conditions a creature can be under. Mechanical consequences are
    /// described by StatusEffectDefinition assets, not hard-coded here — this enum
    /// is just the vocabulary (and lets UI show the right icon/label).
    /// </summary>
    public enum Condition
    {
        None,
        Poisoned,
        Prone,
        Stunned,
        Incapacitated,
        Restrained,
        Blinded,
        Frightened,
        Charmed,
        Burning,     // setting-specific: ashfire damage-over-time
        Blessed,     // beneficial: +to-hit
        Hasted,      // beneficial: extra movement
        Slowed
    }
}
