using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.MLAgents.Policies;

public class MyInputController : MonoBehaviour
{
    public FatPlayerAgent player;
    [HideInInspector] public int atkNum;
    public CursorAgent cursor;
    public PlayerAnimation anim;
    bool isHeuristic;

    void Start()
    {
        Invoke("CheckHeuristic", 2f);
    }

    void CheckHeuristic()
    {
        isHeuristic = GetComponent<BehaviorParameters>().IsInHeuristicMode();
    }

    // Update is called once per frame
    void Update()
    {
        if (isHeuristic) MouseHovering();
    }

    void MouseHovering()
    {
        Vector3 mousePosition = Mouse.current.position.ReadValue();

        mousePosition.z = Camera.main.nearClipPlane;
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        //Debug.Log(worldPosition);
        cursor.gameObject.transform.position = worldPosition;
    }

    #region Input Callbacks
    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.ReadValue<Vector2>().x > 0)
            player.SetMoveIp(1);
        else if (context.ReadValue<Vector2>().x < 0)
            player.SetMoveIp(2);
        else player.SetMoveIp(0);
        anim.GotoRun();

        if (context.canceled)
        {
            anim.GotoIdle();
        }
    }

    public void OnRightClk(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            player.SetMeleeIp(1);
            anim.GotoMeleeAtk();
        }
        if (context.canceled)
        {
            player.SetMeleeIp(0);
            anim.Reset_Trigger("melee_atk");
        }
    }

    public void OnLeftClk(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            cursor.ShootIp(1);
            // anim.GotoRangedAtk();
        }
        if (context.canceled)
        {
            cursor.ShootIp(0);
            //  anim.Reset_Trigger("melee_atk");
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        // Debug.Log("jumped");
        player.Jump();
    }

    public void OnUseAbility1(InputAction.CallbackContext context)
    {
        Debug.Log("ability1 used");
        player.UseAbility1();
    }
    #endregion

}