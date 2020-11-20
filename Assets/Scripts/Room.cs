using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Room : MonoBehaviour
{
    [SerializeField]
    Vector2 _Extents = new Vector2(10.0f,7.0f);

    CameraFollow _camFollow;

    List<EnemyHealth> _enemiesInRoom;

    public Vector2 Extents { get { return _Extents; } }

    private BoxCollider2D _roomCollider;

    // Start is called before the first frame update
    void Awake()
    {
        _roomCollider = GetComponent<BoxCollider2D>();
        _roomCollider.size = _Extents;
        _camFollow = Camera.main.GetComponent<CameraFollow>();
        _enemiesInRoom = new List<EnemyHealth>();

        List<Collider2D> allObjectInRoom = new List<Collider2D>();
        ContactFilter2D contact = new ContactFilter2D();
        contact.NoFilter();
        _roomCollider.OverlapCollider( contact,allObjectInRoom);
        foreach(Collider2D c in allObjectInRoom)
        {
            EnemyHealth eh = c.gameObject.GetComponent<EnemyHealth>();
            if (eh)
            {

            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Trigger Entered");
        PlatformerCharacter pc = collision.gameObject.GetComponent<PlatformerCharacter>();
        if (pc)
        {
            Debug.Log("Setting new room for camera.");
            _camFollow.SetRoom(this);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(this.transform.position, _Extents);
    }
}
