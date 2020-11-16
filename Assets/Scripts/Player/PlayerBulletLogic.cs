using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class PlayerBulletLogic : MonoBehaviour
{
    [SerializeField]
    public int damage = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        EnemyHealth eh = collision.gameObject.GetComponent<EnemyHealth>();
        Debug.Log("Bullet collided with: " + collision.gameObject.name);
        if (eh)
        {
            eh.DealDamage(damage);
            Destroy(this.gameObject);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
    }
}
