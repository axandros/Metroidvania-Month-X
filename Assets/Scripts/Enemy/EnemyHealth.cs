using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField]
    private int _Health = 1;
    [SerializeField]
    private int _collisionDamage = 1;
    public int HitPoints { get { return _Health; } }
    public int CollisionDamage { get { return _collisionDamage; } }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int DealDamage(int damage, DamageType type = DamageType.NONE)
    {
        _Health -= damage;
        if(_Health <= 0)
        {
            Death();
        }
        return damage;
    }

    void Death()
    {
        Destroy(this.gameObject);
    }
}
