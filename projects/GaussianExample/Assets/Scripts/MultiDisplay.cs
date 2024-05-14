using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiDisplay : MonoBehaviour
{
    private enum DisplayMode { MultiDisplay, SplitScreen, Projection };
    private GameObject[] _cameras;
    private int _nCameras = 3;

    [SerializeField] private GameObject _lookAt;
    [SerializeField] private RenderTexture[] _renderTextures;
    [SerializeField] private float _cameraAngle = 40f;
    [SerializeField] private float _radius = 5.0f;
    [SerializeField] private float _speed = 0.2f;
    [SerializeField] private DisplayMode _displayMode = DisplayMode.SplitScreen;

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
                _lookAt.transform.position.x + _radius * Mathf.Cos(Time.time * _speed + i),
                _lookAt.transform.position.y,
                _lookAt.transform.position.z + _radius * Mathf.Sin(Time.time * _speed + i)
            );
            _cameras[i].transform.LookAt(_lookAt.transform);
        }
    }

    void CreateCameras(){
        _cameras = new GameObject[_nCameras];
        float angleInRadians = _cameraAngle * Mathf.Deg2Rad;

        for (int i = 0; i < _nCameras; i++)
        {
            _cameras[i] = new GameObject("Camera" + i);
            _cameras[i].AddComponent<Camera>();
            _cameras[i].transform.position = new Vector3(
                _lookAt.transform.position.x + _radius * Mathf.Cos(i * angleInRadians),
                _lookAt.transform.position.y,
                _lookAt.transform.position.z + _radius * Mathf.Sin(i * angleInRadians)
            );
            _cameras[i].transform.LookAt(_lookAt.transform);
            _cameras[i].GetComponent<Camera>().clearFlags = CameraClearFlags.SolidColor;
            _cameras[i].GetComponent<Camera>().backgroundColor = new Color(0, 0, 0, 0);

            // switch projection mode
            switch (_displayMode)
            {
                case DisplayMode.MultiDisplay:
                    SetMultiDisplay(i);
                    break;
                case DisplayMode.SplitScreen:
                    SetSplitScreen(i);
                    break;
                case DisplayMode.Projection:
                    SetProjection(i);
                    break;
            }
        }
    }

    void SetMultiDisplay(int index){
        _cameras[index].GetComponent<Camera>().targetDisplay = index;
    }

    void SetSplitScreen(int index){
        _cameras[index].GetComponent<Camera>().rect = new Rect(index * 1.0f / _nCameras, 0, 1.0f / _nCameras, 1);
    }

    void SetProjection(int index){
        _cameras[index].GetComponent<Camera>().targetTexture = _renderTextures[index];
        _cameras[index].GetComponent<Camera>().rect = new Rect(0, 0, 1, 1);
    }

    // editor preview wireframe
    void OnDrawGizmos()
    {
        float angleInRadians = _cameraAngle * Mathf.Deg2Rad;

        // array of 3 colores
        Color[] colors = { Color.red, Color.green, Color.blue };

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_lookAt.transform.position, 0.5f);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(_lookAt.transform.position, _radius);
        // this part only when the game is running
        if (Application.isPlaying) {
            for (int i = 0; i < _nCameras; i++)
            {
                Gizmos.color = colors[i];
                Gizmos.DrawLine(_lookAt.transform.position, _cameras[i].transform.position); 
            }
        }
    }
}
