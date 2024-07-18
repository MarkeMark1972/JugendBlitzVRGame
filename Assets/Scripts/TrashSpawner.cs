using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashSpawner : MonoBehaviour
{
    [SerializeField] private List<Transform> trashPrefabs;
    [SerializeField] private List<Transform> trashCollection;
    [SerializeField] private float spawnInterval = 1.0f; // seconds between each spawn

    private void Start()
    {
        StartCoroutine(SpawnTrash());
    }
    
    private void OnDestroy()
    {
        StopAllCoroutines();
        foreach (var trash in trashCollection)
        {
            Destroy(trash.gameObject);
        }
        trashCollection.Clear();
        trashPrefabs.Clear();
        trashCollection = null;
        trashPrefabs = null;
    }

    private IEnumerator SpawnTrash()
    {
        while (true)
        {
            var rnd = Random.Range(0, trashPrefabs.Count);
            var spawnedTrash = Instantiate(trashPrefabs[rnd], this.transform.position, Quaternion.identity);
            spawnedTrash.GetComponent<Rigidbody>().AddTorque(Random.rotation.eulerAngles * (Mathf.Deg2Rad * 0.01f));
            trashCollection.Add(spawnedTrash);

            yield return new WaitForSeconds(spawnInterval);
        }
    }
}