using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public delegate void OnAbilityUse();
public delegate void OnLeftMouseButton();

public class PlayerInputController : NetworkBehaviour
{
    public OnAbilityUse[] onAbilityUse;
    public OnLeftMouseButton onHotbarButton;
    [SerializeField] private InputActionAsset inputActionAsset;
    private PlayerInputActions playerInputActions;
    [SerializeField] private Animator animator;

    private void Awake()
    {
        onAbilityUse = new OnAbilityUse[4];
    }
    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            enabled = false;
            return;

        }

        InitializeInputSystem();
        EnableInput();
    }
    public override void OnNetworkDespawn()
    {
        DisableInput();
    }
    private void OnEnable()
    {
        EnableInput();
    }

    private void OnDisable()
    {
        DisableInput();
    }

    private void InitializeInputSystem()
    {
        // Инициализация системы ввода
        playerInputActions = new PlayerInputActions();
        
        // Подписка на события
        playerInputActions.Gameplay.HeadUse.performed += ctx => AbilityUse(0, ctx);
        playerInputActions.Gameplay.MainHandUse.performed += ctx => AbilityUse(1, ctx);
        playerInputActions.Gameplay.OffHandUse.performed += ctx => AbilityUse(2, ctx);
        playerInputActions.Gameplay.LegsUse.performed += ctx => AbilityUse(3, ctx);
        playerInputActions.Gameplay.Hotbar.performed += ctx => Hotbar();
        playerInputActions.Gameplay.Menu.performed += ctx => Menu();
        playerInputActions.Gameplay.Inventory.performed += ctx => Inventory();
    }

    public void EnableInput()
    {
        if (playerInputActions != null)
        {
            playerInputActions.Gameplay.Enable();
        }
        else if (inputActionAsset != null)
        {
            inputActionAsset.FindActionMap("Gameplay").Enable();
        }
    }

    public void DisableInput()
    {
        if (playerInputActions != null)
        {
            playerInputActions.Gameplay.Disable();
        }
        else if (inputActionAsset != null)
        {
            inputActionAsset.FindActionMap("Gameplay").Disable();
        }
    }

    public Vector2 GetMovementVectorNormalized()
    {
        if (playerInputActions == null || !IsOwner) return Vector2.zero;
        
        Vector2 inputVector = playerInputActions.Gameplay.Movement.ReadValue<Vector2>();
        inputVector = inputVector.normalized;

        animator?.SetFloat("MoveX", inputVector.x);
        animator?.SetFloat("MoveY", inputVector.y);

        return inputVector;
    }

    public void AbilityUse(int i, InputAction.CallbackContext obj)
    {
        onAbilityUse[i]?.Invoke();
    }

    public void Menu()
    {
        if (!FindObjectOfType<TradePostUI>().TryHide())
        {
            FindObjectOfType<WindowManager>().ChangeWindowState(6);
        }
    }

    public void Inventory()
    {
        FindObjectOfType<WindowManager>().ChangeWindowState(0);
        FindObjectOfType<WindowManager>().ChangeWindowState(1);
        FindObjectOfType<WindowManager>().ChangeWindowState(4);
    }

    public void Hotbar()
    {
        Debug.Log("Hotbar");
        onHotbarButton?.Invoke();
    }
}