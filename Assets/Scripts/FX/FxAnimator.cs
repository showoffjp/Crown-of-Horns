using UnityEngine;

namespace SunderedCrown.FX
{
    /// <summary>
    /// Plays a sprite-frame animation on a SpriteRenderer, then destroys itself (unless
    /// looping). Used for one-shot combat VFX (impacts, spell bursts). Frame sprites are
    /// supplied by FxSystem, which loads them from Resources so no inspector wiring is
    /// needed.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class FxAnimator : MonoBehaviour
    {
        public Sprite[] frames;
        public float fps = 16f;
        public bool loop = false;

        private SpriteRenderer _sr;
        private float _t;

        void Awake()
        {
            _sr = GetComponent<SpriteRenderer>();
            _sr.sortingOrder = 1000; // draw VFX above units
        }

        void Start()
        {
            if (frames == null || frames.Length == 0) { Destroy(gameObject); return; }
            _sr.sprite = frames[0];
        }

        void Update()
        {
            if (frames == null || frames.Length == 0) return;
            _t += Time.deltaTime * fps;
            int i = (int)_t;
            if (i >= frames.Length)
            {
                if (loop) { _t = 0f; i = 0; }
                else { Destroy(gameObject); return; }
            }
            _sr.sprite = frames[Mathf.Clamp(i, 0, frames.Length - 1)];
        }
    }
}
