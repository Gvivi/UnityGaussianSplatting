using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
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
    SerialPort data_stream = new SerialPort("COM3", 9600);
    private string received_data;
    private string[] data_array;
    private bool portIsOpen = false;
    private int prevSliderValue = 0;
    private int prevSwitchValue = 0;

    public event Action<int> GSAssetIndex1Event;
    public event Action<int> GSAssetIndex1KeyboardEvent;
    public event Action<int> GSAssetIndex2Event;
    public event Action<int> GSAssetIndex2KeyboardEvent;
    public event Action<float> SplatScaleEvent;
    public event Action<Vector2> MoveEvent;
    public event Action<Vector2> LookEvent;

    private void OnEnable() {
        if(isSimulation){
            _input.Simulation.Enable();
            _input.Installation.Disable();
        } else {
            _input.Installation.Enable();
            _input.Simulation.Disable();
        }

        try {
            data_stream.Open();
        } catch (Exception e) {
            Debug.LogWarning("Error: " + e.Message);
        }
    }

    private void OnDisable() {
        _input.Disable();
        if(data_stream.IsOpen){
            data_stream.Close();
        }
    }

    private void Awake() {
        _instance = this;

        _input = new PlayerInput();
        _input.Simulation.Look.performed += ctx => HandleLook(ctx);
        _input.Simulation.GSAssetIndex1.started += ctx => HandleGSAssetIndex1Keyboard(ctx);
        _input.Simulation.GSAssetIndex2.started += ctx => HandleGSAssetIndex2Keyboard(ctx);
        _input.Simulation.SplatScale.started += ctx => HandleSplatScale(ctx);
        _input.Installation.GSAssetIndex1.started += ctx => HandleGSAssetIndex1Keyboard(ctx);
        _input.Installation.GSAssetIndex2.started += ctx => HandleGSAssetIndex2Keyboard(ctx);
        _input.Installation.SplatScale.started += ctx => HandleSplatScale(ctx);
    }

    private void Start() {
        if(data_stream.IsOpen){
            portIsOpen = true;
        }
    }

    private void Update() {
        if (portIsOpen) {
            received_data = data_stream.ReadLine();
            data_array = received_data.Split(',');
            
            HandleGSAssetIndex1(int.Parse(data_array[0]));
            
            HandleGSAssetIndex2(int.Parse(data_array[1]));
        }
    }

    private void FixedUpdate() {
        MoveEvent?.Invoke(_input.Simulation.Move.ReadValue<Vector2>());
    }

    public void HandleLook(InputAction.CallbackContext context)
    {
        LookEvent?.Invoke(context.ReadValue<Vector2>());
    }

    private void HandleGSAssetIndex1(int sliderValue){
        if(sliderValue != prevSliderValue){
            prevSliderValue = sliderValue;
            GSAssetIndex1Event?.Invoke(sliderValue);
        }
    }

    private void HandleGSAssetIndex2(int switchValue){
        if(switchValue != prevSwitchValue){
            prevSwitchValue = switchValue;
            GSAssetIndex2Event?.Invoke(switchValue);
        }
    }

    private void HandleGSAssetIndex1Keyboard(InputAction.CallbackContext context){
        float input = context.ReadValue<float>();
        int value = (input > 0) ? 1 : -1;
        GSAssetIndex1KeyboardEvent?.Invoke(value);
    }

    private void HandleGSAssetIndex2Keyboard(InputAction.CallbackContext context){
        float input = context.ReadValue<float>();
        int value = (input > 0) ? 1 : -1;
        GSAssetIndex2KeyboardEvent?.Invoke(value);
    }

    private void HandleSplatScale(InputAction.CallbackContext context){
        SplatScaleEvent?.Invoke(context.ReadValue<float>());
    }
}
