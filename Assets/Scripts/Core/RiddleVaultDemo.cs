using UnityEngine;
using SunderedCrown.Characters;
using SunderedCrown.Content;
using SunderedCrown.Stats;

namespace SunderedCrown.Core
{
    /// <summary>
    /// ONE-CLICK VAULT OF TENS DEMO. Drop on an empty GameObject and press Play to enter the Lady in
    /// the Margins' riddle room with a full pouch of tokens. Walk to a pedestal (E or click), place a
    /// token to answer; walk to a brazier to speak a word. Wit beats correctness — try a *clever* wrong
    /// token. The secret violet brazier asks who *she* is.
    /// </summary>
    public class RiddleVaultDemo : MonoBehaviour
    {
        void Start()
        {
            if (GameManager.Instance == null) new GameObject("GameManager").AddComponent<GameManager>();
            if (Party.Instance == null) new GameObject("Party").AddComponent<Party>();

            // A lone hero to walk the vault.
            var hero = new CharacterSheet { displayName = "The Returned", level = 5, baseArmorClass = 14 };
            hero.abilities.Set(Ability.Dexterity, 14);
            hero.maxHitPoints = 30; hero.currentHitPoints = 30;
            Party.Instance.Recruit(hero);

            var riddles = new RiddleContent();

            var root = new GameObject("VaultMode");
            var vault = root.AddComponent<RiddleVault>();
            vault.content = riddles;
            vault.leaderSheet = hero;
            vault.grantTokens = true;
            vault.onLeave = () => { Destroy(root); Debug.Log("[VaultDemo] You step back into the story. She waves without looking up."); };
            vault.Begin();

            Debug.Log("[VaultDemo] Welcome to the Vault of Tens. Place tokens on the pedestals, speak at the " +
                      "braziers, and try the violet brazier last. Clever-wrong answers earn her amusement.");
        }
    }
}
