using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GaussianSplatting.Runtime;
using UnityEditor;

public class MorphModels : MonoBehaviour
{
    private float _timer = 0.0f;
    private float _transitionScaler = 0.2f;
    private bool _isMorphing = false;
    private GaussianSplatRenderer _currentModel;
    private GaussianSplatRenderer _prevModel;

    private void OnEnable() {
        GameManager.MorphStartEvent += HandleMorphStart;
        GameManager.NewSplatSizeEvent += HandleNewSplatSize;
    }

    private void OnDisable() {
        GameManager.MorphStartEvent -= HandleMorphStart;
        GameManager.NewSplatSizeEvent -= HandleNewSplatSize;
    }

    private void Start() {
        _currentModel = GameManager.Instance.CurrentModelObject.GetComponent<GaussianSplatRenderer>();
    }

    private void FixedUpdate() {
        if(_isMorphing){
            _timer += Time.fixedDeltaTime;
            LerpOpacity();
        }
    }

    private void LerpOpacity(){
        float interpol = _timer*_transitionScaler;

        _currentModel.m_OpacityScale = Mathf.Min(1f, _timer*_transitionScaler + 0.05f);
        _prevModel.m_OpacityScale = Mathf.Max(0.05f, 1f - (_timer*_transitionScaler + 0.05f));
        
        if(_timer*_transitionScaler >= 1.0f){
            Debug.Log("Morphing done.");
            _prevModel.gameObject.SetActive(false);
            _isMorphing = false;
            GameManager.Instance.IsMorphing = false;
        }
    }

    private void HandleMorphStart(){
        Debug.Log("Morphing start.");
        GameManager.Instance.IsMorphing = true;
        _isMorphing = true;
        
        GameManager.Instance.CurrentModelObject.SetActive(true);
        _currentModel = GameManager.Instance.CurrentModelObject.GetComponent<GaussianSplatRenderer>();
        _prevModel = GameManager.Instance.PrevModelObject.GetComponent<GaussianSplatRenderer>();
        _currentModel.m_OpacityScale = 0.05f;
        _timer = 0.0f;
    }

    private void HandleNewSplatSize(float newSize){
        _currentModel.m_SplatScale = newSize;
        if(_prevModel) _prevModel.m_SplatScale = newSize;
    }
}
