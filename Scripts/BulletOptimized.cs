using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BulletOptimized : MonoBehaviour
{
    public float speed = 2.0f;
    float interval = 0.4f; // this is 'n', in seconds.

    private Vector3 begin;
    private float timer = 0.0f;
    private bool hasHit = false;
    private float timeTillImpact = 0.0f;
    private RaycastHit2D hit;
    private string[] layerThatBulletHits = { "Enemy", "Ground", "Player" };
    private LayerMask mask;
    private GameObject sender; //unit that shoot the bullet
    // set up initial interval
    void Start()
    {
        begin = transform.position;
        timer = interval + 1;
        //Debug.Log("right  = " + transform.right);
    }

    void OnEnable()
    {
        Invoke("Dizable", 1f);
    }

    void OnDisable()
    {
        hasHit = false;
    }

    public void Dizable()
    {
        if (!hasHit)
        {
        }
        gameObject.DestroyAPS();
    }

    public void SetShooter(GameObject shooter)
    {
        this.sender = shooter;
    }

    void Update()
    {
        // don't allow an interval smaller than the frame.
        var usedInterval = interval;

        if (Time.deltaTime > usedInterval) usedInterval = Time.deltaTime;

        // every interval, we cast a ray forward from where we were at the start of this interval
        // to where we will be at the start of the next interval
        if (!hasHit && timer >= usedInterval)
        {
            timer = 0;

            var distanceThisInterval = speed * usedInterval;
            mask = LayerMask.GetMask(layerThatBulletHits);

            hit = Physics2D.Raycast(begin, transform.right, distanceThisInterval, mask);
            if (hit)
            {
                hasHit = true;
                if (speed != 0) timeTillImpact = hit.distance / speed;
            }

            begin += transform.right * distanceThisInterval;
        }

        timer += Time.deltaTime;

        // after the Raycast hit something, wait until the bullet has traveled
        // about as far as the ray traveled to do the actual hit
        if (hasHit && timer > timeTillImpact)
        {
        }
        else
        {
            //TODO use velocity insted
            this.GetComponent<Rigidbody2D>().velocity = transform.right * speed; // <speed> units/sec
                                                                                 //transform.position += transform.right * speed * Time.deltaTime;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (sender.tag == collision.tag) return; //hitting urself does nothing

        if (collision.CompareTag("target"))
        {
            collision.gameObject.GetComponent<Unit>().player.GetComponent<FatPlayerAgent>().GiveBigReward();
            collision.gameObject.GetComponent<Unit>().GiveDamage();
            CancelInvoke();
            gameObject.DestroyAPS();
        }
    }

}