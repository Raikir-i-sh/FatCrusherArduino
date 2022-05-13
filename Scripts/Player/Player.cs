using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Vector3 movedir;

    public int movespeed = 100;
    public float moveSpeedMultiplier = 1;
    public int jumpHeight = 50;
    public int damage = 10;
    public Powerup powerup;
    public Transform bulletspawn;
    public GameObject bulletPrefab;
    public GameObject weaponArm;

    [HideInInspector] public bool facingleft;
    [Tooltip("range of current map")]
    [HideInInspector]
    public float min, max;
    [HideInInspector] public Vector3 mousePos;
    //This value gets changed by Input Controller script
    [HideInInspector]
    public Vector3 Movedir { get => movedir; set => movedir = value; }

    // Start is called before the first frame update
    void Awake()
    {
        max = 20; min = -20;
        Movedir = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        Move(Movedir);
        RotateArm();
    }

    void Move(Vector2 dir)
    {
        if (Movedir != Vector3.zero)
        {
            Flip();
        }

        float temp = dir.x * movespeed * moveSpeedMultiplier * Time.deltaTime;
        float xPos = Mathf.Clamp(transform.position.x + temp, min, max);
        transform.position = new Vector3(xPos, transform.position.y, transform.position.z);
    }

    private void Flip()
    {
        if (Movedir.x == -1)
        {
            facingleft = true;
            transform.localScale = new Vector3(-1 * Mathf.Abs(transform.localScale.x), transform.localScale.y, 0f);
        }
        if (Movedir.x == 1)
        {
            facingleft = false;
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, 0f);
        }
        return;
    }

    private void RotateArm()
    {
        Vector3 difference = mousePos - weaponArm.transform.position;
        difference.Normalize();
        float rotation_z = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;

        if (facingleft)
        {
            weaponArm.transform.localRotation = Quaternion.Euler(0, 0, -rotation_z - 90);
        }
        else weaponArm.transform.localRotation = Quaternion.Euler(0f, 0f, rotation_z + 90);
    }

    public void Shoot()
    {
        if (bulletspawn)
        {
            Vector3 dir = mousePos - weaponArm.transform.position;
            dir.Normalize();
            float rotation_z = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            Instantiate(bulletPrefab, bulletspawn.transform.position, Quaternion.Euler(0, 0, rotation_z));
        }
    }

    public void TakeDamage(int dmg)
    {
        Debug.Log("dmg taken");
    }

    public void UseAbility1()
    {
        if (powerup)
        {
        }
    }
}