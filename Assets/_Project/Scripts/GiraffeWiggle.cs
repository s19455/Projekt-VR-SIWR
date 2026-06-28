using UnityEngine;

/// <summary>
/// Proceduralna animacja żyrafy bez rigu:
/// - Walk loop: subtelny bob (góra-dół) + sway (kołysanie wokół world Y).
/// - Bend (one-shot): pochylenie całej żyrafy w stronę -Z (gdzie zwykle wisi kosz z liśćmi),
///   imituje schylanie głowy do jedzenia. Wywołaj TriggerBend() z GiraffeFeed.
/// </summary>
public class GiraffeWiggle : MonoBehaviour
{
    [Header("Walk loop")]
    [SerializeField] private float walkSpeed = 1.0f;
    [SerializeField] private float bobAmplitude = 0.05f;       // world meters
    [SerializeField] private float swayDegrees = 2.0f;         // around world Y

    [Header("Bend (eating)")]
    [SerializeField] private float bendDegrees = 28f;          // pochylenie do przodu
    [SerializeField] private float bendDuration = 1.4f;        // czas pełnego cyklu (down + up)

    private Vector3 _basePos;
    private Quaternion _baseRot;
    private float _bendTimer = -1f;
    private float _phase;  // offset żeby każda żyrafa nie kołysała się dokładnie w fazie

    private void Awake()
    {
        _basePos = transform.localPosition;
        _baseRot = transform.localRotation;
        // unikalne przesunięcie fazy oparte o pozycję żyrafy w scenie
        _phase = (transform.position.x * 1.7f + transform.position.z * 2.3f) % (Mathf.PI * 2f);
    }

    private void Update()
    {
        float t = Time.time * walkSpeed + _phase;
        float bob = Mathf.Sin(t * 4f) * bobAmplitude;
        float sway = Mathf.Sin(t * 2f) * swayDegrees;

        Vector3 pos = _basePos + Vector3.up * bob;
        // sway: prepended w world space (wokół world Y)
        Quaternion rot = Quaternion.AngleAxis(sway, Vector3.up) * _baseRot;

        if (_bendTimer >= 0f)
        {
            float p = _bendTimer / bendDuration;
            float curve = Mathf.Sin(p * Mathf.PI);     // 0 -> 1 (max przy 50%) -> 0
            float bend = curve * bendDegrees;
            // pochylenie wokół world Vector3.left (top leci ku -Z, gdzie wisi kosz)
            rot = Quaternion.AngleAxis(bend, Vector3.left) * rot;
            _bendTimer += Time.deltaTime;
            if (_bendTimer >= bendDuration) _bendTimer = -1f;
        }

        transform.localPosition = pos;
        transform.localRotation = rot;
    }

    /// <summary>Wywołaj żeby żyrafa pochyliła głowę przez bendDuration sekund.</summary>
    public void TriggerBend()
    {
        if (_bendTimer < 0f) _bendTimer = 0f;
    }
}
