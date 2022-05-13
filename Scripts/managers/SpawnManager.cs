using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnManager : Singleton<SpawnManager>
{
    private FatPlayerAgent player;
    [HideInInspector] private GameObject temp;

    [Tooltip("Put'em in order of easy,air-type,med,heavy")]
    public List<GameObject> enemyPrefabs;
    public List<Transform> spawnPoints;
    [HideInInspector] public AdvPoolingSystem pooler;
    public GameObject enemyGrp; // this is parent for all enemies to disable them easily.
    public float spawnRate = 7f; //in sec

    Transform[] allEnemies;
    bool firstTime = true;
    IEnumerator coroutine;

    void OnEnable()
    {
        GameflowChanger.Instance.RaiseEvent += OnGameFlowUpdate;
    }

    void Start()
    {
        pooler = AdvPoolingSystem.Instance;
        player = GameObject.FindObjectOfType<FatPlayerAgent>();
        coroutine = spawnEnemy();
        if (player != null)
        {
            StartCoroutine(coroutine);
        }
    }

    public void StartSpawning(FatPlayerAgent playr)
    {
        player = playr;
        StopCoroutine(coroutine);
        StartCoroutine(coroutine);
    }

    public void spawnOneEnemy()
    {
        Invoke("OE", 1f);
    }

    void OE()
    {
        temp = pooler.InstantiateAPS(enemyPrefabs[0].name, spawnPoints[Random.Range(0, spawnPoints.Capacity)].position, Quaternion.identity, enemyGrp);
        temp.GetComponent<Unit>().SetPlayer(player.gameObject);
    }

    IEnumerator spawnEnemy()
    {
        while (true)
        {
            if (GameController.isGamePaused) yield return new WaitForSeconds(2f);

            int index = GetRandomEnemyIndex();

            if (index >= enemyPrefabs.Count) index = 0;

            temp = pooler.InstantiateAPS(enemyPrefabs[index].name, spawnPoints[Random.Range(0, spawnPoints.Capacity)].position, enemyGrp);
            if (temp.GetComponent<Unit>() != null) temp.GetComponent<Unit>().SetPlayer(player.gameObject);
            else temp.GetComponentInChildren<Unit>().SetPlayer(player.gameObject);

            yield return new WaitForSeconds(spawnRate);

            DestroyObjectsOutOfBorder();
        }
    }

    public void DestroyAll()
    {
        allEnemies = gameObject.FindComponentsInChildrenWithTag<Transform>("target");
        // if (allEnemies.Length == 0) { return; }
        foreach (Transform enemy in allEnemies)
        {
            enemy.gameObject.DestroyAPS();
        }
    }

    public void DestroyObjectsOutOfBorder()
    {
        allEnemies = gameObject.FindComponentsInChildrenWithTag<Transform>("target");
        // if (allEnemies.Length == 0) { return; }
        foreach (Transform enemy in allEnemies)
        {
            if (Mathf.Abs(enemy.localPosition.x) > 90 || Mathf.Abs(enemy.localPosition.y) > 30)
                enemy.gameObject.DestroyAPS();
        }
    }
    //4 ranges coz 4 types of enemy
    private int GetRandomEnemyIndex()
    {
        int probability = Random.Range(0, 100);

        if (probability < 50) return 0;
        if (probability >= 50 && probability < 70) return 1;
        if (probability >= 70 && probability < 90) return 2;
        if (probability >= 90 && probability < 100) return 3;
        return 0;
    }
    // Event that runs every 5 sec 
    void OnGameFlowUpdate(object sender, GameEventArgs e)
    {
        spawnRate = 9f - e.spawnDecreaseRate;
    }

}