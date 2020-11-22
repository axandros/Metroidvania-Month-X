using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

[RequireComponent(typeof(Camera))]
[RequireComponent(typeof(PixelPerfectCamera))]
public class CameraFollow : MonoBehaviour
{
    [SerializeField]
    private GameObject _Target;

    PixelPerfectCamera _ppc;

    private Camera _cam;
    Room _activeRoom;
    float _cameraHalfHeight;
    float _cameraHalfWidth;

    private Vector2 _roomMax; // Top Right corner
    private Vector2 _roomMin; // Bottom Left Corner


    // Start is called before the first frame update
    void Start()
    {
        _ppc = GetComponent<PixelPerfectCamera>();
        _cam = GetComponent<Camera>();
        _cameraHalfHeight = _cam.orthographicSize;
            
        //Debug.Log("Camera Orthographic: " + _cam.orthographicSize);
    }

    // Update is called once per frame
    void Update()
    {
        float x = _Target.transform.position.x;
        float y = _Target.transform.position.y;
        Vector2 newPos = Clamp(new Vector2(x, y), _roomMin, _roomMax);
        
        //cam.orthographicSize;

        this.transform.position = new Vector3(newPos.x, newPos.y, this.transform.position.z);
    }

    public void SetRoom(Room r)
    {
        _activeRoom = r;

        Vector2 RoomPos = _activeRoom.transform.position;

        _cameraHalfHeight = _cam.orthographicSize;
        _cameraHalfWidth = _cameraHalfHeight * _ppc.refResolutionX / _ppc.refResolutionY;
        //_roomMax = new Vector2(_activeRoom.Extents.x/2 - _cameraHalfWidth, _activeRoom.Extents.y/2 - _cameraHalfHeight) + RoomPos;
        //_roomMin = new Vector2(_cameraHalfWidth - _activeRoom.Extents.x/2, _cameraHalfHeight - _activeRoom.Extents.y/2) + RoomPos;
        _roomMax = new Vector2(_activeRoom.Extents.x /2  - _cameraHalfWidth, _activeRoom.Extents.y / 2 - _cameraHalfHeight) + RoomPos;
        _roomMin = new Vector2(_cameraHalfWidth - _activeRoom.Extents.x / 2, _cameraHalfHeight - _activeRoom.Extents.y / 2) + RoomPos;
    }

    Vector2 Clamp(Vector2 vectorToClamp, Vector2 MinValues, Vector2 MaxValues)
    {
        //DebugText.Log("Clamping: " + vectorToClamp+ " between " +MinValues+ " and " +MaxValues );
        float x = Mathf.Clamp(vectorToClamp.x, MinValues.x, MaxValues.x);
        float y = Mathf.Clamp(vectorToClamp.y, MinValues.y, MaxValues.y);
        return new Vector2(x, y);
    }
}
