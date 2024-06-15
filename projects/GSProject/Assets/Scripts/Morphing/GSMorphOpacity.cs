// Morphs two Gaussian Splat Assets by changing their opacity values. 
// Can only morph to a new asset if the current morphing process is finished. To avoid this restriction, use the GSMorphSplats.cs script instead.
// put this script on the parent object of the GameObjects containing GaussianSplatRenderers.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GaussianSplatting.Runtime;
using UnityEditor;

public class GSMorphOpacity : MonoBehaviour
{
    private bool _isMorphing = false;
    private GaussianSplatRenderer _currentGSRenderer;
    private GaussianSplatRenderer _prevGSRenderer;
    private int _numGSAssets;
    private int _index1 = 0;
    private float SplatScale {
        get { return GameManager.Instance.SplatScale; }
        set { GameManager.Instance.SplatScale = value; }
    }

    private void OnEnable() {
        InputManager.Instance.GSAssetIndex1Event += HandleGSAssetIndex1Keyboard;
        InputManager.Instance.SplatScaleEvent += HandleSplatScale;
    }

    private void OnDisable() {
        InputManager.Instance.GSAssetIndex1Event -= HandleGSAssetIndex1Keyboard;
        InputManager.Instance.SplatScaleEvent -= HandleSplatScale;
    }

    private void Start() {
        _numGSAssets = transform.childCount;

        GameObject currentGSObject = transform.GetChild(_index1).gameObject;
        currentGSObject.SetActive(true);
        _currentGSRenderer = currentGSObject.GetComponent<GaussianSplatRenderer>();
        _currentGSRenderer.m_OpacityScale = 1.0f;
        _currentGSRenderer.m_SplatScale = SplatScale;
        _prevGSRenderer = null;

        // deaktivate all models except the first one 
        for (int i = 1; i < _numGSAssets; i++){
            GameObject child = transform.GetChild(i).gameObject;
            child.SetActive(false);
        }
    }

    private void FixedUpdate() {
        float morphSpeed = Time.fixedDeltaTime;
        if(_isMorphing){
            LerpOpacity(morphSpeed);
        }
    }

    private void LerpOpacity(float interpolation){
        _currentGSRenderer.m_OpacityScale = Mathf.Lerp(_currentGSRenderer.m_OpacityScale, 1.0f, interpolation);
        _prevGSRenderer.m_OpacityScale = Mathf.Lerp(_prevGSRenderer.m_OpacityScale, 0.0f, interpolation);
        
        if(_currentGSRenderer.m_OpacityScale >= 0.999f){
            Debug.Log("Morphing done.");
            _prevGSRenderer.gameObject.SetActive(false);
            _isMorphing = false;
        }
    }

    private void HandleGSAssetIndex1Keyboard(int indexShift){
        // check if the new index shift is valid
        if (_index1 + indexShift < 0 || _index1 + indexShift >= _numGSAssets || _isMorphing){
            Debug.Log("Invalid index shift");
            return;
        } else {
            _index1 += indexShift;
        }

        _prevGSRenderer?.gameObject.SetActive(false);
        _prevGSRenderer = _currentGSRenderer;
        _currentGSRenderer = transform.GetChild(_index1).gameObject.GetComponent<GaussianSplatRenderer>();
        _currentGSRenderer.m_OpacityScale = 0.0f;
        _currentGSRenderer.m_SplatScale = SplatScale;
        _currentGSRenderer.gameObject.SetActive(true);

        string assetName = _currentGSRenderer.name;
        Debug.Log("new GS asset index: " + _index1);
        Debug.Log("new GS asset name: " + assetName);
        
        _isMorphing = true;
    }

    private void HandleSplatScale(float input){
        SplatScale = Mathf.Clamp(SplatScale + input*0.1f, 0.1f, 2.0f);
        _currentGSRenderer.m_SplatScale = SplatScale;
        Debug.Log("splat scale: " + SplatScale);
        if(_prevGSRenderer) _prevGSRenderer.m_SplatScale = SplatScale;
    }
}
