using UnityEngine;
using SunderedCrown.World;

namespace SunderedCrown.Rendering
{
    /// <summary>
    /// Gives world markers — NPCs, chests, doors, stairways, braziers — actual art.
    /// <para>
    /// Every interactable is built by a MakeMarker-style factory as a tinted
    /// <c>PrimitiveType.Cube</c>, which is why the world read as coloured
    /// rectangles. Three art sets replace them, chosen by what the marker *is*:
    /// </para>
    /// <list type="bullet">
    /// <item>people get a <see cref="WorldArt.Standee"/> — the portrait's figure
    /// cut out on transparency, not the opaque portrait card, which stood on a
    /// floor tile only ever looked like a framed picture hovering in mid-air;</item>
    /// <item>objects get a painted prop from <see cref="PropArt"/>, matched off
    /// the marker's own player-facing label ("Ash-Caked Chest" → chest);</item>
    /// <item>anything with neither keeps its tinted cube, so a marker this does
    /// not understand degrades instead of vanishing.</item>
    /// </list>
    /// <para>
    /// Applied from <c>Interactable.Start</c> rather than <c>Place</c>: the marker
    /// factories call <c>Place</c> before they have set <c>kind</c>, <c>lootFlag</c>
    /// or the dialogue, so art chosen there could not tell a chest from a door.
    /// </para>
    /// </summary>
    public static class MarkerArt
    {
        /// World height, in units, for a person. One floor tile is 1.0 wide by 0.5
        /// high, so a person stands about two tile-heights tall.
        private const float PersonHeight = 1.02f;
        /// Nothing is allowed to be wider than this, whatever its height works out
        /// to. A market stall and a notice board are painted wide and short; scaled
        /// on height alone they came out three tiles across and buried the room.
        private const float MaxWidth = 1.15f;
        /// How far below the marker's centre the art's feet sit.
        private const float FootDrop = -0.12f;

        private const string ArtChild = "Art";
        private const string ShadowChild = "Shadow";

        private static Sprite _shadow;

        public static void Apply(GameObject go, string label) =>
            Apply(go, label, InteractionKind.Talk, false);

        public static void Apply(GameObject go, string label, InteractionKind kind, bool looted)
        {
            if (go == null || string.IsNullOrEmpty(label)) return;

            var sprite = Resolve(label, kind, looted, out float height);
            if (sprite == null) return;                 // no art: keep the tinted cube

            // Re-skinning is legitimate — a chest that has just been opened swaps to
            // its open art — so clear any previous art rather than bailing out.
            Strip(go, ArtChild);
            Strip(go, ShadowChild);

            // Hide the placeholder mesh but keep the collider, so clicks and
            // "walk up and press E" keep working exactly as before.
            var mr = go.GetComponent<MeshRenderer>();
            if (mr != null) mr.enabled = false;

            AddShadow(go, height);

            var art = new GameObject(ArtChild);
            art.transform.SetParent(go.transform, false);
            art.transform.localPosition = new Vector3(0f, FootDrop, -0.02f);

            var sr = art.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;
            sr.sortingOrder = 10;
            // Nothing in this world is lit geometry — the grid is a flat XY plane
            // viewed head-on — so a cast shadow only ever smeared a long streak
            // across the floor. The painted blob below does the job properly.
            sr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            sr.receiveShadows = false;

            // Standees and props are painted at different pixel sizes; normalise
            // them to a world height so a crowd doesn't come out ragged.
            float h = sprite.bounds.size.y;
            float w = sprite.bounds.size.x;
            if (h > 0.0001f && w > 0.0001f)
            {
                float s = Mathf.Min(height / h, MaxWidth / w);
                art.transform.localScale = new Vector3(s, s, s);
            }

            art.AddComponent<CameraBillboard>();
            // Sort by the tile the marker stands on, not by where its art is lifted
            // to: a standee's feet sit below its pivot, and half a row of drift is
            // enough to make it draw through a wall one row nearer the camera.
            var depth = art.AddComponent<IsoDepthSorter>();
            depth.target = sr;
            depth.basis = go.transform;
        }

