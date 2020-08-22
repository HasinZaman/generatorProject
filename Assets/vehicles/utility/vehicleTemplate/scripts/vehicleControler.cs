// GENERATED AUTOMATICALLY FROM 'Assets/vehicles/utility/vehicleTemplate/scripts/vehicleControler.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @VehicleControler : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @VehicleControler()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""vehicleControler"",
    ""maps"": [
        {
            ""name"": ""land"",
            ""id"": ""45e1ed44-8901-40fa-8eaf-c875eea43313"",
            ""actions"": [
                {
                    ""name"": ""movement"",
                    ""type"": ""PassThrough"",
                    ""id"": ""8f3442c5-ed60-4fe7-a599-978c143ca709"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""gear"",
                    ""type"": ""PassThrough"",
                    ""id"": ""49c0136d-70a9-4e83-bb74-31765ba1455b"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Press""
                },
                {
                    ""name"": ""brake"",
                    ""type"": ""PassThrough"",
                    ""id"": ""cb05ecb3-c3ce-4e3f-9c2b-0c0287e0d27a"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""turretMovement"",
                    ""type"": ""PassThrough"",
                    ""id"": ""37570ddc-bfc4-4f19-86a1-49c2c555ba59"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""shoot"",
                    ""type"": ""PassThrough"",
                    ""id"": ""12b7b602-ba5e-4749-94a3-ab458d95a91d"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""turretChange"",
                    ""type"": ""PassThrough"",
                    ""id"": ""3a0aa2ee-3e93-4d92-a4e9-db93bfbe905d"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""toggleLight"",
                    ""type"": ""PassThrough"",
                    ""id"": ""e10331de-d4e4-477f-a979-49d2540fe331"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Press(behavior=2)""
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""1D Axis"",
                    ""id"": ""2e95137c-95c3-4a24-be76-2ef02a0f9fd7"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""gear"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""917c41a8-7b9a-44f0-ae2c-81b38c9bc535"",
                    ""path"": ""<Keyboard>/ctrl"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""keyBoard"",
                    ""action"": ""gear"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""49b849aa-cbde-4315-bce9-a8e39f2768c6"",
                    ""path"": ""<Keyboard>/leftShift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""keyBoard"",
                    ""action"": ""gear"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""f11a0e83-6718-4457-b08b-9e696706fbfe"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": ""Hold"",
                    ""processors"": """",
                    ""groups"": ""keyBoard"",
                    ""action"": ""brake"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""6d8c191d-bda1-4a2c-9206-f699645a9bc6"",
                    ""path"": ""2DVector(mode=1)"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""ece7cb6d-eb37-42fe-9218-55f59b227340"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""keyBoard"",
                    ""action"": ""movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""7200fb76-d592-4cd0-ab27-85e0ba92a95d"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""keyBoard"",
                    ""action"": ""movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""aaca0ed6-d91a-4c8e-bc72-236785980d2f"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""keyBoard"",
                    ""action"": ""movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""0d8cb202-b205-455c-b0ba-1a728b613a1e"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""keyBoard"",
                    ""action"": ""movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""f78940b8-7b74-4c76-9656-2fc5257ca087"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""turretMovement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""00fa4f22-e9de-4f42-af07-33bbf31c9d48"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""turretMovement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""850dcdd6-556f-46ff-bc2d-be171b8ce416"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""turretMovement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""9a24b5f0-b439-4f28-a4c7-143e384d6b2b"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""turretMovement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""ab7d18f9-8a33-4898-aeb8-e261d7764337"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""turretMovement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""bf5422bc-f4dc-462a-a30f-edd39f8b6685"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""keyBoard"",
                    ""action"": ""shoot"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""1D Axis"",
                    ""id"": ""611027d1-2d9d-48fe-9777-14ae962f56dc"",
                    ""path"": ""1DAxis"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""turretChange"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""a3d5f7cc-c977-486a-8cb2-8608328c3025"",
                    ""path"": ""<Keyboard>/q"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""keyBoard"",
                    ""action"": ""turretChange"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""95e7a428-9d97-4732-8eae-2f64653e4ce7"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""keyBoard"",
                    ""action"": ""turretChange"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""a36de280-203f-4392-9444-0e1aef33f353"",
                    ""path"": ""<Keyboard>/f"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""toggleLight"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""keyBoard"",
            ""bindingGroup"": ""keyBoard"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Mouse>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // land
        m_land = asset.FindActionMap("land", throwIfNotFound: true);
        m_land_movement = m_land.FindAction("movement", throwIfNotFound: true);
        m_land_gear = m_land.FindAction("gear", throwIfNotFound: true);
        m_land_brake = m_land.FindAction("brake", throwIfNotFound: true);
        m_land_turretMovement = m_land.FindAction("turretMovement", throwIfNotFound: true);
        m_land_shoot = m_land.FindAction("shoot", throwIfNotFound: true);
        m_land_turretChange = m_land.FindAction("turretChange", throwIfNotFound: true);
        m_land_toggleLight = m_land.FindAction("toggleLight", throwIfNotFound: true);
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

    // land
    private readonly InputActionMap m_land;
    private ILandActions m_LandActionsCallbackInterface;
    private readonly InputAction m_land_movement;
    private readonly InputAction m_land_gear;
    private readonly InputAction m_land_brake;
    private readonly InputAction m_land_turretMovement;
    private readonly InputAction m_land_shoot;
    private readonly InputAction m_land_turretChange;
    private readonly InputAction m_land_toggleLight;
    public struct LandActions
    {
        private @VehicleControler m_Wrapper;
        public LandActions(@VehicleControler wrapper) { m_Wrapper = wrapper; }
        public InputAction @movement => m_Wrapper.m_land_movement;
        public InputAction @gear => m_Wrapper.m_land_gear;
        public InputAction @brake => m_Wrapper.m_land_brake;
        public InputAction @turretMovement => m_Wrapper.m_land_turretMovement;
        public InputAction @shoot => m_Wrapper.m_land_shoot;
        public InputAction @turretChange => m_Wrapper.m_land_turretChange;
        public InputAction @toggleLight => m_Wrapper.m_land_toggleLight;
        public InputActionMap Get() { return m_Wrapper.m_land; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(LandActions set) { return set.Get(); }
        public void SetCallbacks(ILandActions instance)
        {
            if (m_Wrapper.m_LandActionsCallbackInterface != null)
            {
                @movement.started -= m_Wrapper.m_LandActionsCallbackInterface.OnMovement;
                @movement.performed -= m_Wrapper.m_LandActionsCallbackInterface.OnMovement;
                @movement.canceled -= m_Wrapper.m_LandActionsCallbackInterface.OnMovement;
                @gear.started -= m_Wrapper.m_LandActionsCallbackInterface.OnGear;
                @gear.performed -= m_Wrapper.m_LandActionsCallbackInterface.OnGear;
                @gear.canceled -= m_Wrapper.m_LandActionsCallbackInterface.OnGear;
                @brake.started -= m_Wrapper.m_LandActionsCallbackInterface.OnBrake;
                @brake.performed -= m_Wrapper.m_LandActionsCallbackInterface.OnBrake;
                @brake.canceled -= m_Wrapper.m_LandActionsCallbackInterface.OnBrake;
                @turretMovement.started -= m_Wrapper.m_LandActionsCallbackInterface.OnTurretMovement;
                @turretMovement.performed -= m_Wrapper.m_LandActionsCallbackInterface.OnTurretMovement;
                @turretMovement.canceled -= m_Wrapper.m_LandActionsCallbackInterface.OnTurretMovement;
                @shoot.started -= m_Wrapper.m_LandActionsCallbackInterface.OnShoot;
                @shoot.performed -= m_Wrapper.m_LandActionsCallbackInterface.OnShoot;
                @shoot.canceled -= m_Wrapper.m_LandActionsCallbackInterface.OnShoot;
                @turretChange.started -= m_Wrapper.m_LandActionsCallbackInterface.OnTurretChange;
                @turretChange.performed -= m_Wrapper.m_LandActionsCallbackInterface.OnTurretChange;
                @turretChange.canceled -= m_Wrapper.m_LandActionsCallbackInterface.OnTurretChange;
                @toggleLight.started -= m_Wrapper.m_LandActionsCallbackInterface.OnToggleLight;
                @toggleLight.performed -= m_Wrapper.m_LandActionsCallbackInterface.OnToggleLight;
                @toggleLight.canceled -= m_Wrapper.m_LandActionsCallbackInterface.OnToggleLight;
            }
            m_Wrapper.m_LandActionsCallbackInterface = instance;
            if (instance != null)
            {
                @movement.started += instance.OnMovement;
                @movement.performed += instance.OnMovement;
                @movement.canceled += instance.OnMovement;
                @gear.started += instance.OnGear;
                @gear.performed += instance.OnGear;
                @gear.canceled += instance.OnGear;
                @brake.started += instance.OnBrake;
                @brake.performed += instance.OnBrake;
                @brake.canceled += instance.OnBrake;
                @turretMovement.started += instance.OnTurretMovement;
                @turretMovement.performed += instance.OnTurretMovement;
                @turretMovement.canceled += instance.OnTurretMovement;
                @shoot.started += instance.OnShoot;
                @shoot.performed += instance.OnShoot;
                @shoot.canceled += instance.OnShoot;
                @turretChange.started += instance.OnTurretChange;
                @turretChange.performed += instance.OnTurretChange;
                @turretChange.canceled += instance.OnTurretChange;
                @toggleLight.started += instance.OnToggleLight;
                @toggleLight.performed += instance.OnToggleLight;
                @toggleLight.canceled += instance.OnToggleLight;
            }
        }
    }
    public LandActions @land => new LandActions(this);
    private int m_keyBoardSchemeIndex = -1;
    public InputControlScheme keyBoardScheme
    {
        get
        {
            if (m_keyBoardSchemeIndex == -1) m_keyBoardSchemeIndex = asset.FindControlSchemeIndex("keyBoard");
            return asset.controlSchemes[m_keyBoardSchemeIndex];
        }
    }
    public interface ILandActions
    {
        void OnMovement(InputAction.CallbackContext context);
        void OnGear(InputAction.CallbackContext context);
        void OnBrake(InputAction.CallbackContext context);
        void OnTurretMovement(InputAction.CallbackContext context);
        void OnShoot(InputAction.CallbackContext context);
        void OnTurretChange(InputAction.CallbackContext context);
        void OnToggleLight(InputAction.CallbackContext context);
    }
}
