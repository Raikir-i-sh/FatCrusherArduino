using System.Collections;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Policies;

public class CursorAgent : Agent
{
    public Transform bulletspawn;
    public AreaDetect areadetect;
    public Transform player;

    public GameObject bulletPrefab;
    public ParticleSystem bulletspark;
    public AudioClip sound_shoot;

    private float fireRate = 0.5f; // 0.2 to 0.8
    private float nextFire = 0f;
    private int bulletcount = 0;
    private int bulletmiss = 0;
    private bool canshoot;
    #region Unity Methods

    void Start()
    {
        GetComponent<BehaviorParameters>().BehaviorType = (BehaviorType)SettingsManager.Instance.inputmode;
        GameflowChanger.Instance.RaiseEvent += OnGameFlowUpdate;
    }

    #endregion

    #region ML agent 
    public override void Initialize()
    {
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        int numOfEnemyToObserve = 1;
        float maxDistX = 8.5f;
        float maxDistY = 4.5f;

        // if not enough enemy to observe , add 0 to obs to occupy correct slots.
        canshoot = areadetect.enemiesInRange.Count > 0;
        for (int i = 0; i < numOfEnemyToObserve; i++)
        {
            if (i <= (areadetect.enemiesInRange.Count - 1))
            {
                if (areadetect.enemiesInRange[i] != null)
                {
                    Vector2 v = transform.InverseTransformDirection(areadetect.enemiesInRange[i].position - transform.position);
                    sensor.AddObservation(v.magnitude);
                    sensor.AddObservation(Mathf.Clamp(v.x / maxDistX, -1, 1)); //normalized x distance for M-E
                    sensor.AddObservation(Mathf.Clamp(v.y / maxDistY, -1, 1));
                }
            }
            else
            {
                sensor.AddObservation(0);
                sensor.AddObservation(0);
                sensor.AddObservation(0);
            }
        }
        try
        {
            sensor.AddObservation(Mathf.Clamp((float)bulletmiss / (float)bulletcount, 0f, 1f));
        }
        catch (System.Exception)
        {
            sensor.AddObservation(0);
        }
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)

    {
        Shoot(actionBuffers.DiscreteActions);

        MoveMousePosition(actionBuffers.DiscreteActions);
    }
    int shootInput = 0;

    public void ShootIp(int s)
    {
        shootInput = s;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;

        discreteActionsOut[2] = shootInput; //shoot
        discreteActionsOut[0] = 0;
        discreteActionsOut[1] = 0;
        /* Done directly by InputController class
                if (Input.GetKey(KeyCode.J))
                {
                    discreteActionsOut[0] = 1; // mouse right
                }
                else if (Input.GetKey(KeyCode.L))
                {
                    discreteActionsOut[0] = 2;
                }

                if (Input.GetKey(KeyCode.I))
                {
                    discreteActionsOut[1] = 1; // mouse up
                }
                else if (Input.GetKey(KeyCode.K))
                {
                    discreteActionsOut[1] = 2;
                }
        */
        // ObservationDebugging();
    }

    public override void WriteDiscreteActionMask(IDiscreteActionMask actionMask)
    {
        //unable to shoot n melee during melee_animation
        actionMask.SetActionEnabled(2, 1, canshoot); //branch 2,indx 1 is enabled when true.
    }

    public override void OnEpisodeBegin()
    {
        bulletcount = 0;
        canshoot = true;
        transform.localPosition = Vector3.zero;
        ReloadBullets();
    }
    #endregion

