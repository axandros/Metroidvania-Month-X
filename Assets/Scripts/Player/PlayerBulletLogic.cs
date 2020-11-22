using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerBulletLogic : MonoBehaviour
{
    [SerializeField]
    public int damage = 1;

    Room _managerRoom;

    BoxCollider2D _bc;
    Rigidbody2D _rb;

    private void Start()
    {
        _bc = GetComponent<BoxCollider2D>();
        _rb = GetComponent<Rigidbody2D>();
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Bullet collided with: " + collision.gameObject.name);

        // Check for enemy
        EnemyHealth eh = collision.gameObject.GetComponent<EnemyHealth>();
        if (eh)
        {
            eh.DealDamage(damage);
            Destroy(this.gameObject);
        }
        else {

            // Check for room
            Room r = collision.GetComponent<Room>();
            if (r)
            {
                if(_managerRoom)
                {
                    if (r != _managerRoom)
                    {
                        Destroy(this.gameObject);
                    }
                }
                else { _managerRoom = r; }
            }
        }
    }
}
