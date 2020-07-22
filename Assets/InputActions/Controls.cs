// GENERATED AUTOMATICALLY FROM 'Assets/InputActions/Controls.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @Controls : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @Controls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""Controls"",
    ""maps"": [
        {
            ""name"": ""Gameplay"",
            ""id"": ""ae821fcc-9030-425e-bff1-53bf8a410b37"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""b6ab8017-9960-4a96-ae84-d3108d12c2fb"",
                    ""expectedControlType"": ""Stick"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""0c83fd8d-689a-4e7d-8b5a-f2c09f56b929"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Search"",
            ""id"": ""e306a262-8ba0-43a0-9d3f-d0f3595d260a"",
            ""actions"": [
                {
                    ""name"": ""Up"",
                    ""type"": ""Button"",
                    ""id"": ""7a42c484-7c87-4686-9214-be193ac946b5"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Down"",
                    ""type"": ""Button"",
                    ""id"": ""c45d68da-637b-412e-b0d8-493149579742"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Next"",
                    ""type"": ""Button"",
                    ""id"": ""2de1d612-ad1a-4a2e-8b30-659c3fb15736"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""8e194ac0-7d61-407b-b980-2ccb3d0b51c1"",
                    ""path"": ""<XInputController>/dpad/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Up"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""28f4ba38-5ef4-4420-a05a-d4a44e7122a6"",
                    ""path"": ""<XInputController>/leftStick/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Down"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""af562b4b-68c3-4ae3-b448-693b3ed408c1"",
                    ""path"": ""<XInputController>/dpad/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Next"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Gameplay
        m_Gameplay = asset.FindActionMap("Gameplay", throwIfNotFound: true);
        m_Gameplay_Move = m_Gameplay.FindAction("Move", throwIfNotFound: true);
        // Search
        m_Search = asset.FindActionMap("Search", throwIfNotFound: true);
        m_Search_Up = m_Search.FindAction("Up", throwIfNotFound: true);
        m_Search_Down = m_Search.FindAction("Down", throwIfNotFound: true);
        m_Search_Next = m_Search.FindAction("Next", throwIfNotFound: true);
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

    // Gameplay
    private readonly InputActionMap m_Gameplay;
    private IGameplayActions m_GameplayActionsCallbackInterface;
    private readonly InputAction m_Gameplay_Move;
    public struct GameplayActions
    {
        private @Controls m_Wrapper;
        public GameplayActions(@Controls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Move => m_Wrapper.m_Gameplay_Move;
        public InputActionMap Get() { return m_Wrapper.m_Gameplay; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(GameplayActions set) { return set.Get(); }
        public void SetCallbacks(IGameplayActions instance)
        {
            if (m_Wrapper.m_GameplayActionsCallbackInterface != null)
            {
                @Move.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMove;
                @Move.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMove;
                @Move.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMove;
            }
            m_Wrapper.m_GameplayActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Move.started += instance.OnMove;
                @Move.performed += instance.OnMove;
                @Move.canceled += instance.OnMove;
            }
        }
    }
    public GameplayActions @Gameplay => new GameplayActions(this);

    // Search
    private readonly InputActionMap m_Search;
    private ISearchActions m_SearchActionsCallbackInterface;
    private readonly InputAction m_Search_Up;
    private readonly InputAction m_Search_Down;
    private readonly InputAction m_Search_Next;
    public struct SearchActions
    {
        private @Controls m_Wrapper;
        public SearchActions(@Controls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Up => m_Wrapper.m_Search_Up;
        public InputAction @Down => m_Wrapper.m_Search_Down;
        public InputAction @Next => m_Wrapper.m_Search_Next;
        public InputActionMap Get() { return m_Wrapper.m_Search; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(SearchActions set) { return set.Get(); }
        public void SetCallbacks(ISearchActions instance)
        {
            if (m_Wrapper.m_SearchActionsCallbackInterface != null)
            {
                @Up.started -= m_Wrapper.m_SearchActionsCallbackInterface.OnUp;
                @Up.performed -= m_Wrapper.m_SearchActionsCallbackInterface.OnUp;
                @Up.canceled -= m_Wrapper.m_SearchActionsCallbackInterface.OnUp;
                @Down.started -= m_Wrapper.m_SearchActionsCallbackInterface.OnDown;
                @Down.performed -= m_Wrapper.m_SearchActionsCallbackInterface.OnDown;
                @Down.canceled -= m_Wrapper.m_SearchActionsCallbackInterface.OnDown;
                @Next.started -= m_Wrapper.m_SearchActionsCallbackInterface.OnNext;
                @Next.performed -= m_Wrapper.m_SearchActionsCallbackInterface.OnNext;
                @Next.canceled -= m_Wrapper.m_SearchActionsCallbackInterface.OnNext;
            }
            m_Wrapper.m_SearchActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Up.started += instance.OnUp;
                @Up.performed += instance.OnUp;
                @Up.canceled += instance.OnUp;
                @Down.started += instance.OnDown;
                @Down.performed += instance.OnDown;
                @Down.canceled += instance.OnDown;
                @Next.started += instance.OnNext;
                @Next.performed += instance.OnNext;
                @Next.canceled += instance.OnNext;
            }
        }
    }
    public SearchActions @Search => new SearchActions(this);
    public interface IGameplayActions
    {
        void OnMove(InputAction.CallbackContext context);
    }
    public interface ISearchActions
    {
        void OnUp(InputAction.CallbackContext context);
        void OnDown(InputAction.CallbackContext context);
        void OnNext(InputAction.CallbackContext context);
    }
}
