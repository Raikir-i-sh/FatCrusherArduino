using System;
using UnityEngine;

public class PacmanEnemy : Unit
{
    bool attackable = true;
    public bool IsHurt { get; set; }
    //	public SpriteRenderer sprite;
    private EnemyState currentstate;

    protected override void Awake()
    {
        base.Awake();
    }
    // show Atk range in gizmos
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(visualRange.x * 2, visualRange.y * 2, 0f));
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        attackable = true;
        IsHurt = false;
        currentstate = currentstate = EnemyState.MOVE;
    }

    private void OnDisable()
    {
    }

    void Update()
    {
        MoveState();
        Atk1State();
        DeathState();
    }

    protected override void MoveState()
    {
        if (player != null && currentstate == EnemyState.MOVE)
        {
            Flip();

            float x = Mathf.MoveTowards(transform.position.x, player.transform.position.x, Time.deltaTime * movespeed);
            float y = Mathf.MoveTowards(transform.position.y, player.transform.position.y, Time.deltaTime * (movespeed / 3));
            transform.position = new Vector3(x, y, transform.position.z);
            unitAnim.GotoMove();
        }
    }

    private void Flip()
    {
        if (player.transform.position.x - transform.position.x < 0)
        {
            transform.localScale = new Vector3(-1 * Mathf.Abs(transform.localScale.x), transform.localScale.y, 0f);
        }
        else
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, 0f);
        }
        return;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (player.GetComponent<FatPlayerAgent>() != null)
                player.GetComponent<FatPlayerAgent>().GiveDamage();
        }
    }

    protected override void Atk1State()
    {
        if (PlayerIsInRange())
        {
            currentstate = EnemyState.ATK;

            unitAnim.GotoAtk1();
        }
    }

    protected override void DeathState()
    {
        // Put death particles ,sound ,etc
    }

    public void GiveDamage(int dmg)
    {
        attackable = false;
        IsHurt = true;
        health -= dmg;
        SoundInstance.InstantiateOnTransform(SoundInstance.GetClipFromLibrary("hit"), transform, 0.4f, false, SoundInstance.Randomization.Low);
        if (health <= 0)
        {
            currentstate = EnemyState.DEAD;
            LootGenerator.Instance.LootDrop(this.transform);
            ParticleSystemManager.Instance.PlayParticle1(this.transform);
            gameObject.DestroyAPS();
        }
    }

    public override void GiveDamage()
    {
        GiveDamage(1);
    }

    public void AE_GotoMoveState()
    {
        currentstate = EnemyState.MOVE;
    }
}