using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using GaussianSplatting.Runtime;
using UnityEditor;
using System;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance {
        get {
            if (_instance == null) {
                Debug.LogError("GameManager is NULL."); 
            }
            return _instance;
        }
    }

    public float SplatScale { get; set; } = 1.0f;
    private void Awake() {
        _instance = this;
    }
}