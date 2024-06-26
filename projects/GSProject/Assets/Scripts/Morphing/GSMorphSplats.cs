// Morphs Gaussian Splat Assets by lerping each splat to values of the target asset over time.
// Only for very high quality GS assets for now.
// Put this script on the GameObject containing the GaussianSplatRenderer component to start with.

using UnityEngine;
using GaussianSplatting.Runtime;
using UnityEditor;
using UnityEditor.EditorTools;
using GaussianSplatting.Editor;
using System.Linq;
using System.Collections.Generic;

public class GSMorphSplats : MonoBehaviour
{
    private GaussianSplatRenderer _GSRenderer; // GaussianSplatRenderer component on this object
    [Tooltip("GS assets to morph to in this order.")]
    private int _numAssetsIndex1;
    private int _numAssetsIndex2;
    private GaussianSplatAsset _targetAsset = null;
    private int _index1 = 0;
    private int _index2 = 0;
    private float SplatScale {
        get { return GameManager.Instance.SplatScale; }
        set { GameManager.Instance.SplatScale = value; }
    }

    [Header("Gaussian Splat Morphing")]
    [Tooltip("GS assets to morph between.")]
    [SerializeField]
    private GSMatrixElement[] _assetsToMorph = new GSMatrixElement[0];

    public GSMatrixElement[] AssetsToMorph
    {
        get => _assetsToMorph;
        set => _assetsToMorph = value;
    }
    
    private void OnEnable() {
        InputManager.Instance.GSAssetIndex1Event += HandleGSAssetIndex1;
        InputManager.Instance.GSAssetIndex2Event += HandleGSAssetIndex2;
        InputManager.Instance.GSAssetIndex1KeyboardEvent += HandleGSAssetIndex1Keyboard;
        InputManager.Instance.GSAssetIndex2KeyboardEvent += HandleGSAssetIndex2Keyboard;
        InputManager.Instance.SplatScaleEvent += HandleSplatScaleChange;
    }

    private void OnDisable() {
        InputManager.Instance.GSAssetIndex1Event -= HandleGSAssetIndex1;
        InputManager.Instance.GSAssetIndex2Event -= HandleGSAssetIndex2;
        InputManager.Instance.GSAssetIndex1KeyboardEvent -= HandleGSAssetIndex1Keyboard;
        InputManager.Instance.GSAssetIndex2KeyboardEvent -= HandleGSAssetIndex2Keyboard;
        InputManager.Instance.SplatScaleEvent -= HandleSplatScaleChange;
    }

    private void Start() {
        _GSRenderer = GetComponent<GaussianSplatRenderer>();

        _index1 = 0;
        _numAssetsIndex1 = _assetsToMorph.Length;
        _numAssetsIndex2 = _assetsToMorph[0].SubElements.Length;
    }

    private void FixedUpdate() {
        float morphSpeed = Time.fixedDeltaTime;
        if(_index1 >= 0){
            Morphing(morphSpeed);
        }
    }

    private void Morphing(float morphSpeed){
        _GSRenderer.MorphSplats(morphSpeed);
    }

    private int Remap (float value, float from1, float to1, float from2, float to2) {
        float result = (value - from1) / (to1 - from1) * (to2 - from2) + from2 + 0.5f;
        return (int)result;
    }

    private void HandleGSAssetIndex1(int sliderValue){
        int newIndex = Remap((float)sliderValue, 0f, 1023f, 0f, (float)_numAssetsIndex1-1f);
        Debug.Log("Slider Value: " + sliderValue + " New Index: " + newIndex);

        _index1 = newIndex;

        _targetAsset = _assetsToMorph[_index1].SubElements[_index2];
        _GSRenderer.UpdateTargetAsset(_targetAsset);
    }

    private void HandleGSAssetIndex2(int newIndex){
        // check if the new index is valid
        if (newIndex < 0 || newIndex >= _numAssetsIndex2){
            Debug.Log("Invalid index2");
            return;
        }

        Debug.Log("Switch Value: " + newIndex);
        _index2 = newIndex;

        _targetAsset = _assetsToMorph[_index1].SubElements[_index2];
        _GSRenderer.UpdateTargetAsset(_targetAsset);
    }

    private void HandleGSAssetIndex1Keyboard(int indexShift){
        // check if the new index shift is valid
        if (_index1 + indexShift < 0 || _index1 + indexShift >= _numAssetsIndex1){
            Debug.Log("Invalid index1 shift");
            return;
        } else {
            _index1 += indexShift;
        }

        _targetAsset = _assetsToMorph[_index1].SubElements[_index2];
        _GSRenderer.UpdateTargetAsset(_targetAsset);
    }

    private void HandleGSAssetIndex2Keyboard(int indexShift){
        // check if the new index shift is valid
        if (_index2 + indexShift < 0 || _index2 + indexShift >= _numAssetsIndex2){
            Debug.Log("Invalid index2 shift");
            return;
        } else {
            _index2 += indexShift;
        }

        _targetAsset = _assetsToMorph[_index1].SubElements[_index2];
        _GSRenderer.UpdateTargetAsset(_targetAsset);
    }

    private void HandleSplatScaleChange(float input){
        SplatScale = Mathf.Clamp(SplatScale + input*0.1f, 0.1f, 2.0f);
        _GSRenderer.m_SplatScale = SplatScale;
        Debug.Log("splat scale: " + SplatScale);
    }
}