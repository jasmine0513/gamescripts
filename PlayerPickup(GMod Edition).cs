using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;         // new input system
#endif

public class PlayerPickup : MonoBehaviour
{
    [Header("References")]
    public Camera cam;

    [Header("Tuning")]
    public float maxGrabDistance = 4f;
    public float holdDistance = 2f;
    public float moveStrength = 50f;
    public float rotateStrength = 10f;
    public float throwForce = 10f;
    public LayerMask grabbableMask = ~0;

    [Header("Input")]
#if ENABLE_INPUT_SYSTEM
    public Key pickupKeyNew = Key.E;
#else
    [Tooltip("Used when Legacy Input is active")]
#endif
#if ENABLE_LEGACY_INPUT_MANAGER
    public KeyCode pickupKeyLegacy = KeyCode.E;
#endif

    Rigidbody held;
    float savedDrag, savedAngularDrag;

    void Awake()
    {
        if (!cam) cam = Camera.main;
    }

    void Update()
    {
        if (PressedPickup())
        {
            if (held) Drop();
            else TryGrab();
        }

        if (held && PressedThrow())
            Throw();
    }

    void FixedUpdate()
    {
        if (!held) return;

        Vector3 target = cam.transform.position + cam.transform.forward * holdDistance;

        Vector3 toTarget = target - held.worldCenterOfMass;
        held.linearVelocity = toTarget * moveStrength;

        Quaternion wanted = Quaternion.LookRotation(cam.transform.forward, Vector3.up);
        Quaternion delta = wanted * Quaternion.Inverse(held.rotation);
        delta.ToAngleAxis(out float angle, out Vector3 axis);
        if (angle > 180f) angle -= 360f;
        held.angularVelocity = axis * angle * Mathf.Deg2Rad * rotateStrength;
    }

    void TryGrab()
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if (Physics.Raycast(ray, out RaycastHit hit, maxGrabDistance, grabbableMask, QueryTriggerInteraction.Ignore))
        {
            Rigidbody rb = hit.rigidbody;
            if (rb && !rb.isKinematic)
            {
                held = rb;

                savedDrag = held.linearDamping;
                savedAngularDrag = held.angularDamping;

                held.useGravity = false;
                held.linearDamping = 10f;
                held.angularDamping = 10f;
                held.collisionDetectionMode = CollisionDetectionMode.Continuous;
                held.interpolation = RigidbodyInterpolation.Interpolate;
            }
        }
    }

    void Drop()
    {
        if (!held) return;
        held.useGravity = true;
        held.linearDamping = savedDrag;
        held.angularDamping = savedAngularDrag;
        held = null;
    }

    void Throw()
    {
        if (!held) return;
        held.linearVelocity = Vector3.zero;
        held.angularVelocity = Vector3.zero;
        held.AddForce(cam.transform.forward * throwForce, ForceMode.VelocityChange);
        Drop();
    }

    // -------- Input helpers (works with either system) --------
    bool PressedPickup()
    {
#if ENABLE_INPUT_SYSTEM
        var kb = Keyboard.current;
        if (kb != null) return kb[pickupKeyNew].wasPressedThisFrame;
#endif
#if ENABLE_LEGACY_INPUT_MANAGER
        return Input.GetKeyDown(pickupKeyLegacy);
#else
        return false;
#endif
    }

    bool PressedThrow()
    {
#if ENABLE_INPUT_SYSTEM
        var m = Mouse.current;
        if (m != null) return m.leftButton.wasPressedThisFrame;
#endif
#if ENABLE_LEGACY_INPUT_MANAGER
        return Input.GetMouseButtonDown(0);
#else
        return false;
#endif
    }
}
