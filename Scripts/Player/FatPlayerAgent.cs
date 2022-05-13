using System.Collections;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Policies;

public class FatPlayerAgent : Agent
{
    public AreaDetect areadetect;

    [Tooltip("CursorAgent must be attached to this")]
    public Transform mouseTransform;
    [HideInInspector] public float moveSpeedMultiplier = 1;
    [HideInInspector] public float min, max;

    public GameObject weaponArm;
    public Transform fatStomach;
    public int Health { get { return health; } }
    private Rigidbody2D m_AgentRb;
    private float movespeed;
    private float jumpHeight;
    private int health = 10;

    private bool facingleft = false;
    private bool isGrounded;

    private bool meleeAtkin;
    private bool enemyTooClose; // for masking melee atk when far.
    private int meleeMisscount;

    private int killcount = 0;
    private int maxkillcount = 1;

    private BufferSensorComponent m_BufferSensor;
    private PlayerAnimation anim;
    #region Unity Methods

    void Start()
    {
        enemyTooClose = false;
        min = -20;
        max = 20;
        movespeed = SettingsManager.Instance.playersetting.agentRunSpeed;
        jumpHeight = SettingsManager.Instance.playersetting.jumpHeight;
        health = SettingsManager.Instance.playersetting.maxhealth;
        GetComponent<BehaviorParameters>().BehaviorType = (BehaviorType)SettingsManager.Instance.inputmode;
        anim = GetComponent<PlayerAnimation>();
        GameflowChanger.Instance.RaiseEvent += OnGameFlowUpdate;
    }

