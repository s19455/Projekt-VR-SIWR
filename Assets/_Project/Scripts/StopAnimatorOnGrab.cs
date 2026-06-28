using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

/// <summary>
/// Po pierwszym chwyceniu XR Grab Interactable: wyłącza Animator (klip stop),
/// przełącza Rigidbody z kinematic → dynamic (gracz może rzucać klucze).
/// </summary>
[RequireComponent(typeof(XRGrabInteractable))]
public class StopAnimatorOnGrab : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private bool destroyAfterGrab = false;

    private XRGrabInteractable _grab;
    private bool _triggered;

    private void Awake()
    {
        _grab = GetComponent<XRGrabInteractable>();
        if (animator == null) animator = GetComponent<Animator>();
        if (rb == null) rb = GetComponent<Rigidbody>();
        _grab.selectEntered.AddListener(OnGrabbed);
    }

    private void OnDestroy()
    {
        if (_grab != null) _grab.selectEntered.RemoveListener(OnGrabbed);
    }

private void OnGrabbed(SelectEnterEventArgs args)
    {
        if (_triggered) return;
        _triggered = true;
        if (animator != null) animator.enabled = false;
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }
        if (destroyAfterGrab)
        {
            Debug.Log("[StopAnimatorOnGrab] '" + gameObject.name + "' picked up — destroying.");
            // Disable interaction immediately, destroy with small delay for tactile feedback
            _grab.enabled = false;
            Destroy(gameObject, 0.15f);
        }
    }
}
