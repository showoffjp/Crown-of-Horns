using UnityEngine;

namespace SunderedCrown.CameraRig
{
    /// <summary>
    /// Classic CRPG camera: edge/WASD pan, scroll-wheel zoom, optional rotate.
    /// Works with an Orthographic camera for the crisp 2D-iso look, or a tilted
    /// Perspective camera for 2.5D. Attach to your Main Camera.
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class IsometricCameraController : MonoBehaviour
    {
        [Header("Pan")]
        public float panSpeed = 8f;
        public float edgeScrollMargin = 12f; // pixels from screen edge
        public bool enableEdgeScroll = true;

        [Header("Zoom (orthographic size)")]
        public float zoomSpeed = 4f;
        public float minZoom = 3f;
        public float maxZoom = 12f;

        [Header("Bounds in world XY; zero size = unbounded")]
        public Vector2 minBounds;
        public Vector2 maxBounds;

        private Camera _cam;

        void Awake() => _cam = GetComponent<Camera>();

        void Update()
        {
            HandlePan();
            HandleZoom();
        }

        private Vector3? _focusTarget;

        /// <summary>Ask the camera to glide to center on a world position (e.g. the active combatant). Cancelled
        /// the moment the player pans manually.</summary>
        public void FocusOn(Vector3 world)
        {
            _focusTarget = new Vector3(world.x, world.y, transform.position.z);
        }

        private void HandlePan()
        {
            Vector3 move = Vector3.zero;
            move.x += Input.GetAxisRaw("Horizontal");
            move.y += Input.GetAxisRaw("Vertical");

            if (enableEdgeScroll)
            {
                Vector3 m = Input.mousePosition;
                if (m.x <= edgeScrollMargin) move.x -= 1;
                if (m.x >= Screen.width - edgeScrollMargin) move.x += 1;
                if (m.y <= edgeScrollMargin) move.y -= 1;
                if (m.y >= Screen.height - edgeScrollMargin) move.y += 1;
            }

            if (move == Vector3.zero)
            {
                // No manual input: glide toward a focus target if one's set.
                if (_focusTarget.HasValue)
                {
                    var p = Vector3.Lerp(transform.position, _focusTarget.Value, 6f * Time.deltaTime);
                    if (maxBounds != minBounds)
                    {
                        p.x = Mathf.Clamp(p.x, minBounds.x, maxBounds.x);
                        p.y = Mathf.Clamp(p.y, minBounds.y, maxBounds.y);
                    }
                    transform.position = p;
                    if ((transform.position - _focusTarget.Value).sqrMagnitude < 0.0004f) _focusTarget = null;
                }
                return;
            }
            _focusTarget = null; // player took control
            Vector3 pos = transform.position + move.normalized * panSpeed * Time.deltaTime;

            if (maxBounds != minBounds)
            {
                pos.x = Mathf.Clamp(pos.x, minBounds.x, maxBounds.x);
                pos.y = Mathf.Clamp(pos.y, minBounds.y, maxBounds.y);
            }
            transform.position = pos;
        }

        private void HandleZoom()
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(scroll) < 0.0001f) return;

            if (_cam.orthographic)
                _cam.orthographicSize = Mathf.Clamp(
                    _cam.orthographicSize - scroll * zoomSpeed, minZoom, maxZoom);
            else
                _cam.fieldOfView = Mathf.Clamp(
                    _cam.fieldOfView - scroll * zoomSpeed * 4f, 20f, 70f);
        }
    }
}
