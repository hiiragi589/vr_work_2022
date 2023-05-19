using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;
using System;

public class ItemUse : MonoBehaviour
{
    [SerializeField] PocketManager pocketManager;
    public void OnTriggerPressed(InputAction.CallbackContext context)
    {
        if(pocketManager != null && context.performed)//https://unity2d.hateblo.jp/entry/2021/08/24/170308
        {
            if(context.ReadValue<Single>() > 0.5)
            {
                pocketManager.Use();
            }
        }
    }
}