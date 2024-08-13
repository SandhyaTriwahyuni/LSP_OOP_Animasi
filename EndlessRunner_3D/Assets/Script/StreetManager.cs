using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StreetManager : MonoBehaviour
{
    public GameObject[] JalanPrefabs;
    public float ZSpawn = 0;
    public float JalanLength = 40;
    public int NumberOfJalan = 10;

    private List<GameObject> _activeJalan = new List<GameObject>();
    private Queue<int> _lastSpawnedIndexes = new Queue<int>();

    public Transform PlayerTransform;

    void Start()
    {
        for (int i = 0; i < NumberOfJalan; i++)
        {
            if (i == 0)
                SpawnJalan(0);
            else
                SpawnJalan(Random.Range(0, JalanPrefabs.Length));
        }
    }

    void Update()
    {
        if (PlayerTransform.position.z - 35 > ZSpawn - (NumberOfJalan * JalanLength))
        {
            SpawnJalan(GetRandomJalanIndex());
            DeleteJalan();
        }
    }

    public void SpawnJalan(int JalanIndex)
    {
        GameObject go = Instantiate(JalanPrefabs[JalanIndex], transform.forward * ZSpawn, transform.rotation);
        _activeJalan.Add(go);
        ZSpawn += JalanLength;

        _lastSpawnedIndexes.Enqueue(JalanIndex);
        if (_lastSpawnedIndexes.Count > 5)
        {
            _lastSpawnedIndexes.Dequeue();
        }
    }

    private void DeleteJalan()
    {
        Destroy(_activeJalan[0]);
        _activeJalan.RemoveAt(0);
    }

    private int GetRandomJalanIndex()
    {
        int randomIndex;
        do
        {
            randomIndex = Random.Range(0, JalanPrefabs.Length);
        } while (_lastSpawnedIndexes.Contains(randomIndex));
        return randomIndex;
    }
}