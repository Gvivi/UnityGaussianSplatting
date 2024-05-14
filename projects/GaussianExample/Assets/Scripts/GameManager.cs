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

    public static event Action MorphStartEvent;
    public static event Action<float> NewSplatSizeEvent;
    public GameObject ModelManagerObject { get; private set; } // object that holds all the models
    public GameObject CurrentModelObject { get; private set; }
    public GameObject PrevModelObject { get; private set; }
    // from input
    public int Timestamp { get; private set; }
    public int ZoomLevel { get; private set; }
    public float SplatSize { get; private set; }

    private int _numModels;
    private int _currentModelIndex = 0;

    public bool IsMorphing { get; set; }
    
    private void Awake() {
        _instance = this;
        IsMorphing = false;
        SplatSize = 1f;

        ModelManagerObject = GameObject.Find("ModelManager");
        _numModels = ModelManagerObject.transform.childCount;
        _currentModelIndex = 0;

        GameObject firstModel = ModelManagerObject.transform.GetChild(_currentModelIndex).gameObject;
        firstModel.SetActive(true);
        firstModel.GetComponent<GaussianSplatRenderer>().m_OpacityScale = 1.0f;
        firstModel.GetComponent<GaussianSplatRenderer>().m_SplatScale = SplatSize;
        CurrentModelObject = firstModel;
        PrevModelObject = null;

        // deaktivate all models except the first one 
        for (int i = 1; i < _numModels; i++){
            GameObject child = ModelManagerObject.transform.GetChild(i).gameObject;
            child.SetActive(false);
            child.GetComponent<GaussianSplatRenderer>().m_OpacityScale = 0.05f;
        }
    }

    private void OnEnable() {
        InputManager.Instance.ModelChangeEvent += HandleModelChange;
        InputManager.Instance.SplatSizeChangeEvent += HandleSplatSizeChange;
    }

    private void OnDisable() {
        InputManager.Instance.ModelChangeEvent -= HandleModelChange;
        InputManager.Instance.SplatSizeChangeEvent -= HandleSplatSizeChange;
    }

    // adjusts the CurrentModelObject by index for development purposes
    private void HandleModelChange(int indexShift){
        // check if the new index shift is valid
        if (_currentModelIndex + indexShift < 0 || _currentModelIndex + indexShift >= _numModels || IsMorphing){
            Debug.Log("Invalid index shift");
            return;
        } else {
            _currentModelIndex += indexShift;
        } 

        PrevModelObject = CurrentModelObject;
        CurrentModelObject = ModelManagerObject.transform.GetChild(_currentModelIndex).gameObject;
        CurrentModelObject.GetComponent<GaussianSplatRenderer>().m_OpacityScale = 0.05f;
        CurrentModelObject.GetComponent<GaussianSplatRenderer>().m_SplatScale = SplatSize;
        string modelName = CurrentModelObject.name;

        Debug.Log("new model index: " + _currentModelIndex);
        Debug.Log("new model name: " + modelName);
        
        MorphStartEvent?.Invoke();
    }
    
    private void HandleSplatSizeChange(float input){
        Debug.Log("SplatSize input: " + input);
        SplatSize = Mathf.Clamp(SplatSize + input*0.1f, 0.1f, 2.0f);
        // pass on float value to CurrentModelObject
        NewSplatSizeEvent?.Invoke(SplatSize);
    }
}