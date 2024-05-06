using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiDisplay : MonoBehaviour
{
    [SerializeField] private int _nCameras = 3;
    [SerializeField] private GameObject _lookAt;
    [SerializeField] private float _radius = 5.0f;
    [SerializeField] private float _speed = 0.2f;
    [SerializeField] private bool _multiDisplay = false;

    private GameObject[] _cameras;

    void Start()
    {
        CreateCameras(); 
    }

    void Update()
    {
        // move cameras around the _lookAt object
        for (int i = 0; i < _cameras.Length; i++)
        {
            _cameras[i].transform.position = new Vector3(
                _lookAt.transform.position.x + _radius * Mathf.Cos(Time.time * _speed + i * 2 * Mathf.PI / _cameras.Length),
                _lookAt.transform.position.y,
                _lookAt.transform.position.z + _radius * Mathf.Sin(Time.time * _speed + i * 2 * Mathf.PI / _cameras.Length)
            );
            _cameras[i].transform.LookAt(_lookAt.transform);
        }
    }

    void CreateCameras(){
        _cameras = new GameObject[_nCameras];

        for (int i = 0; i < _nCameras; i++)
        {
            _cameras[i] = new GameObject("Camera" + i);
            _cameras[i].AddComponent<Camera>();
            _cameras[i].transform.position = new Vector3(
                _lookAt.transform.position.x + _radius * Mathf.Cos(i * 2 * Mathf.PI / _nCameras),
                _lookAt.transform.position.y,
                _lookAt.transform.position.z + _radius * Mathf.Sin(i * 2 * Mathf.PI / _nCameras)
            );
            _cameras[i].transform.LookAt(_lookAt.transform);

            if (_multiDisplay)
            {
                SetMultiDisplay(i);
            } else {
                SetSplitScreen(i);
            }
        }
    }

    void SetMultiDisplay(int index){
        _cameras[index].GetComponent<Camera>().targetDisplay = index;
    }

    void SetSplitScreen(int index){
        _cameras[index].GetComponent<Camera>().rect = new Rect(index * 1.0f / _nCameras, 0, 1.0f / _nCameras, 1);
    }

    // editor preview wireframe
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_lookAt.transform.position, 0.5f);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(_lookAt.transform.position, _radius);
        for (int i = 0; i < _nCameras; i++)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(_lookAt.transform.position, new Vector3(
                _lookAt.transform.position.x + _radius * Mathf.Cos(i * 2 * Mathf.PI / _nCameras),
                _lookAt.transform.position.y,
                _lookAt.transform.position.z + _radius * Mathf.Sin(i * 2 * Mathf.PI / _nCameras)
            )); 
        }
    }
}
