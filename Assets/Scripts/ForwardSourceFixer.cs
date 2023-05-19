using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;
using System;

public class ForwardSourceFixer : MonoBehaviour
{
    [SerializeField] RotationConstraint rotationConstraint;
    public void OnTriggerPressed(InputAction.CallbackContext context)
    {
        if(rotationConstraint != null && context.performed)//https://unity2d.hateblo.jp/entry/2021/08/24/170308
        {
            if(context.ReadValue<Single>() > 0.5)
            {
                rotationConstraint.enabled = false;
            }
            else
            {
                rotationConstraint.enabled = true;
            }
        }
    }
}