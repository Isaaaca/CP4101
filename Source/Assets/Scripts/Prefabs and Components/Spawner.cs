using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour, ResetableInterface
{
    [SerializeField] private GameObject objectToSpawn = null;
    [SerializeField] private float spawnInterval = 1f;
    [SerializeField] private float timerOffset = 0f;

    private float timer;
    private List<GameObject> objectPool = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        timer = spawnInterval + timerOffset;
    }

    // Update is called once per frame
    void Update()
    {
        if (timer> 0)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                Spawn();
                timer = spawnInterval;
            }
        }
    }

    private void Spawn()
    {
        int availableObect = -1;
        //check for available object in pool
        for (int i= 0; i<objectPool.Count && availableObect ==-1; i++)
        {
            if (!objectPool[i].activeSelf) availableObect = i;
        }

        GameObject objToSpawn;
        if (availableObect == -1)
        {
            objToSpawn = Instantiate(objectToSpawn, transform);
            objectPool.Add(objToSpawn);
        }
        else
        {
            objToSpawn = objectPool[availableObect];
        }

        objToSpawn.SetActive(true);
        objToSpawn.GetComponent<ResetableInterface>().Reset();
        objToSpawn.transform.position = transform.position;
    }

    public void Reset()
    {
        for (int i = 0; i < objectPool.Count; i++)
        {
            ResetableInterface resetable = objectPool[i].GetComponent<ResetableInterface>();
            if (resetable != null) resetable.Reset();
            objectPool[i].SetActive(false);
            
        }
    }
}
