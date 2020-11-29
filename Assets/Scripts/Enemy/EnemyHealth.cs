using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
public class EnemyHealth : MonoBehaviour
{
    [SerializeField]
    private int _Health = 1;
    [SerializeField]
    private int _collisionDamage = 1;

    private int _currentHealth;
    public int HitPoints { get { return _Health; } }
    public int CollisionDamage { get { return _collisionDamage; } }

    [SerializeField]
    private Room _roomAssigned;
    public Room RoomAssigned { get { return _roomAssigned; } set { _roomAssigned = value; } }

    private bool _isActive = true;
    public bool Active { get { return _isActive; } 
        set { 
            _isActive = value;
            _currentHealth = 1;
            if (_anim) { _anim.SetBool("Dead", false); }
            //OnActiveChange.Invoke(_isActive);
        } 
    }

    public delegate void ActivationDelegate(bool setActive);
    public ActivationDelegate OnActiveChange;

    private Animator _anim;
    private void Start()
    {
        _anim = GetComponent<Animator>();
        _currentHealth = _Health;
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
        Active = false;
        _anim.SetBool("Dead", true);
        AudioManager.Play("EnemyDie");
    }
}
