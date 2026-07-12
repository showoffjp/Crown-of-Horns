namespace SunderedCrown.Combat
{
    /// <summary>The outcome of a single death save: the updated running tally plus any terminal flag.</summary>
    public struct DeathSaveResult
    {
        public int successes;   // running success count after this roll
        public int failures;    // running failure count after this roll
        public bool revived;    // a natural 20 — the creature snaps back to consciousness (1 HP)
        public bool dead;       // third failure — permanently dead
        public bool stable;     // third success — unconscious but no longer dying
    }

    /// <summary>
    /// 5e / BG3 death saves. PURE and engine-agnostic so it unit-tests headless with no scene:
    /// given the running {successes, failures} tally and a d20 roll, return the next state.
    /// 20 → revive (back up at 1 HP); 1 → two failures; 10+ → a success; else a failure.
    /// Three successes → stable; three failures → dead. Mirrors the browser slice's deathSaveStep,
    /// which is pinned by play/downed.test.js.
    /// </summary>
    public static class DeathSaves
    {
        public static DeathSaveResult Step(int successes, int failures, int roll)
        {
            int s = successes, f = failures;
            bool revived = false, dead = false, stable = false;

            if (roll >= 20)
            {
                revived = true; s = 0; f = 0;
            }
            else
            {
                if (roll <= 1) f += 2;
                else if (roll >= 10) s += 1;
                else f += 1;

                if (f >= 3) { dead = true; f = 3; }
                else if (s >= 3) { stable = true; s = 3; }
            }

            return new DeathSaveResult { successes = s, failures = f, revived = revived, dead = dead, stable = stable };
        }
    }
}
