using UnityEngine;
using UnityEngine.XR;

namespace SpiderDud.Player
{
    /// <summary>
    /// Dual-hand web-swing locomotion. Each hand fires an independent rope
    /// (raycast from the hand anchor) and the player's velocity is constrained
    /// to a sphere around each active anchor, like a pendulum on a rope.
    /// Reads grip button state directly from the XR input devices so it works
    /// with any OpenXR-compliant controller without an Input Actions asset.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class WebSwingController : MonoBehaviour
    {
        [Header("Hand & Head Anchors")]
        [Tooltip("Assign to the XR Origin's left controller transform.")]
        [SerializeField] private Transform leftHandAnchor;
        [Tooltip("Assign to the XR Origin's right controller transform.")]
        [SerializeField] private Transform rightHandAnchor;
        [Tooltip("Assign to the XR Origin's camera (head) transform. Used as a fallback aim source.")]
        [SerializeField] private Transform headAnchor;

        [Header("Web Settings")]
        [SerializeField] private float maxWebDistance = 60f;
        [SerializeField] private float minRopeLength = 2f;
        [SerializeField] private LayerMask swingableMask = ~0;
        [SerializeField] private float releaseBoost = 1.15f;

        [Header("Physics")]
        [SerializeField] private float gravity = -18f;
        [SerializeField] private float swingAssist = 6f;
        [SerializeField] private float maxSpeed = 40f;

        [Header("Visuals")]
        [SerializeField] private LineRenderer leftWebLine;
        [SerializeField] private LineRenderer rightWebLine;

        private CharacterController controller;
        private Vector3 velocity;

        private struct Web
        {
            public bool active;
            public Vector3 anchor;
            public float length;
        }

        private Web leftWeb;
        private Web rightWeb;

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
        }

        private void Update()
        {
            HandleHand(XRNode.LeftHand, leftHandAnchor, ref leftWeb);
            HandleHand(XRNode.RightHand, rightHandAnchor, ref rightWeb);

            UpdateWebLine(leftWeb, leftHandAnchor, leftWebLine);
            UpdateWebLine(rightWeb, rightHandAnchor, rightWebLine);
        }

        private void FixedUpdate()
        {
            velocity.y += gravity * Time.fixedDeltaTime;

            ApplyWebConstraint(ref leftWeb);
            ApplyWebConstraint(ref rightWeb);
            ClampSpeed();

            controller.Move(velocity * Time.fixedDeltaTime);

            if (controller.isGrounded && velocity.y < 0f)
            {
                velocity.y = -1f;
            }
        }

        private void HandleHand(XRNode node, Transform anchor, ref Web web)
        {
            if (anchor == null)
            {
                return;
            }

            InputDevice device = InputDevices.GetDeviceAtXRNode(node);
            bool pressed = device.isValid
                && device.TryGetFeatureValue(CommonUsages.gripButton, out bool gripped)
                && gripped;

            if (pressed && !web.active)
            {
                FireWeb(anchor, ref web);
            }
            else if (!pressed && web.active)
            {
                ReleaseWeb(ref web);
            }
        }

        private void FireWeb(Transform anchor, ref Web web)
        {
            if (Physics.Raycast(anchor.position, anchor.forward, out RaycastHit hit, maxWebDistance, swingableMask))
            {
                web.active = true;
                web.anchor = hit.point;
                web.length = Mathf.Max(minRopeLength, Vector3.Distance(transform.position, hit.point));
            }
        }

        private void ReleaseWeb(ref Web web)
        {
            web.active = false;
            velocity *= releaseBoost;
        }

        private void ApplyWebConstraint(ref Web web)
        {
            if (!web.active)
            {
                return;
            }

            Vector3 toPlayer = transform.position - web.anchor;
            float distance = toPlayer.magnitude;
            if (distance <= web.length || distance < 0.0001f)
            {
                return;
            }

            Vector3 radial = toPlayer / distance;

            // Kill the outward radial component of velocity so the player
            // can't stretch past the rope length (a simple distance constraint).
            float radialSpeed = Vector3.Dot(velocity, radial);
            if (radialSpeed > 0f)
            {
                velocity -= radial * radialSpeed;
            }

            // Snap the position back onto the rope's sphere.
            transform.position -= radial * (distance - web.length);

            // Small tangential assist so momentum builds naturally through the swing arc.
            Vector3 tangent = Vector3.ProjectOnPlane(Vector3.down, radial);
            velocity += tangent.normalized * (swingAssist * Time.fixedDeltaTime);
        }

        private void ClampSpeed()
        {
            Vector3 horizontal = new Vector3(velocity.x, 0f, velocity.z);
            if (horizontal.magnitude > maxSpeed)
            {
                horizontal = horizontal.normalized * maxSpeed;
                velocity = new Vector3(horizontal.x, velocity.y, horizontal.z);
            }
        }

        private static void UpdateWebLine(Web web, Transform anchorTransform, LineRenderer line)
        {
            if (line == null)
            {
                return;
            }

            if (web.active && anchorTransform != null)
            {
                line.enabled = true;
                line.SetPosition(0, anchorTransform.position);
                line.SetPosition(1, web.anchor);
            }
            else
            {
                line.enabled = false;
            }
        }
    }
}
