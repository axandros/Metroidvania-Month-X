using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Room : MonoBehaviour
{
    [SerializeField]
    Vector2 _Extents = new Vector2(10.0f,7.0f);

    CameraFollow _camFollow;

    public Vector2 Extents { get { return _Extents; } }

    private BoxCollider2D _roomCollider;

    // Start is called before the first frame update
    void Awake()
    {
        _roomCollider = GetComponent<BoxCollider2D>();
        _roomCollider.size = _Extents;
        _camFollow = Camera.main.GetComponent<CameraFollow>();
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
