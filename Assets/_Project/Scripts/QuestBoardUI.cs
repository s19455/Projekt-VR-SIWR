using UnityEngine;
using TMPro;

public class QuestBoardUI : MonoBehaviour
{
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text feedingText;
    [SerializeField] private TMP_Text factsText;
    [SerializeField] private TMP_Text statusText;

    [SerializeField] private Color pendingColor = new Color(0.95f, 0.85f, 0.30f);
    [SerializeField] private Color doneColor = new Color(0.30f, 0.85f, 0.40f);

    private QuestManager _qm;

    private void OnEnable()
    {
        if (titleText != null) titleText.text = "Misja: Opiekun ZOO";
        TryHook();
        Refresh();
    }

    private void OnDisable()
    {
        if (_qm != null) _qm.OnQuestUpdated -= Refresh;
        _qm = null;
    }

    private void Update()
    {
        if (_qm == null) TryHook();
    }

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

        if (feedingText != null)
        {
            feedingText.text = $"Nakarm żyrafy:  {qm.GiraffesFed} / {QuestManager.GiraffesRequired}";
            feedingText.color = qm.FeedingComplete ? doneColor : pendingColor;
        }
        if (factsText != null)
        {
            factsText.text = $"Poznaj fakty:  {qm.FactsHeard.Count} / {QuestManager.AllFacts.Length}";
            factsText.color = qm.FactsComplete ? doneColor : pendingColor;
        }
        if (statusText != null)
        {
            if (qm.AllComplete)
            {
                statusText.text = "MISJA UKOŃCZONA!";
                statusText.color = doneColor;
            }
            else
            {
                statusText.text = "Postęp w toku...";
                statusText.color = pendingColor;
            }
        }
    }
}
