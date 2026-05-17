using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRSimpleInteractable))]
public class InfoBoardButton : MonoBehaviour
{
    [SerializeField] private string factId = "tongue";
    [SerializeField] private AudioSource voiceSource;
    [SerializeField] private AudioClip voiceClip;

    private XRSimpleInteractable interactable;
    private bool played;

private void Awake()
    {
        interactable = GetComponent<XRSimpleInteractable>();
        interactable.hoverEntered.AddListener(OnHovered);
        interactable.selectEntered.AddListener(OnSelected);
    }

private void OnDestroy()
    {
        if (interactable != null)
        {
            interactable.hoverEntered.RemoveListener(OnHovered);
            interactable.selectEntered.RemoveListener(OnSelected);
        }
    }

private void OnHovered(HoverEnterEventArgs args)
    {
        Activate("hover");
    }


private void OnSelected(SelectEnterEventArgs args)
    {
        Activate("select");
    }

    private void Activate(string source)
    {
        if (played) return;
        played = true;

        Debug.Log("[InfoBoardButton] Activated fact '" + factId + "' via " + source + ".");

        if (voiceSource != null && voiceClip != null)
            voiceSource.PlayOneShot(voiceClip);

        if (QuestManager.Instance != null)
            QuestManager.Instance.RegisterFact(factId);
        else
            Debug.LogWarning("[InfoBoardButton] QuestManager.Instance is null.");
    }

private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("MainCamera")) Activate("trigger");
    }

}
