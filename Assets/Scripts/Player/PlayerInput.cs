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
    [SerializeField] private Animator animator;

    private void Awake(){
        onAbilityUse = new OnAbilityUse[4];
        //animator = GetComponent<Animator>();
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

        //bool isMoving = inputVector.magnitude > 0.1f;
        //animator.SetBool("IsMoving", isMoving);
        //if(isMoving)
        //{
            //float angle = Mathf.Atan2(inputVector.y, inputVector.x) * Mathf.Rad2Deg;
            animator.SetFloat("MoveX", inputVector.x);
            animator.SetFloat("MoveY", inputVector.y);
        //}

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
