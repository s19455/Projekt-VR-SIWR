using UnityEngine;

[RequireComponent(typeof(Collider))]
public class GiraffeFeed : MonoBehaviour
{
    [SerializeField] private string giraffeId = "giraffe";
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

        if (QuestManager.Instance != null)
            QuestManager.Instance.RegisterGiraffeFed(giraffeId);
        else
            Debug.LogWarning("[GiraffeFeed] QuestManager.Instance is null. Did you forget to add it to a scene loaded earlier?");

        Destroy(other.gameObject, destroyDelay);
    }
}
