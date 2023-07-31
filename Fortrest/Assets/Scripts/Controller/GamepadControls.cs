//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.4.4
//     from Assets/Scripts/Controller/GamepadControls.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @GamepadControls : IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @GamepadControls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""GamepadControls"",
    ""maps"": [
        {
            ""name"": ""Controls"",
            ""id"": ""a66ba898-db65-4dd6-8687-2062ab74ceff"",
            ""actions"": [
                {
                    ""name"": ""Sprint"",
                    ""type"": ""Button"",
                    ""id"": ""cd0da87f-f992-4559-94e3-d18d371f4bd8"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""5543b14c-3aa2-41a2-8917-ea0a39fb09a2"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Pause"",
                    ""type"": ""Button"",
                    ""id"": ""efa259b7-28d4-40d7-a414-014f702e78ff"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Gathering"",
                    ""type"": ""Button"",
                    ""id"": ""e1b1042a-1773-4989-b30d-62a62f08a781"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Attacking"",
                    ""type"": ""Button"",
                    ""id"": ""1a0a28e1-8eba-4039-b2fb-ae4d0af6b075"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Aiming"",
                    ""type"": ""Button"",
                    ""id"": ""86ab70cd-e0f1-4756-beeb-ef2504a9023a"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Interact"",
                    ""type"": ""Button"",
                    ""id"": ""a5c4cbcb-7cfb-449a-b5e5-702f1abb1def"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Map"",
                    ""type"": ""Button"",
                    ""id"": ""5fb0f7fd-a93d-49aa-8776-7355b6744a8a"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Swap"",
                    ""type"": ""Button"",
                    ""id"": ""effb0c60-5d98-428c-86db-988d8d3005f8"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Evade"",
                    ""type"": ""Button"",
                    ""id"": ""75ededa6-357b-493e-a650-5cb1da35335f"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Heal"",
                    ""type"": ""Button"",
                    ""id"": ""b6f1ebff-9c48-44a1-9bfa-db06c1019078"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Turret"",
                    ""type"": ""Button"",
                    ""id"": ""eb479ea0-f943-4bac-beb7-ed136154e9b9"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""d7dcaf20-f3e1-4005-a9b4-508311a98af1"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Sprint"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b84bcb03-c061-4e24-9084-0b755e30f9fa"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f553c906-db9f-4dad-8866-0b9e1abe69ea"",
                    ""path"": ""<Gamepad>/start"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Pause"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f368e9d4-10d2-4c34-933e-b2e717ba6ea8"",
                    ""path"": ""<Gamepad>/rightTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Gathering"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a9ed920b-afa6-4da2-a404-5b5f0044da16"",
                    ""path"": ""<Gamepad>/rightTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Attacking"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""3ded1994-292c-461c-b483-240ee737e9f9"",
                    ""path"": ""<Gamepad>/buttonWest"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Interact"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2024ec19-b839-4fbe-a4fb-6a2601bf24b1"",
                    ""path"": ""<Gamepad>/select"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Map"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""907b66e7-d034-4733-a382-aa8f87ec422d"",
                    ""path"": ""<Gamepad>/buttonNorth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Swap"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""95502650-db7c-46b5-be06-0694f2da419e"",
                    ""path"": ""<Gamepad>/leftTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Aiming"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""65752130-a0c7-4ce8-b0b9-d2e753b16ce9"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Evade"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""54dd2816-5cfe-4039-b6b0-ab11cf9584f2"",
                    ""path"": ""<Gamepad>/leftShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Heal"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""46f861a7-e4e7-4248-a54c-67cb3fd44dad"",
                    ""path"": ""<Gamepad>/rightShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Turret"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Controls
        m_Controls = asset.FindActionMap("Controls", throwIfNotFound: true);
        m_Controls_Sprint = m_Controls.FindAction("Sprint", throwIfNotFound: true);
        m_Controls_Move = m_Controls.FindAction("Move", throwIfNotFound: true);
        m_Controls_Pause = m_Controls.FindAction("Pause", throwIfNotFound: true);
        m_Controls_Gathering = m_Controls.FindAction("Gathering", throwIfNotFound: true);
        m_Controls_Attacking = m_Controls.FindAction("Attacking", throwIfNotFound: true);
        m_Controls_Aiming = m_Controls.FindAction("Aiming", throwIfNotFound: true);
        m_Controls_Interact = m_Controls.FindAction("Interact", throwIfNotFound: true);
        m_Controls_Map = m_Controls.FindAction("Map", throwIfNotFound: true);
        m_Controls_Swap = m_Controls.FindAction("Swap", throwIfNotFound: true);
        m_Controls_Evade = m_Controls.FindAction("Evade", throwIfNotFound: true);
        m_Controls_Heal = m_Controls.FindAction("Heal", throwIfNotFound: true);
        m_Controls_Turret = m_Controls.FindAction("Turret", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }
    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }
    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // Controls
    private readonly InputActionMap m_Controls;
    private IControlsActions m_ControlsActionsCallbackInterface;
    private readonly InputAction m_Controls_Sprint;
    private readonly InputAction m_Controls_Move;
    private readonly InputAction m_Controls_Pause;
    private readonly InputAction m_Controls_Gathering;
    private readonly InputAction m_Controls_Attacking;
    private readonly InputAction m_Controls_Aiming;
    private readonly InputAction m_Controls_Interact;
    private readonly InputAction m_Controls_Map;
    private readonly InputAction m_Controls_Swap;
    private readonly InputAction m_Controls_Evade;
    private readonly InputAction m_Controls_Heal;
    private readonly InputAction m_Controls_Turret;
    public struct ControlsActions
    {
        private @GamepadControls m_Wrapper;
        public ControlsActions(@GamepadControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Sprint => m_Wrapper.m_Controls_Sprint;
        public InputAction @Move => m_Wrapper.m_Controls_Move;
        public InputAction @Pause => m_Wrapper.m_Controls_Pause;
        public InputAction @Gathering => m_Wrapper.m_Controls_Gathering;
        public InputAction @Attacking => m_Wrapper.m_Controls_Attacking;
        public InputAction @Aiming => m_Wrapper.m_Controls_Aiming;
        public InputAction @Interact => m_Wrapper.m_Controls_Interact;
        public InputAction @Map => m_Wrapper.m_Controls_Map;
        public InputAction @Swap => m_Wrapper.m_Controls_Swap;
        public InputAction @Evade => m_Wrapper.m_Controls_Evade;
        public InputAction @Heal => m_Wrapper.m_Controls_Heal;
        public InputAction @Turret => m_Wrapper.m_Controls_Turret;
        public InputActionMap Get() { return m_Wrapper.m_Controls; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(ControlsActions set) { return set.Get(); }
        public void SetCallbacks(IControlsActions instance)
        {
            if (m_Wrapper.m_ControlsActionsCallbackInterface != null)
            {
                @Sprint.started -= m_Wrapper.m_ControlsActionsCallbackInterface.OnSprint;
                @Sprint.performed -= m_Wrapper.m_ControlsActionsCallbackInterface.OnSprint;
                @Sprint.canceled -= m_Wrapper.m_ControlsActionsCallbackInterface.OnSprint;
                @Move.started -= m_Wrapper.m_ControlsActionsCallbackInterface.OnMove;
                @Move.performed -= m_Wrapper.m_ControlsActionsCallbackInterface.OnMove;
                @Move.canceled -= m_Wrapper.m_ControlsActionsCallbackInterface.OnMove;
                @Pause.started -= m_Wrapper.m_ControlsActionsCallbackInterface.OnPause;
                @Pause.performed -= m_Wrapper.m_ControlsActionsCallbackInterface.OnPause;
                @Pause.canceled -= m_Wrapper.m_ControlsActionsCallbackInterface.OnPause;
                @Gathering.started -= m_Wrapper.m_ControlsActionsCallbackInterface.OnGathering;
                @Gathering.performed -= m_Wrapper.m_ControlsActionsCallbackInterface.OnGathering;
                @Gathering.canceled -= m_Wrapper.m_ControlsActionsCallbackInterface.OnGathering;
                @Attacking.started -= m_Wrapper.m_ControlsActionsCallbackInterface.OnAttacking;
                @Attacking.performed -= m_Wrapper.m_ControlsActionsCallbackInterface.OnAttacking;
                @Attacking.canceled -= m_Wrapper.m_ControlsActionsCallbackInterface.OnAttacking;
                @Aiming.started -= m_Wrapper.m_ControlsActionsCallbackInterface.OnAiming;
                @Aiming.performed -= m_Wrapper.m_ControlsActionsCallbackInterface.OnAiming;
                @Aiming.canceled -= m_Wrapper.m_ControlsActionsCallbackInterface.OnAiming;
                @Interact.started -= m_Wrapper.m_ControlsActionsCallbackInterface.OnInteract;
                @Interact.performed -= m_Wrapper.m_ControlsActionsCallbackInterface.OnInteract;
                @Interact.canceled -= m_Wrapper.m_ControlsActionsCallbackInterface.OnInteract;
                @Map.started -= m_Wrapper.m_ControlsActionsCallbackInterface.OnMap;
                @Map.performed -= m_Wrapper.m_ControlsActionsCallbackInterface.OnMap;
                @Map.canceled -= m_Wrapper.m_ControlsActionsCallbackInterface.OnMap;
                @Swap.started -= m_Wrapper.m_ControlsActionsCallbackInterface.OnSwap;
                @Swap.performed -= m_Wrapper.m_ControlsActionsCallbackInterface.OnSwap;
                @Swap.canceled -= m_Wrapper.m_ControlsActionsCallbackInterface.OnSwap;
                @Evade.started -= m_Wrapper.m_ControlsActionsCallbackInterface.OnEvade;
                @Evade.performed -= m_Wrapper.m_ControlsActionsCallbackInterface.OnEvade;
                @Evade.canceled -= m_Wrapper.m_ControlsActionsCallbackInterface.OnEvade;
                @Heal.started -= m_Wrapper.m_ControlsActionsCallbackInterface.OnHeal;
                @Heal.performed -= m_Wrapper.m_ControlsActionsCallbackInterface.OnHeal;
                @Heal.canceled -= m_Wrapper.m_ControlsActionsCallbackInterface.OnHeal;
                @Turret.started -= m_Wrapper.m_ControlsActionsCallbackInterface.OnTurret;
                @Turret.performed -= m_Wrapper.m_ControlsActionsCallbackInterface.OnTurret;
                @Turret.canceled -= m_Wrapper.m_ControlsActionsCallbackInterface.OnTurret;
            }
            m_Wrapper.m_ControlsActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Sprint.started += instance.OnSprint;
                @Sprint.performed += instance.OnSprint;
                @Sprint.canceled += instance.OnSprint;
                @Move.started += instance.OnMove;
                @Move.performed += instance.OnMove;
                @Move.canceled += instance.OnMove;
                @Pause.started += instance.OnPause;
                @Pause.performed += instance.OnPause;
                @Pause.canceled += instance.OnPause;
                @Gathering.started += instance.OnGathering;
                @Gathering.performed += instance.OnGathering;
                @Gathering.canceled += instance.OnGathering;
                @Attacking.started += instance.OnAttacking;
                @Attacking.performed += instance.OnAttacking;
                @Attacking.canceled += instance.OnAttacking;
                @Aiming.started += instance.OnAiming;
                @Aiming.performed += instance.OnAiming;
                @Aiming.canceled += instance.OnAiming;
                @Interact.started += instance.OnInteract;
                @Interact.performed += instance.OnInteract;
                @Interact.canceled += instance.OnInteract;
                @Map.started += instance.OnMap;
                @Map.performed += instance.OnMap;
                @Map.canceled += instance.OnMap;
                @Swap.started += instance.OnSwap;
                @Swap.performed += instance.OnSwap;
                @Swap.canceled += instance.OnSwap;
                @Evade.started += instance.OnEvade;
                @Evade.performed += instance.OnEvade;
                @Evade.canceled += instance.OnEvade;
                @Heal.started += instance.OnHeal;
                @Heal.performed += instance.OnHeal;
                @Heal.canceled += instance.OnHeal;
                @Turret.started += instance.OnTurret;
                @Turret.performed += instance.OnTurret;
                @Turret.canceled += instance.OnTurret;
            }
        }
    }
    public ControlsActions @Controls => new ControlsActions(this);
    public interface IControlsActions
    {
        void OnSprint(InputAction.CallbackContext context);
        void OnMove(InputAction.CallbackContext context);
        void OnPause(InputAction.CallbackContext context);
        void OnGathering(InputAction.CallbackContext context);
        void OnAttacking(InputAction.CallbackContext context);
        void OnAiming(InputAction.CallbackContext context);
        void OnInteract(InputAction.CallbackContext context);
        void OnMap(InputAction.CallbackContext context);
        void OnSwap(InputAction.CallbackContext context);
        void OnEvade(InputAction.CallbackContext context);
        void OnHeal(InputAction.CallbackContext context);
        void OnTurret(InputAction.CallbackContext context);
    }
}
