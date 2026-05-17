using System.Collections.Generic;
using UnityEngine;

public class LeafSpawner : MonoBehaviour
{
    [SerializeField] private GameObject leafPrefab;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private int maxLeaves = 6;
    [SerializeField] private float spawnInterval = 2f;

    private readonly List<GameObject> activeLeaves = new List<GameObject>();
    private float timer;

private void Update()
    {
        activeLeaves.RemoveAll(x => x == null);
        timer += Time.deltaTime;
        if (timer < spawnInterval) return;
        timer = 0f;

        Debug.Log("[LeafSpawner] Tick. ActiveLeaves=" + activeLeaves.Count + " prefab=" + (leafPrefab != null) + " points=" + (spawnPoints != null ? spawnPoints.Length : -1));

        if (activeLeaves.Count >= maxLeaves) return;
        if (leafPrefab == null || spawnPoints == null || spawnPoints.Length == 0) return;

        var point = spawnPoints[Random.Range(0, spawnPoints.Length)];
        if (point == null) { Debug.LogWarning("[LeafSpawner] spawnPoint is null"); return; }
        var leaf = Instantiate(leafPrefab, point.position, Random.rotation);
        leaf.name = "Leaf";
        activeLeaves.Add(leaf);
        Debug.Log("[LeafSpawner] Spawned leaf at " + point.position);
    }
}
