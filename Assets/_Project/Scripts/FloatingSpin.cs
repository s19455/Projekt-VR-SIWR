using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

/// <summary>
/// GTA-style pickup: obiekt unosi się sinusoidalnie i obraca się wokół własnej osi Y.
/// Po pierwszym chwyceniu (XRGrabInteractable) animacja się wyłącza i obiekt podlega fizyce gracza.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class FloatingSpin : MonoBehaviour
{
    [Header("Hover (góra-dół)")]
    [SerializeField] private float hoverAmplitude = 0.05f;   // metry
    [SerializeField] private float hoverSpeed = 2f;          // rad/s

    [Header("Spin (rotacja Y)")]
    [SerializeField] private float spinSpeedDegPerSec = 90f;

    [Header("Pickup behavior")]
    [SerializeField] private bool disableOnGrab = true;

    private Vector3 _basePos;
    private Quaternion _baseRot;
    private float _phase;
    private bool _active = true;
    private Rigidbody _rb;
    private XRGrabInteractable _grab;

    private void Awake()
    {
        _basePos = transform.position;
        _baseRot = transform.rotation;
        _phase = Random.value * Mathf.PI * 2f;

        _rb = GetComponent<Rigidbody>();
        _rb.isKinematic = true;
        _rb.useGravity = false;

        _grab = GetComponent<XRGrabInteractable>();
        if (_grab != null)
            _grab.selectEntered.AddListener(OnGrabbed);
    }

    private void OnDestroy()
    {
        if (_grab != null)
            _grab.selectEntered.RemoveListener(OnGrabbed);
    }

    private void Update()
    {
        if (!_active) return;

        float t = Time.time * hoverSpeed + _phase;
        float yOffset = Mathf.Sin(t) * hoverAmplitude;
        transform.position = _basePos + Vector3.up * yOffset;

        float spin = spinSpeedDegPerSec * Time.deltaTime;
        transform.rotation = Quaternion.AngleAxis(spin, Vector3.up) * transform.rotation;
        // Zachowaj base rotation pitch/roll, kumulatywny tylko yaw
        // (uproszczenie: pełna rotation jest reset gdy gracz puści — nieistotne dla pickup)
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        if (!disableOnGrab) return;
        _active = false;
        _rb.isKinematic = false;     // żeby chwyt VR działał z fizyką
        _rb.useGravity = true;
    }
}
