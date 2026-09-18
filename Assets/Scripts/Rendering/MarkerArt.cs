using UnityEngine;

namespace SunderedCrown.Rendering
{
    /// <summary>
    /// Gives world markers — NPCs, doors, objects — an actual face.
    /// <para>
    /// Every interactable in the world is built by a MakeMarker-style factory as a
    /// tinted <c>PrimitiveType.Cube</c>, which is why the game read as coloured
    /// rectangles. The art to replace them already ships: 309 painted portraits in
    /// <c>Resources/Portraits</c> and 62 battle tokens in <c>Resources/Sprites</c>,
    /// both keyed by display name. <see cref="WorldArt.Portrait"/> already walks
    /// portrait → first word → battle token, so one lookup covers every case.
    /// </para>
    /// <para>
    /// Applied centrally from <c>Interactable.Place</c> so it reaches every marker
    /// in every scene, rather than being repeated in each content file.
    /// </para>
    /// </summary>
    public static class MarkerArt
    {
        /// World height, in units, to draw a marker's art at.
        private const float TargetHeight = 1.15f;

        public static void Apply(GameObject go, string label)
        {
            if (go == null || string.IsNullOrEmpty(label)) return;
            if (go.transform.Find("Art") != null) return;          // already skinned

            var sprite = WorldArt.Portrait(label);                 // portrait → firstWord → token
            if (sprite == null) return;                            // no art: keep the tinted cube

            // Hide the placeholder mesh but keep the collider, so clicks and
            // "walk up and press E" keep working exactly as before.
            var mr = go.GetComponent<MeshRenderer>();
            if (mr != null) mr.enabled = false;

            var art = new GameObject("Art");
            art.transform.SetParent(go.transform, false);
            art.transform.localPosition = new Vector3(0f, 0.45f, 0f);

            var sr = art.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;
            sr.sortingOrder = 10;

            // Portraits are tall busts and tokens are square; normalise both to a
            // consistent world height so a crowd doesn't come out ragged.
            float h = sprite.bounds.size.y;
            if (h > 0.0001f)
            {
                float s = TargetHeight / h;
                art.transform.localScale = new Vector3(s, s, s);
            }

            art.AddComponent<CameraBillboard>();
            art.AddComponent<IsoDepthSorter>().target = sr;
        }
    }
}
