// Advanced Pooling System
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("AdvancedAdvPoolingSystem/AdvPoolingSystem")]

/// <summary>
///
/// </summary>
public sealed class AdvPoolingSystem : MonoBehaviour
{
    [System.Serializable]
    public class PoolingItems
    {
        public GameObject prefab;
        public int amount;
    }

    public static AdvPoolingSystem Instance;

    /// <summary>
    /// These fields will hold all the different types of assets you wish to pool.
    /// </summary>
    public PoolingItems[] poolingItems;
    public List<GameObject>[] pooledItems;

    /// <summary>
    /// The default pooling amount for each object type, in case the pooling amount is not mentioned or is 0.
    /// </summary>
    public int defaultPoolAmount = 10;

    /// <summary>
    /// Do you want the pool to expand in case more instances of pooled objects are required.
    /// </summary>
    [Tooltip("Do you want the pool to expand in case more instances of pooled objects are required.")]
    public bool poolExpand = true;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        pooledItems = new List<GameObject>[poolingItems.Length];

        for (int i = 0; i < poolingItems.Length; i++)
        {
            pooledItems[i] = new List<GameObject>();

            int poolingAmount;

            if (poolingItems[i].amount > 0) poolingAmount = poolingItems[i].amount;
            else poolingAmount = defaultPoolAmount;

            for (int j = 0; j < poolingAmount; j++)
            {
                GameObject newItem = (GameObject)Instantiate(poolingItems[i].prefab);
                newItem.SetActive(false);
                pooledItems[i].Add(newItem);
                newItem.transform.parent = transform;
            }
        }
    }

    public static void DestroyAPS(GameObject myObject)
    {
        myObject.SetActive(false);
    }

    public GameObject InstantiateAPS(string itemType)
    {
        GameObject newObject = GetPooledItem(itemType);

        if (newObject != null)
        {
            newObject.SetActive(true);
            return newObject;
        }
        Debug.Log("Warning: Pool is out of objects.\nTry enabling 'Pool Expand' option.");
        return null;
    }

    public GameObject InstantiateAPS(string itemType, Vector3 itemPosition)
    {
        GameObject newObject = GetPooledItem(itemType);

        if (newObject != null)
        {
            newObject.transform.position = itemPosition;
            newObject.SetActive(true);
            return newObject;
        }
        Debug.Log("Warning: Pool is out of objects.\nTry enabling 'Pool Expand' option.");
        return null;
    }

    public GameObject InstantiateAPS(string itemType, Vector3 itemPosition, Quaternion itemRotation)
    {
        GameObject newObject = GetPooledItem(itemType);

        if (newObject != null)
        {
            newObject.transform.position = itemPosition;
            newObject.transform.rotation = itemRotation;
            newObject.SetActive(true);
            return newObject;
        }
        Debug.Log("Warning: Pool is out of objects.\nTry enabling 'Pool Expand' option.");
        return null;
    }

    public GameObject InstantiateAPS(string itemType, Vector3 itemPosition, GameObject myParent)
    {
        GameObject newObject = GetPooledItem(itemType);

        if (newObject != null)
        {
            newObject.transform.position = itemPosition;
            newObject.transform.parent = myParent.transform;
            newObject.SetActive(true);
            return newObject;
        }
        Debug.Log("Warning: Pool is out of objects.\nTry enabling 'Pool Expand' option.");
        return null;
    }

    public GameObject InstantiateAPS(string itemType, Vector3 itemPosition, Quaternion itemRotation, GameObject myParent)
    {
        GameObject newObject = GetPooledItem(itemType);

        if (newObject != null)
        {
            newObject.transform.position = itemPosition;
            newObject.transform.rotation = itemRotation;
            newObject.transform.parent = myParent.transform;
            newObject.SetActive(true);
            return newObject;
        }
        Debug.Log("Warning: Pool is out of objects.\nTry enabling 'Pool Expand' option.");
        return null;
    }

    public static void PlayEffect(GameObject particleEffect, int particlesAmount)
    {
        if (particleEffect.GetComponent<ParticleSystem>())
        {
            particleEffect.GetComponent<ParticleSystem>().Emit(particlesAmount);
        }
    }

    public static void PlaySound(GameObject soundSource)
    {
        AudioSource audiosource = soundSource.GetComponent<AudioSource>();

        if (audiosource)
        {
            audiosource.PlayOneShot(audiosource.GetComponent<AudioSource>().clip);
        }
    }

    public GameObject GetPooledItem(string itemType)
    {
        for (int i = 0; i < poolingItems.Length; i++)
        {
            if (poolingItems[i].prefab.name == itemType)
            {
                for (int j = 0; j < pooledItems[i].Count; j++)
                {
                    if (!pooledItems[i][j].activeInHierarchy)
                    {
                        return pooledItems[i][j];
                    }
                }

                if (poolExpand)
                {
                    GameObject newItem = (GameObject)Instantiate(poolingItems[i].prefab);
                    newItem.SetActive(false);
                    pooledItems[i].Add(newItem);
                    newItem.transform.parent = transform;
                    return newItem;
                }

                break;
            }
        }

        return null;
    }
}

public static class AdvPoolingSystemExtensions
{
    public static void DestroyAPS(this GameObject myobject)
    {
        AdvPoolingSystem.DestroyAPS(myobject);
    }

    public static void PlayEffect(this GameObject particleEffect, int particlesAmount)
    {
        AdvPoolingSystem.PlayEffect(particleEffect, particlesAmount);
    }

    public static void PlaySound(this GameObject soundSource)
    {
        AdvPoolingSystem.PlaySound(soundSource);
    }
}