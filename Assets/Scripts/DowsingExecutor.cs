using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
using System;

public class DowsingExecutor : MonoBehaviour
{
    [SerializeField] ActionBasedController actionBasedController;
    [SerializeField] Transform dowsingDirection;
    [SerializeField] MoveSpeedChanger moveSpeedChanger;
    public bool enableDowsing = false;
    public float playerDowsingMoveSpeedMultiplier = 0.9f;

    public void ToggleDowsingActive(InputAction.CallbackContext context)
    {
        if(context.performed)//https://unity2d.hateblo.jp/entry/2021/08/24/170308
        {
            if(enableDowsing)
            {
                enableDowsing = false;
                moveSpeedChanger.DividePlayerSpeed(playerDowsingMoveSpeedMultiplier);
            }
            else
            {
                enableDowsing = true;
                moveSpeedChanger.MultiplyPlayerSpeed(playerDowsingMoveSpeedMultiplier);
            }
        }
    }

    public void DowsingHaptic(Transform[] items)
    {
        if(!enableDowsing)
        {
            return;
        }

        Single haptic = 0f;
        foreach(var item in items)
        {
            if(item == null)
            {
                continue;
            }
            haptic = Math.Max(haptic, MathF.Pow(1 - Quaternion.Angle(dowsingDirection.rotation, Quaternion.AngleAxis(Vector3.SignedAngle(Vector3.forward, Vector3.ProjectOnPlane(item.position - dowsingDirection.position, Vector3.up), Vector3.up), Vector3.up)) / 180, 3));
        }
        if(actionBasedController != null)
        {
            actionBasedController.SendHapticImpulse(haptic, 0.05f);
        }
    }
}