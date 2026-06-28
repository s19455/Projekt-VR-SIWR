using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

// A collectible paper. On grab/select: show its fact before the player's eyes, register it, then disappear.
[RequireComponent(typeof(XRSimpleInteractable))]
public class FactPaper : MonoBehaviour
{
    [SerializeField] private string factId = "tongue";
    [SerializeField] private string title = "Ciekawostka";
    [TextArea(2, 5)]
    [SerializeField] private string body = "...";
    [SerializeField] private float floatAmplitude = 0.05f;
    [SerializeField] private float floatSpeed = 1.5f;
    [SerializeField] private float spinSpeed = 30f;

    private XRSimpleInteractable _interactable;
    private bool _collected;
    private Vector3 _baseLocalPos;

    private void Awake()
    {
        _interactable = GetComponent<XRSimpleInteractable>();
        _interactable.selectEntered.AddListener(OnSelected);
        _interactable.hoverEntered.AddListener(OnHovered);
        _baseLocalPos = transform.localPosition;
    }

    private void OnDestroy()
    {
        if (_interactable != null)
        {
            _interactable.selectEntered.RemoveListener(OnSelected);
            _interactable.hoverEntered.RemoveListener(OnHovered);
        }
    }

    private void Update()
    {
        // Gentle float + spin to attract attention
        float y = Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
        transform.localPosition = _baseLocalPos + new Vector3(0f, y, 0f);
        transform.Rotate(0f, spinSpeed * Time.deltaTime, 0f, Space.Self);
    }

    private void OnHovered(HoverEnterEventArgs args) => Collect();
    private void OnSelected(SelectEnterEventArgs args) => Collect();

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("MainCamera")) Collect();
    }

    private void Collect()
    {
        if (_collected) return;
        _collected = true;

        if (FactDisplay.Instance != null)
            FactDisplay.Instance.ShowFact(title, body);

        if (QuestManager.Instance != null)
            QuestManager.Instance.RegisterFact(factId);

        // Play narration mp3 by convention: Resources/FactAudio/<factId>
        var clip = Resources.Load<AudioClip>($"FactAudio/{factId}");
        if (clip != null)
            AudioSource.PlayClipAtPoint(clip, Camera.main != null ? Camera.main.transform.position : transform.position, 1f);
        else
            Debug.Log($"[FactPaper] No audio at Resources/FactAudio/{factId}");

        Debug.Log($"[FactPaper] Collected '{factId}'");
        Destroy(gameObject, 0.05f);
    }
}
