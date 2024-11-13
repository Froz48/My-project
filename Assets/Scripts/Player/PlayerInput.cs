using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

 public delegate void OnAbilityUse();
 public delegate void OnLeftMouseButton();
public class PlayerInput : NetworkBehaviour
{
    public OnAbilityUse[] onAbilityUse;
    public OnLeftMouseButton onHotbarButton;
    private PlayerInputActions playerInputActions;

    private void Awake(){
        onAbilityUse = new OnAbilityUse[4];
    }

    public override void OnNetworkSpawn(){
        playerInputActions = new PlayerInputActions();
        playerInputActions.Gameplay.HeadUse.performed += ctx => AbilityUse(0, ctx);
        playerInputActions.Gameplay.MainHandUse.performed += ctx => AbilityUse(1, ctx);
        playerInputActions.Gameplay.OffHandUse.performed += ctx => AbilityUse(2, ctx);
        playerInputActions.Gameplay.LegsUse.performed += ctx => AbilityUse(3, ctx);
        playerInputActions.Gameplay.Hotbar.performed += ctx => Hotbar();

        playerInputActions.Gameplay.Enable();
    }

    public Vector2 GetMovementVectorNormalized(){
        Vector2 inputVector = playerInputActions.Gameplay.Movement.ReadValue<Vector2>();
        inputVector = inputVector.normalized;
        return inputVector;
    }

    public void AbilityUse(int i, InputAction.CallbackContext obj){
        onAbilityUse[i]?.Invoke();
    }

    public void Hotbar(){
        Debug.Log("Hotbar");
        onHotbarButton.Invoke();
    }
}
