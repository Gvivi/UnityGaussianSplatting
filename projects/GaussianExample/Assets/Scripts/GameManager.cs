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

    public static event Action ModelChangeEvent;
    public GameObject ModelManagerObject { get; private set; } // object that holds all the models
    public GameObject CurrentModelObject { get; private set; }
    public GameObject PrevModelObject { get; private set; }
    // from input
    public int Timestamp { get; private set; }
    public int ZoomLevel { get; private set; }

    [SerializeField] private InputReader _inputReader;
    private int _numModels;
    private int _currentModelIndex = 0; 
    
    private void Awake() {
        _instance = this;

        ModelManagerObject = GameObject.Find("ModelManager");
        _numModels = ModelManagerObject.transform.childCount;
        _currentModelIndex = 0;

        GameObject firstModel = ModelManagerObject.transform.GetChild(_currentModelIndex).gameObject;
        firstModel.SetActive(true);
        firstModel.GetComponent<GaussianSplatRenderer>().m_OpacityScale = 1.0f;
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
        //_inputReader.TimeSliderEvent += HandleTimeSlider;
        _inputReader.DevShiftIndexEvent += HandleDevShiftIndex;
    }

    private void OnDisable() {
        //_inputReader.TimeSliderEvent -= HandleTimeSlider;
        _inputReader.DevShiftIndexEvent -= HandleDevShiftIndex;
    }

    // adjusts the CurrentModelObject by index for development purposes
    private void HandleDevShiftIndex(int indexShift){
        // check if the new index shift is valid
        if (_currentModelIndex + indexShift < 0 || _currentModelIndex + indexShift >= _numModels){
            Debug.Log("Invalid index shift");
            return;
        } else {
            _currentModelIndex += indexShift;
        } 

        PrevModelObject = CurrentModelObject;
        CurrentModelObject = ModelManagerObject.transform.GetChild(_currentModelIndex).gameObject;
        string modelName = CurrentModelObject.name;

        Debug.Log("new model index: " + _currentModelIndex);
        Debug.Log("new model name: " + modelName);
        
        ModelChangeEvent?.Invoke();
    }
}