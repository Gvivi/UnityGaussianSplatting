using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


[CreateAssetMenu(menuName = "InputReader")]

public class InputReader : ScriptableObject, PlayerInput.IInstallationActions
{
    public event Action<int> DevShiftIndexEvent;

    private PlayerInput _playerInput;

    private void OnEnable() {
        if(_playerInput == null){
            _playerInput = new PlayerInput();

            _playerInput.Installation.SetCallbacks(this);
            _playerInput.Installation.Enable();
        }
    }

    public void OnDevShiftIndex(InputAction.CallbackContext context){
        if(context.phase == InputActionPhase.Started){
            // pass on int value to GameManager
            Vector2 input = context.ReadValue<Vector2>();
            int value = (input.x > 0) ? 1 : -1;
            DevShiftIndexEvent?.Invoke(value);
        }
    }
}