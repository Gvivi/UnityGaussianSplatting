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

    public event Action<int> GSAssetIndex1Event;
    public event Action<int> GSAssetIndex2Event;
    public event Action<float> SplatScaleEvent;
    public event Action<Vector2> MoveEvent;
    public event Action<Vector2> LookEvent;
    
    private void Awake() {
        _instance = this;

        _input = new PlayerInput();
        _input.Simulation.Look.performed += ctx => HandleLook(ctx);
        _input.Simulation.GSAssetIndex1.started += ctx => HandleGSAssetIndex1(ctx);
        _input.Simulation.GSAssetIndex2.started += ctx => HandleGSAssetIndex2(ctx);
        _input.Simulation.SplatScale.started += ctx => HandleSplatScale(ctx);
        _input.Installation.GSAssetIndex1.started += ctx => HandleGSAssetIndex1(ctx);
        _input.Installation.GSAssetIndex2.started += ctx => HandleGSAssetIndex2(ctx);
        _input.Installation.SplatScale.started += ctx => HandleSplatScale(ctx);
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

    private void HandleGSAssetIndex1(InputAction.CallbackContext context){
        float input = context.ReadValue<float>();
        int value = (input > 0) ? 1 : -1;
        GSAssetIndex1Event?.Invoke(value);
    }

    private void HandleGSAssetIndex2(InputAction.CallbackContext context){
        float input = context.ReadValue<float>();
        int value = (input > 0) ? 1 : -1;
        GSAssetIndex2Event?.Invoke(value);
    }

    private void HandleSplatScale(InputAction.CallbackContext context){
        SplatScaleEvent?.Invoke(context.ReadValue<float>());
    }
}