    void Update()
    {
        areadetect.Follow(transform.position.x);
        RotateArm();
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("projectile"))
        {
            //Deflect bullet
        }
        if (col.gameObject.CompareTag("target"))
        {
            // Damage trigger from melee atk.
            meleeMisscount--;
            GiveSmallReward();
            CameraShake.Shake(0.3f, 0.3f); // duration, amount
            col.GetComponent<Unit>().GiveDamage();
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("ground"))
        {
            isGrounded = true;
        }
    }
    #endregion
    #region ML agent 
    public override void Initialize()
    {
        m_BufferSensor = GetComponent<BufferSensorComponent>();

        m_AgentRb = GetComponent<Rigidbody2D>();

        isGrounded = true;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        int numOfEnemyToObserve = 2;
        // range at which player detects enemy
        //this MUST be half of size of 'AreaDetect' collider.
        float maxDistX = 14f;
        float maxDisty = 7.5f;

        //relative distance from Parent transform to Player
        sensor.AddObservation(Mathf.Clamp(transform.localPosition.x / max, -1, 1));
        sensor.AddObservation(meleeMisscount);
        System.Array.Sort(areadetect.enemiesInRange.ToArray(), (a, b) => (Vector2.Distance(a.transform.position, transform.position)).CompareTo(Vector2.Distance(b.transform.position, transform.position)));

        if (areadetect.enemiesInRange.Count == 0) enemyTooClose = false;
        // if not enough enemy to observe , add 1 to obs to occupy correct slots.
        for (int i = 0; i < numOfEnemyToObserve; i++)
        {
            if (i <= (areadetect.enemiesInRange.Count - 1))
            {
                var dir = areadetect.enemiesInRange[i].position - transform.position;
                int facingEnemy = 0;

                if (dir.magnitude < 5f) enemyTooClose = true;
                if ((facingleft && dir.x < 0) || (!facingleft) && dir.x >= 0)
                {
                    facingEnemy = 1;//facing enemy
                }
                else facingEnemy = 2;

                float[] enemyObservation = new float[]{
                    facingEnemy,
                dir.magnitude ,
                    (areadetect.enemiesInRange[i].position.x - transform.position.x) / maxDistX,
                    (areadetect.enemiesInRange[i].position.y - transform.position.y) / maxDisty
                    };
                m_BufferSensor.AppendObservation(enemyObservation);
            }
        }
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)

    {
        MoveAgent(actionBuffers.DiscreteActions);
        Melee(actionBuffers.DiscreteActions);
    }

    private int meleeInput, moveInput;

    public void SetMeleeIp(int meleeIp = 0)
    {
        meleeInput = meleeIp;
    }

    public void SetMoveIp(int moveIp = 0)
    {
        moveInput = moveIp;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;

        discreteActionsOut[1] = meleeInput; // melee atk 0 or 1

        discreteActionsOut[0] = moveInput; // 1 for right, 2 for left
        // ObservationDebugging();
    }

    public override void WriteDiscreteActionMask(IDiscreteActionMask actionMask)
    {
        //unable to shoot n melee during melee_animation

        actionMask.SetActionEnabled(1, 1, !meleeAtkin && enemyTooClose);
    }

    public override void OnEpisodeBegin()
    {
        meleeAtkin = false;
        meleeMisscount = 0;
        transform.localPosition = Vector3.zero;

        SpawnManager.Instance.DestroyAll();

        health = SettingsManager.Instance.playersetting.maxhealth;
        killcount = 0;
    }

    #endregion

    #region My methods

    public void MoveAgent(ActionSegment<int> act)
    {
        float dirToGo = 0;

        // Get the action branch index for movement
        int movement = act[0];

        switch (movement)
        {
            case 1:
                dirToGo = 1f;
                break;
            case 2:
                dirToGo = -1f;
                break;
        }
        if (dirToGo != 0)
        {
            Flip(dirToGo);
            anim.GotoRun();
        }
        else anim.GotoIdle();
        // Look up the index in the jump action list:

        if (!isGrounded)
        {
            moveSpeedMultiplier = 0.6f;
        }
        else moveSpeedMultiplier = 1;

        StayCloseToCenter();

        float temp = dirToGo * movespeed * moveSpeedMultiplier * Time.deltaTime;
        float xPos = Mathf.Clamp(transform.localPosition.x + temp, min, max);

        transform.localPosition = new Vector3(xPos, transform.localPosition.y, transform.localPosition.z); //MOVE
    }

    private void Flip(float dir)
    {
        if (dir == -1)
        {
            facingleft = true;
            transform.localScale = new Vector3(-1 * Mathf.Abs(transform.localScale.x), transform.localScale.y, 0f);
        }
        if (dir == 1)
        {
            facingleft = false;
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, 0f);
        }
        return;
    }

    private void RotateArm()
    {
        Vector3 difference = mouseTransform.position - weaponArm.transform.position;
        difference.Normalize();
        float rotation_z = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;

        if (facingleft)
        {
            weaponArm.transform.localRotation = Quaternion.Euler(0, 0, -rotation_z - 90);
        }
        else weaponArm.transform.localRotation = Quaternion.Euler(0f, 0f, rotation_z + 90);
    }
    //only possible in manual play
    public void Jump()
    {
        /*------- JUMP ---------- */
        int dirY = 0;

        if (isGrounded)
        {
            dirY = 1;

            var jumpForce = dirY * jumpHeight;

            m_AgentRb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse); //JUMP
            isGrounded = false;

            SoundInstance.InstantiateOnTransform(SoundInstance.GetClipFromLibrary("jump"), transform, 0.5f, false, SoundInstance.Randomization.Low);
        }
    }

    private void Melee(ActionSegment<int> act)
    {
        /*----   MELEE  ------*/
        int melee = act[1];

        if (melee == 1 && !meleeAtkin)
        {
            if (meleeMisscount > 2)
            {
                AddReward(-meleeMisscount / 50f); // melee miss
            }
            meleeMisscount++;
            GetComponent<Animator>().SetTrigger("melee_atk");
            meleeAtkin = true;
        }
        else
        {
            if (meleeMisscount > 0) meleeMisscount--;
            else meleeMisscount = 0;
            AddReward(0.02f);
        }
    }
    // function called by an Animation Event
    public void MeleeTriggerOff()
    {
        meleeAtkin = false;
    }

    private void StayCloseToCenter()
    {
        if (transform.localPosition.x > 10 || transform.localPosition.x < -10)
        {
            // print("far from center");
            AddReward(-0.002f);
        }
        else AddReward((10 - Mathf.Abs(transform.localPosition.x)) / 5000f);
    }

    //this reward usually given for killing enemy by bullets
    public void GiveBigReward()
    {
        AddReward(1f);
        mouseTransform.GetComponent<CursorAgent>().GiveBigReward();

        SimpleEnemyKillCount();
    }
    //Called by Enemy Unit when melee atk hits enemy.
    public void GiveSmallReward()
    {
        AddReward(.5f);

        SimpleEnemyKillCount();
    }

    void SimpleEnemyKillCount()
    {
        ++killcount;

        print("player_reward =" + GetCumulativeReward());
    }

    public void ShootMiss()
    {
        mouseTransform.GetComponent<CursorAgent>().AtkMiss();
    }

    public void GiveDamage(int dmg)
    {
        health -= dmg;
        AddReward(-0.6f);
        CameraShake.Shake(0.3f, 0.6f);
        SoundInstance.InstantiateOnTransform(SoundInstance.GetClipFromLibrary("hit"), transform, 0.5f, false, SoundInstance.Randomization.Low);

        if (health <= 0)
        {
            print("player_reward =" + GetCumulativeReward());
            mouseTransform.GetComponent<CursorAgent>().BadEnd();

            EndEpisode();
        }
    }

    public void GiveDamage()
    {
        GiveDamage(1);
    }
    // Event that runs every 5 sec 
    void OnGameFlowUpdate(object sender, GameEventArgs e)
    {
        movespeed = SettingsManager.Instance.playersetting.agentRunSpeed + e.playerMovespeed;

        fatStomach.localScale = new Vector2(1f - e.fatLossScale,
        fatStomach.localScale.y);
    }

    public void UseAbility1()
    {
        /* if (powerup)
         {
             //Adrenaline p = (Adrenaline)powerup;
             powerup.Fn();
        }*/
    }

    public void AddHealth(float num)
    {
        health = Mathf.Clamp(health + (int)num, 0, SettingsManager.Instance.playersetting.maxhealth);
    }

    void ObservationDebugging()
    {
        string s = " ";

        for (int i = 0; i < GetObservations().Count; i++)
        {
            s += " d" + i + "= " + GetObservations()[i] + "\n ";
        }
        print(s);
    }
    #endregion

}