using System;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    public event Action OnQuestUpdated;

    public int GiraffesFed { get; private set; }
    public const int GiraffesRequired = 3;

    public HashSet<string> FactsHeard { get; } = new HashSet<string>();
    public static readonly string[] AllFacts = { "tongue", "sleep", "iucn" };

    public bool FeedingComplete => GiraffesFed >= GiraffesRequired;
    public bool FactsComplete => FactsHeard.Count >= AllFacts.Length;
    public bool AllComplete => FeedingComplete && FactsComplete;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void RegisterGiraffeFed(string giraffeId)
    {
        GiraffesFed++;
        Debug.Log("[QuestManager] Fed giraffe '" + giraffeId + "'. Total: " + GiraffesFed + "/" + GiraffesRequired);
        OnQuestUpdated?.Invoke();
        if (FeedingComplete) Debug.Log("[QuestManager] All giraffes fed!");
    }

    public void RegisterFact(string factId)
    {
        if (FactsHeard.Add(factId))
        {
            Debug.Log("[QuestManager] Learned fact '" + factId + "'. Total: " + FactsHeard.Count + "/" + AllFacts.Length);
            OnQuestUpdated?.Invoke();
            if (FactsComplete) Debug.Log("[QuestManager] All facts learned!");
            if (AllComplete) Debug.Log("[QuestManager] QUEST COMPLETE!");
        }
    }

    public void ResetProgress()
    {
        GiraffesFed = 0;
        FactsHeard.Clear();
        OnQuestUpdated?.Invoke();
        Debug.Log("[QuestManager] Progress reset.");
    }
}
