using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider))]
public class SceneTransition : MonoBehaviour
{
    [SerializeField] private string targetScene = "02_Savanna";
    [SerializeField] private string requiredTag = "MainCamera";

    private bool triggered;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        if (!other.CompareTag(requiredTag)) return;
        triggered = true;
        Debug.Log("[SceneTransition] Loading scene: " + targetScene);
        SceneManager.LoadScene(targetScene);
    }
}
