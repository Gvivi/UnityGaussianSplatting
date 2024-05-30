using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private static InputManager _instance;
    public static InputManager Instance {
        get {
            if (_instance == null && !Application.isEditor) {
                Debug.LogError("InputManager is NULL."); 
            }
            return _instance;
        }
    }

    [SerializeField] private bool isSimulation = false;

    private PlayerInput _input;

    public event Action<int> GSAssetChangeEvent;
    public event Action<float> SplatScaleChangeEvent;
    public event Action<Vector2> MoveEvent;
    public event Action<Vector2> LookEvent;
    
    private void Awake() {
        _instance = this;

        _input = new PlayerInput();
        _input.Simulation.Look.performed += ctx => HandleLook(ctx);
        _input.Simulation.ChangeGSAsset.started += ctx => HandleChangeGSAsset(ctx);
        _input.Simulation.ChangeSplatScale.started += ctx => HandleChangeSplatScale(ctx);
        _input.Installation.ChangeGSAsset.started += ctx => HandleChangeGSAsset(ctx);
        _input.Installation.ChangeSplatScale.started += ctx => HandleChangeSplatScale(ctx);
    }

    private void OnEnable() {
        if(isSimulation){
            _input.Simulation.Enable();
            _input.Installation.Disable();
        } else {
            _input.Installation.Enable();
            _input.Simulation.Disable();
        }
    }

    private void OnDisable() {
        _input.Disable();
    }

    private void FixedUpdate() {
        MoveEvent?.Invoke(_input.Simulation.Move.ReadValue<Vector2>());
    }

    public void HandleLook(InputAction.CallbackContext context)
    {
        LookEvent?.Invoke(context.ReadValue<Vector2>());
    }

    private void HandleChangeGSAsset(InputAction.CallbackContext context){
        float input = context.ReadValue<float>();
        int value = (input > 0) ? 1 : -1;
        GSAssetChangeEvent?.Invoke(value);
    }

    private void HandleChangeSplatScale(InputAction.CallbackContext context){
        SplatScaleChangeEvent?.Invoke(context.ReadValue<float>());
    }
}
