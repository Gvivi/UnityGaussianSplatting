using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GaussianSplatting.Runtime;
using UnityEditor;

public class MorphModels : MonoBehaviour
{
    private float _timer = 0.0f;
    private float _transitionDuration = 10.0f;

    private void OnEnable() {
        GameManager.ModelChangeEvent += HandleModelChange;
    }

    private void OnDisable() {
        GameManager.ModelChangeEvent -= HandleModelChange;
    }
 
    private void Update() {
        _timer += Time.deltaTime;
    }

    public void MorphToModel()
    {
        // lerp the opacity of the _models
        StartCoroutine(LerpOpacity());
    }

    private IEnumerator LerpOpacity(){
        // Lerp the opacity of the _models
        _timer = 0.0f;
        GaussianSplatRenderer currentModel = GameManager.Instance.CurrentModelObject.GetComponent<GaussianSplatRenderer>();
        GaussianSplatRenderer prevModel = GameManager.Instance.PrevModelObject.GetComponent<GaussianSplatRenderer>();

        while (_timer <= _transitionDuration) {
            float interpol = _timer/_transitionDuration;

            currentModel.m_OpacityScale = Mathf.Lerp(0.05f, 1.0f, interpol);
            prevModel.m_OpacityScale = Mathf.Lerp(1.0f, 0.05f, interpol);
            yield return null;
        }
    }

    private void HandleModelChange(){
        // Morph to the new model
        GameManager.Instance.CurrentModelObject.GetComponent<GaussianSplatRenderer>().m_OpacityScale = 0.05f;
        GameManager.Instance.CurrentModelObject.SetActive(true);
        MorphToModel();
    }
}
