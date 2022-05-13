using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[ExecuteInEditMode]
public class LootGenerator : Singleton<LootGenerator>
{
    [SerializeField] private LootTable _lootTable;

    void Start()
    {
    }

    void Update()
    {
        // TestLoots();
    }

    public void TestLoots()
    {
        IDictionary<string, int> loots = new Dictionary<string, int>();
        string s = "";

        for (int i = 0; i < 100; i++)
        {
            var item = _lootTable.GetRandomItem();

            if (loots.ContainsKey(item.itemName))
            {
                loots[item.itemName] += 1;
            }
            else
            {
                loots.Add(item.itemName, 1);
            }
        }

        foreach (var kvp in loots)
        {
            s += String.Format("{0} : {1}", kvp.Key, kvp.Value) + "\n";
        }
        print(s);
    }

    public void LootDrop(Transform spawnpoint)
    {
        var item = _lootTable.GetRandomItem();

        if (item.itemName.Equals("None")) return;

        GameObject collectItem = AdvPoolingSystem.Instance.InstantiateAPS(item.itemPrefab.name, spawnpoint.position);
        collectItem.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-1f, 1f), 1f), ForceMode2D.Impulse);
        StartCoroutine(DeliverToPlayer(collectItem));
    }

    IEnumerator DeliverToPlayer(GameObject item)
    {
        yield return new WaitForSeconds(2f);
        item.GetComponent<IPickable>().Use();
    }
}