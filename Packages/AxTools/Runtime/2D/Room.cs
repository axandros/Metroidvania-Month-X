using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AxTools.retro{
    [RequireComponent(typeof(BoxCollider2D))]
    public class Room : MonoBehaviour
    {
        private struct Enemy
        {
            public Enemy(EnemyHealth eh)
            {
                enemyHealth = eh;
                startPosition = eh.transform.position;
            }
            public EnemyHealth enemyHealth;
            public Vector2 startPosition;
        }

        [SerializeField, Tooltip("The width in map tiles for this room."), Range(1, 9)]
        int _Width = 1;
        
        [SerializeField, Tooltip("The height in map tiles for this room."), Range(1, 9)]
        int _Height = 1;

        // TODO: Make these editable gloabally through an editor window.
        static float _widthMultiplier = 10;
        float _heightMultiplier = 9;

        /// <summary>
        /// The Camea Follow Script moves the character to follow a specified camera.  It will obey the bounds of a given room.
        /// </summary>
        CameraFollow _cameraFollow;

        List<Enemy> _enemiesInRoom;

        /// <summary>
        /// Provides the extents of the room, which will match the extents of the box collider at runtime.
        /// </summary>
        public Vector2 Extents { get { return new Vector2(_Width * _widthMultiplier, _Height * _heightMultiplier); ; } }

        private BoxCollider2D _roomCollider;
        
        void Awake()
        {
            if (_cameraFollow == null) { _cameraFollow = Camera.main.GetComponent<CameraFollow>(); }
            _roomCollider = GetComponent<BoxCollider2D>();
            _roomCollider.size = Extents;
            _enemiesInRoom = new List<Enemy>();

            List<Collider2D> allObjectInRoom = new List<Collider2D>();
            ContactFilter2D contact = new ContactFilter2D();
            contact.NoFilter();
            _roomCollider.OverlapCollider(contact, allObjectInRoom);
            foreach (Collider2D c in allObjectInRoom)
            {
                EnemyHealth eh = c.gameObject.GetComponent<EnemyHealth>();
                if (eh)
                {
                    Enemy e = new Enemy(eh);
                    _enemiesInRoom.Add(e);
                    eh.RoomAssigned = this;
                    eh.Active = false;
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            //Debug.Log("Trigger Entered");
            PlatformerCharacter pc = collision.gameObject.GetComponent<PlatformerCharacter>();
            if (pc)
            {
                //Debug.Log("Setting new room for camera.");
                _cameraFollow.SetRoom(this);
                SetRoomStatus(true);
            }
        }
        private void OnTriggerExit2D(Collider2D collision)
        {
            PlatformerCharacter pc = collision.gameObject.GetComponent<PlatformerCharacter>();
            if (pc)
            {
                SetRoomStatus(false);
            }
        }

        /// <summary>
        /// Handles the operations of the player entering and exiting the room.
        /// </summary>
        /// <param name="isActive">Has the player entered (true) or left (false) the room?</param>
        private void SetRoomStatus(bool isActive)
        {
            foreach (Enemy e in _enemiesInRoom)
            {
                e.enemyHealth.Active = isActive;
                if (isActive) { e.enemyHealth.gameObject.transform.position = e.startPosition; }
            }

        }

        private void OnDrawGizmos()
        {
            // The the extents of the room.
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(this.transform.position, Extents);
        }
    } 
}