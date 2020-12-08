using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
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
            if (value)
            {
                _currentHealth = 1;
                if (_cc) { _cc.enabled = true; }
                if (_anim) { _anim.SetBool("Dead", false); }
                if (_sr) { _sr.enabled = true; }
            }
            else {
                if (_cc) { _cc.enabled = false; }
                if (_sr) { _sr.enabled = false; }
            }
            if (OnActiveChange != null) { OnActiveChange.Invoke(_isActive); }
        } 
    }

    public delegate void ActivationDelegate(bool setActive);
    public ActivationDelegate OnActiveChange;

    private Animator _anim;
    private CapsuleCollider2D _cc;
    private Rigidbody2D _rb;
    private SpriteRenderer _sr;


    private void Start()
    {
        _anim = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody2D>();
        _cc = GetComponent<CapsuleCollider2D>();
        _sr = GetComponent<SpriteRenderer>();
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
        _cc.enabled = false;
        
        AudioManager.Play("EnemyDie");
    }
}
