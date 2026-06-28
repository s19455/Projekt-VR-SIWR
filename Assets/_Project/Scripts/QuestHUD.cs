using UnityEngine;
using TMPro;

public class QuestHUD : MonoBehaviour
{
    [SerializeField] private TMP_Text feedingLine;
    [SerializeField] private TMP_Text factsLine;
    [SerializeField] private RectTransform feedingCheckbox;
    [SerializeField] private RectTransform factsCheckbox;
    [SerializeField] private Color pendingColor = new Color(0.95f, 0.92f, 0.85f);
    [SerializeField] private Color doneColor = new Color(0.40f, 0.85f, 0.40f);

    private QuestManager _qm;

    private void OnEnable() { TryHook(); Refresh(); }
    private void OnDisable() { if (_qm != null) _qm.OnQuestUpdated -= Refresh; _qm = null; }
    private void Update() { if (_qm == null) TryHook(); }

    private void TryHook()
    {
        var qm = QuestManager.Instance;
        if (qm == null || qm == _qm) return;
        if (_qm != null) _qm.OnQuestUpdated -= Refresh;
        _qm = qm;
        _qm.OnQuestUpdated += Refresh;
        Refresh();
    }

    private void Refresh()
    {
        var qm = _qm ?? QuestManager.Instance;
        if (qm == null) return;

        if (feedingLine != null)
        {
            feedingLine.text = $"Nakarm żyrafy   {qm.GiraffesFed}/{QuestManager.GiraffesRequired}";
            feedingLine.color = qm.FeedingComplete ? doneColor : pendingColor;
        }
        if (factsLine != null)
        {
            factsLine.text = $"Poznaj fakty    {qm.FactsHeard.Count}/{QuestManager.AllFacts.Length}";
            factsLine.color = qm.FactsComplete ? doneColor : pendingColor;
        }
        if (feedingCheckbox != null) SetChecked(feedingCheckbox, qm.FeedingComplete);
        if (factsCheckbox != null) SetChecked(factsCheckbox, qm.FactsComplete);
    }

    private void SetChecked(RectTransform box, bool done)
    {
        var img = box.GetComponent<UnityEngine.UI.Image>();
        if (img != null) img.color = done ? doneColor : new Color(0.2f, 0.2f, 0.2f, 0.6f);
    }
}