        /// <summary>Picks the art and the height to draw it at.</summary>
        private static Sprite Resolve(string label, InteractionKind kind, bool looted, out float height)
        {
            height = PersonHeight;

            // A container is a container whatever its label says, so it always gets
            // a chest — open once it has been emptied, which is also how a chest
            // looted in an earlier visit reads as already looted on the way back.
            if (kind == InteractionKind.Container)
            {
                var pc = PropArt.Match(label);
                string name = pc.Valid ? pc.Name : "chest";
                if (name == "chest" && looted) name = "chest_open";
                height = pc.Valid ? pc.Height : 0.80f;
                var chest = PropArt.Load(name);
                if (chest != null) return chest;
            }

            // People: a talkable marker is a soul, so try its standee first — the
            // label is the exact display name the portrait fleet is keyed by.
            if (kind == InteractionKind.Talk)
            {
                var soul = WorldArt.Standee(label);
                if (soul != null) return soul;
            }

            var prop = PropArt.Match(label);
            if (prop.Valid)
            {
                var art = PropArt.Load(prop.Name);
                if (art != null) { height = prop.Height; return art; }
            }

            // An examine/exit marker can still name a soul ("Maerin (freed)").
            var fallback = WorldArt.Standee(label);
            if (fallback != null) return fallback;

            return null;
        }

        private static void Strip(GameObject go, string child)
        {
            var t = go.transform.Find(child);
            if (t != null) Object.Destroy(t.gameObject);
        }

        /// <summary>A soft blob under the art, so it sits on the tile instead of
        /// floating above it. Built from a generated texture — one asset-free
        /// radial falloff shared by every marker in the game.</summary>
        private static void AddShadow(GameObject go, float height)
        {
            var sprite = ShadowSprite();
            if (sprite == null) return;

            var sh = new GameObject(ShadowChild);
            sh.transform.SetParent(go.transform, false);
            sh.transform.localPosition = new Vector3(0f, FootDrop + 0.02f, 0.04f);

            var sr = sh.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;
            sr.color = new Color(0f, 0f, 0f, 0.42f);
            sr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            sr.receiveShadows = false;
            // One step behind its own art in the same iso sort, so it never lands
            // in front of a marker standing further down the screen.
            var sort = sh.AddComponent<IsoDepthSorter>();
            sort.target = sr;
            sort.basis = go.transform;
            sort.offset = -1;

            // Wider for a taller thing, and squashed to the floor's 2:1 projection.
            float w = Mathf.Clamp(0.30f + height * 0.34f, 0.34f, 0.92f);
            sh.transform.localScale = new Vector3(w, w * 0.5f, 1f);
        }

        private static Sprite ShadowSprite()
        {
            if (_shadow != null) return _shadow;

            const int N = 64;
            // DontSave keeps it alive across scene loads; every mode transition in this
            // game loads a scene, and a destroyed texture would take the shadows with it.
            var tex = new Texture2D(N, N, TextureFormat.RGBA32, false)
            {
                wrapMode = TextureWrapMode.Clamp,
                hideFlags = HideFlags.DontSave,
            };
            var px = new Color32[N * N];
            float r = N * 0.5f;
            for (int y = 0; y < N; y++)
                for (int x = 0; x < N; x++)
                {
                    float d = Mathf.Sqrt((x - r + 0.5f) * (x - r + 0.5f) + (y - r + 0.5f) * (y - r + 0.5f)) / r;
                    float a = Mathf.Clamp01(1f - d);
                    a = a * a;                                   // soft edge, dense centre
                    px[y * N + x] = new Color32(0, 0, 0, (byte)(a * 255f));
                }
            tex.SetPixels32(px);
            tex.Apply();

            _shadow = Sprite.Create(tex, new Rect(0, 0, N, N), new Vector2(0.5f, 0.5f), N);
            _shadow.hideFlags = HideFlags.DontSave;
            return _shadow;
        }
    }
}
