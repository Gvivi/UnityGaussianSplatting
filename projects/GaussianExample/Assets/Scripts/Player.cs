using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Vector3 _moveDirection = Vector3.zero;

    private void OnEnable() {
        InputManager.Instance.MoveEvent += Move;
        InputManager.Instance.LookEvent += Look;
    }

    private void OnDisable() {
        InputManager.Instance.MoveEvent -= Move;
        InputManager.Instance.LookEvent -= Look;
    }

    private void Start() {
        Cursor.visible = false;
    }

    private void FixedUpdate() {
        transform.Translate(_moveDirection * Time.deltaTime);
    }

    private void Move(Vector2 direction){
        _moveDirection = new Vector3(direction.x, 0, direction.y);
    }

    // look left, right, up, down 
    private void Look(Vector2 direction)
    {
        transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles + new Vector3(0, direction.x/10, 0));
        transform.GetChild(0).localRotation = Quaternion.Euler(transform.GetChild(0).localRotation.eulerAngles + new Vector3(-direction.y/10, 0, 0));
    }
}
