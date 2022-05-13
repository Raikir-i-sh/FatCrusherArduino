using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyState { MOVE, ATK, AVOID, HURT, DEAD };

public class Unit : MonoBehaviour
{
    public float movespeed = 10f;
    public int damage = 1;
    public int health = 1;
    public Vector2 visualRange;
    private int fullhealth;
    [HideInInspector] public GameObject player;
    [HideInInspector] protected EnemyAnimation unitAnim;
    public void SetAnimation(EnemyAnimation unitAnimInstance) { unitAnim = unitAnimInstance; }
    public void DisableAnimation() { unitAnim = null; }

    // all enemies idle position starts at facing right
    [HideInInspector] public bool facingleft = false;

    protected virtual void Awake()
    {
        fullhealth = health;
    }

    protected virtual void OnEnable()
    {
        health = fullhealth;
    }

    public void SetPlayer(GameObject playr)
    {
        player = playr;
    }
    protected virtual void MoveState() { }
    protected virtual void Atk1State() { }
    protected virtual void Atk2State() { }
    protected virtual void DeathState() { }
    protected virtual void AvoidState() { }
    public virtual void GiveDamage() { }
    public virtual void Detect() { }

    protected bool PlayerIsInRange()
    {
        if (Mathf.Abs(player.transform.position.x - transform.position.x) < visualRange.x &&
            Mathf.Abs(player.transform.position.y - transform.position.y) < visualRange.y)
        {
            return true;
        }
        return false;
    }

}