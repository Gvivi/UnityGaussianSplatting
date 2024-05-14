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

    public event Action<int> ModelChangeEvent;
    public event Action<float> SplatSizeChangeEvent;
    public event Action<Vector2> MoveEvent;
    public event Action<Vector2> LookEvent;
    
    private void Awake() {
        _instance = this;

        _input = new PlayerInput();
        _input.Simulation.Look.performed += ctx => HandleLook(ctx);
        _input.Simulation.ChangeModel.started += ctx => HandleChangeModel(ctx);
        _input.Simulation.ChangeSplatSize.started += ctx => HandleChangeSplatSize(ctx);
        _input.Installation.ChangeModel.started += ctx => HandleChangeModel(ctx);
        _input.Installation.ChangeSplatSize.started += ctx => HandleChangeSplatSize(ctx);
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

    private void HandleChangeModel(InputAction.CallbackContext context){
        // pass on int value to GameManager
        float input = context.ReadValue<float>();
        int value = (input > 0) ? 1 : -1;
        ModelChangeEvent?.Invoke(value);
    }

    private void HandleChangeSplatSize(InputAction.CallbackContext context){
        // pass on float value to GameManager
        SplatSizeChangeEvent?.Invoke(context.ReadValue<float>());
    }
}
