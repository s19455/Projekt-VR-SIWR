using UnityEngine;

[RequireComponent(typeof(Collider))]
public class GiraffeFeed : MonoBehaviour
{
    [SerializeField] private string giraffeId = "giraffe";
    [SerializeField] private Animator giraffeAnimator;
    [SerializeField] private float destroyDelay = 0.4f;

    private bool fed;

    private void Reset()
    {
        var col = GetComponent<Collider>();
        if (col != null) col.isTrigger = true;
    }

private void OnTriggerEnter(Collider other)
    {
        if (fed) return;
        if (!other.CompareTag("Leaf")) return;

        fed = true;
        Debug.Log("[GiraffeFeed] '" + giraffeId + "' ate a leaf.");

        if (giraffeAnimator != null)
            giraffeAnimator.SetTrigger("Bend");

        var wiggle = GetComponentInParent<GiraffeWiggle>();
        if (wiggle != null) wiggle.TriggerBend();

        if (QuestManager.Instance != null)
            QuestManager.Instance.RegisterGiraffeFed(giraffeId);
        else
            Debug.LogWarning("[GiraffeFeed] QuestManager.Instance is null.");

        Destroy(other.gameObject, destroyDelay);
    }
}
