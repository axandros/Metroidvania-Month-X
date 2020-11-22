﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    [SerializeField]
    Vector2 _Extents = new Vector2(10.0f,7.0f);

    CameraFollow _camFollow;

    List<Enemy> _enemiesInRoom;

    public Vector2 Extents { get { return _Extents; } }

    private BoxCollider2D _roomCollider;

    // Start is called before the first frame update
    void Awake()
    {
        _roomCollider = GetComponent<BoxCollider2D>();
        _roomCollider.size = _Extents;
        _camFollow = Camera.main.GetComponent<CameraFollow>();
        _enemiesInRoom = new List<Enemy>();

        List<Collider2D> allObjectInRoom = new List<Collider2D>();
        ContactFilter2D contact = new ContactFilter2D();
        contact.NoFilter();
        _roomCollider.OverlapCollider( contact,allObjectInRoom);
        foreach(Collider2D c in allObjectInRoom)
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
            _camFollow.SetRoom(this);
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

    private void SetRoomStatus(bool isActive)
    {
        foreach(Enemy e in _enemiesInRoom)
        {
            e.enemyHealth.Active = isActive;
            if (isActive) { e.enemyHealth.gameObject.transform.position = e.startPosition; }
        }
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(this.transform.position, _Extents);
    }
}
