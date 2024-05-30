// Morphs Gaussian Splat Assets by lerping each splat to values of the target asset over time.
// Only for very high quality GS assets for now.
// Put this script on the GameObject containing the GaussianSplatRenderer component to start with.

using UnityEngine;
using GaussianSplatting.Runtime;
using UnityEditor;
using UnityEditor.EditorTools;
using GaussianSplatting.Editor;
using System.Linq;

public class GSMorphSplats : MonoBehaviour
{
    private GaussianSplatRenderer _GSRenderer; // GaussianSplatRenderer component on this object
    [Tooltip("GS assets to morph to in this order.")]
    [SerializeField] private GaussianSplatAsset[] _assetsToMorph;
    private int _numAssetsToMorph;
    private GaussianSplatAsset _targetAsset = null;
    private int CurrentTargetIndex {
        get { return GameManager.Instance.CurrentTargetIndex; }
        set { GameManager.Instance.CurrentTargetIndex = value; }
    }
    private float SplatScale {
        get { return GameManager.Instance.SplatScale; }
        set { GameManager.Instance.SplatScale = value; }
    }
    
    private void OnEnable() {
        InputManager.Instance.GSAssetChangeEvent += HandleGSAssetChange;
        InputManager.Instance.SplatScaleChangeEvent += HandleSplatScaleChange;
    }

    private void OnDisable() {
        InputManager.Instance.GSAssetChangeEvent -= HandleGSAssetChange;
        InputManager.Instance.SplatScaleChangeEvent -= HandleSplatScaleChange;
    }

    private void Start() {
        _GSRenderer = GetComponent<GaussianSplatRenderer>();
        // add initial asset to morph array
        GaussianSplatAsset[] initAsset = {_GSRenderer.m_Asset};
        _assetsToMorph = initAsset.Concat(_assetsToMorph).ToArray(); 

        CurrentTargetIndex = 0;
        _numAssetsToMorph = _assetsToMorph.Length;
    }

    private void FixedUpdate() {
        float morphSpeed = Time.fixedDeltaTime;
        if(CurrentTargetIndex >= 0){
            Morphing(morphSpeed);
        }
    }

    private void Morphing(float morphSpeed){
        _GSRenderer.MorphSplats(morphSpeed);
    }

    private void HandleGSAssetChange(int indexShift){
        // check if the new index shift is valid
        if (CurrentTargetIndex + indexShift < 0 || CurrentTargetIndex + indexShift >= _numAssetsToMorph){
            Debug.Log("Invalid index shift");
            return;
        } else {
            CurrentTargetIndex += indexShift;
        }

        _targetAsset = _assetsToMorph[CurrentTargetIndex];
        _GSRenderer.UpdateTargetAsset(_targetAsset);
    }

    private void HandleSplatScaleChange(float input){
        SplatScale = Mathf.Clamp(SplatScale + input*0.1f, 0.1f, 2.0f);
        _GSRenderer.m_SplatScale = SplatScale;
        Debug.Log("splat scale: " + SplatScale);
    }
}