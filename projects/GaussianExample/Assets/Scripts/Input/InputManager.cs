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

    public event Action<int> DevShiftIndexEvent;
    public event Action<Vector2> MoveEvent;
    public event Action<Vector2> LookEvent;
    
    private void Awake() {
        _instance = this;

        _input = new PlayerInput();
        _input.Simulation.DevShiftIndex.started += ctx => HandleDevShiftIndex(ctx);
        _input.Simulation.Look.performed += ctx => HandleLook(ctx);
        _input.Installation.DevShiftIndex.started += ctx => HandleDevShiftIndex(ctx);
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

    private void HandleDevShiftIndex(InputAction.CallbackContext context){
        if(context.phase == InputActionPhase.Started){
            // pass on int value to GameManager
            float input = context.ReadValue<float>();
            int value = (input > 0) ? 1 : -1;
            DevShiftIndexEvent?.Invoke(value);
        }
    }

    public void HandleLook(InputAction.CallbackContext context)
    {
        LookEvent?.Invoke(context.ReadValue<Vector2>());
    }
}
