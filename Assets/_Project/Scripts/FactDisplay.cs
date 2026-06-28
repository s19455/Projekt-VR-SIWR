using UnityEngine;
using TMPro;
using System.Collections;

// Singleton on a world-space canvas (child of Main Camera). Shows fact text before the player's eyes.
public class FactDisplay : MonoBehaviour
{
    public static FactDisplay Instance { get; private set; }

    [SerializeField] private CanvasGroup panel;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text bodyText;
    [SerializeField] private float showDuration = 6f;
    [SerializeField] private float fadeDuration = 0.4f;

    private Coroutine _routine;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        if (panel != null) panel.alpha = 0f;
    }

    public void ShowFact(string title, string body)
    {
        if (titleText != null) titleText.text = title;
        if (bodyText != null) bodyText.text = body;
        if (_routine != null) StopCoroutine(_routine);
        _routine = StartCoroutine(ShowRoutine());
    }

    private IEnumerator ShowRoutine()
    {
        if (panel == null) yield break;
        // Fade in
        float t = 0f;
        while (t < fadeDuration) { t += Time.deltaTime; panel.alpha = Mathf.Clamp01(t / fadeDuration); yield return null; }
        panel.alpha = 1f;
        yield return new WaitForSeconds(showDuration);
        // Fade out
        t = 0f;
        while (t < fadeDuration) { t += Time.deltaTime; panel.alpha = 1f - Mathf.Clamp01(t / fadeDuration); yield return null; }
        panel.alpha = 0f;
    }
}
