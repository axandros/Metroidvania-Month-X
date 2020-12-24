using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

namespace AxTools.retro{
    [RequireComponent(typeof(Camera))]
    [RequireComponent(typeof(PixelPerfectCamera))]
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField, Tooltip("The object to follow")]
        private GameObject _Target;

        PixelPerfectCamera _ppc;

        private Camera _cam;
        Room _activeRoom;
        float _cameraHalfHeight;
        float _cameraHalfWidth;

        private Vector2 _roomMax; // Top Right corner
        private Vector2 _roomMin; // Bottom Left Corner


        void Start()
        {
            // Cache components.
            _ppc = GetComponent<PixelPerfectCamera>();
            _cam = GetComponent<Camera>();
            
            // Cache the height and width of the camera.
            _cameraHalfHeight = _cam.orthographicSize;
            _cameraHalfWidth = _cameraHalfHeight * _ppc.refResolutionX / _ppc.refResolutionY;
        }

        void Update()
        {
            // Set the position of the camera.
            float x = _Target.transform.position.x;
            float y = _Target.transform.position.y;
            Vector2 newPos = Clamp(new Vector2(x, y), _roomMin, _roomMax); // Keep the camera in the bounds of the current room.
            this.transform.position = new Vector3(newPos.x, newPos.y, this.transform.position.z);
        }

        /// <summary>
        /// Change the currently focused room.
        /// </summary>
        /// <param name="r">The room to focus on.</param>
        public void SetRoom(Room r)
        {
            // Set the new active room.
            _activeRoom = r;

            // Cache the boundaries of the new room.
            Vector2 RoomPos = _activeRoom.transform.position;
            _roomMax = new Vector2(_activeRoom.Extents.x / 2 - _cameraHalfWidth, _activeRoom.Extents.y / 2 - _cameraHalfHeight) + RoomPos;
            _roomMin = new Vector2(_cameraHalfWidth - _activeRoom.Extents.x / 2, _cameraHalfHeight - _activeRoom.Extents.y / 2) + RoomPos;
        }

        /// <summary>
        /// Clamp the values of a vectors to between two other given vectors.
        /// </summary>
        /// <param name="vectorToClamp">The vector to clamp.</param>
        /// <param name="MinValues">The minimum values.</param>
        /// <param name="MaxValues">The maximum values.</param>
        /// <returns>Returns a clamped version of the vectorToClamp.</returns>
        Vector2 Clamp(Vector2 vectorToClamp, Vector2 MinValues, Vector2 MaxValues)
        {
            float x = Mathf.Clamp(vectorToClamp.x, MinValues.x, MaxValues.x);
            float y = Mathf.Clamp(vectorToClamp.y, MinValues.y, MaxValues.y);
            return new Vector2(x, y);
        }
    }
}