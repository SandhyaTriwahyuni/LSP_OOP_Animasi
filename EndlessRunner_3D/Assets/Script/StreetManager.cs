using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class StreetManager : MonoBehaviour
{
    public GameObject[] JalanPrefabs;
    public float ZSpawn = 0;
    public float JalanLength = 40;
    public int NumberOfJalan = 10;

    private List<GameObject> _activeJalan = new List<GameObject>();
    private Queue<int> _lastSpawnedIndexes = new Queue<int>();
    private Queue<GameObject> _jalanPool = new Queue<GameObject>();

    public Transform PlayerTransform;

    void Start()
    {
        // Initialize the object pool
        for (int i = 0; i < JalanPrefabs.Length * NumberOfJalan; i++)
        {
            GameObject go = Instantiate(JalanPrefabs[i % JalanPrefabs.Length]);
            go.SetActive(false);
            _jalanPool.Enqueue(go);
        }

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
        GameObject go = GetPooledJalan(JalanIndex);
        go.transform.position = transform.forward * ZSpawn;
        go.transform.rotation = transform.rotation;
        go.SetActive(true);
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
        GameObject go = _activeJalan[0];
        go.SetActive(false);
        _jalanPool.Enqueue(go);
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

    private GameObject GetPooledJalan(int JalanIndex)
    {
        foreach (GameObject go in _jalanPool)
        {
            if (go.name.Contains(JalanPrefabs[JalanIndex].name))
            {
                _jalanPool = new Queue<GameObject>(_jalanPool.Where(g => g != go));
                return go;
            }
        }
        GameObject newGo = Instantiate(JalanPrefabs[JalanIndex]);
        newGo.SetActive(false);
        return newGo;
    }
}