    #region My methods
    public void MoveMousePosition(ActionSegment<int> act)
    {
        float xdir = 0, ydir = 0;
        int horizontal = act[0];
        int vertical = act[1];

        switch (horizontal)
        {
            case 1:
                xdir = -1f;
                break;
            case 2:
                xdir = 1f;
                break;
        }
        switch (vertical)
        {
            case 1:
                ydir = 1f;
                break;
            case 2:
                ydir = -1f;
                break;
        }

        float xPos = Mathf.Clamp(transform.localPosition.x + xdir * 18 * Time.deltaTime,
            player.transform.localPosition.x - 8.5f,
            player.transform.localPosition.x + 8.5f);
        float yPos = Mathf.Clamp(transform.localPosition.y + ydir * 9 * Time.deltaTime,
            player.transform.localPosition.y - 2.5f,
            player.transform.localPosition.y + 6.5f);

        transform.localPosition = new Vector3(xPos, yPos, transform.localPosition.z); //move mouse
        GoodAimReward();
    }

    public void Shoot(ActionSegment<int> act)
    {
        if (bulletspawn)
        {
            int shoot = act[2];

            if (shoot == 1 && Time.time > nextFire)
            {
                GoodShotReward();
                nextFire = Time.time + fireRate;

                Vector3 dir = transform.position - bulletspawn.position;
                dir.Normalize();
                float rotation_z = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                GameObject bullet = AdvPoolingSystem.Instance.InstantiateAPS(bulletPrefab.name, bulletspawn.position, Quaternion.Euler(0, 0, rotation_z));
                bullet.GetComponent<BulletOptimized>().SetShooter(player.gameObject);

                bulletcount += 1;
                AddReward(CalcAccuracy() / 1000f);
                bulletspark.Play();
                SoundInstance.InstantiateOnTransform(sound_shoot, transform, 0.3f, false,
                SoundInstance.Randomization.Low);
            }
        }
    }

    void GoodAimReward()
    {
        float maxNegReward = 0.005f, minNegReward = 0.0005f;

        for (int i = 0; i < GetObservations().Count; i = i + 5)
        {
            if (GetObservations()[i] == 0) continue;
            if (areadetect.enemiesInRange.Count <= 0) return;

            Vector2 v = transform.InverseTransformDirection(areadetect.enemiesInRange[i].position - transform.position);
            float reward = minNegReward + (maxNegReward - minNegReward) *
            v.magnitude / 12f;
            AddReward(-reward);
            break;
        }
    }

    void GoodShotReward()
    {
        for (int i = 0; i < GetObservations().Count; i = i + 4)
        {
            if (GetObservations()[i] == 0) continue;
            if (GetObservations()[i] > 0.985f)
            {
                AddReward(0.7f);
                print("aim shot= " + GetCumulativeReward());
            }
            if ((GetObservations()[i + 1] / 8.5) < 0.08f)
            {
                print("good shot=" + GetCumulativeReward());
                AddReward(1f);
                //  return; //so that it rewards only once per step.
            }
        }
    }
    //called when bullet hits enemy
    public void GiveBigReward()
    {
        AddReward(0.3f);
    }
    /// won't be called for now
    public void GoodEnd()
    {
        print("cursor_reward =" + GetCumulativeReward());
        EndEpisode();
    }
    //called by Player when it misses shot
    public void AtkMiss()
    {
        bulletmiss += 1;
    }

    private float CalcAccuracy()
    {
        try
        {
            float accuracy = (float)(bulletcount - bulletmiss) / ((float)bulletcount);
            print("accuracy=" + accuracy);
            if (accuracy <= 1f || accuracy >= 0.8f) return 0.5f;
            if (accuracy < 0.8f && accuracy > 0.3f) return 0.3f;
            if (accuracy < 0.3f) return 0f;
            if (accuracy < 0.1f) return -0.1f;
        }
        catch (System.Exception)
        {
            return 0f;
        }
        return 0f;
    }

    private void ReloadBullets()
    {
        bulletcount = 0;
        bulletmiss = 0;
    }

    public void BadEnd()
    {
        print("cursor_reward =" + GetCumulativeReward());
        AddReward(-0.5f);
        EndEpisode();
    }
    // Event that runs every 5 sec 
    void OnGameFlowUpdate(object sender, GameEventArgs e)
    {
        fireRate = SettingsManager.Instance.playersetting.fireRate - e.shootDecreaseRate;
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