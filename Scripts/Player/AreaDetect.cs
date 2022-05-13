using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//When changing area detect's border , MAKE SURE to change observation maxDistX & maxDistY
//else agent won't work properly.
public class AreaDetect : MonoBehaviour
{
    [HideInInspector] public List<Transform> enemiesInRange = new List<Transform>();

    public void Follow(float x)
    {
        transform.position = new Vector2(x, transform.position.y);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("target"))
        {
            enemiesInRange.Add(collision.transform);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("target"))
        {
            enemiesInRange.Remove(collision.transform);
        }
    }
}