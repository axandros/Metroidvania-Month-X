using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Pickup_PDW : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerShooting ps = collision.GetComponent<PlayerShooting>();
        if (ps && !ps.isActiveAndEnabled)
        {
            ps.enabled = true;
            Destroy(this.gameObject);
        }
    }
}
