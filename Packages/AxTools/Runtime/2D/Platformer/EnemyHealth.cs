﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AxTools.retro
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(CapsuleCollider2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class EnemyHealth : MonoBehaviour
    {
        [SerializeField, Tooltip("The maximum health of this enemy.")]
        private int _Health = 1;
        [SerializeField, Tooltip("How much damage this enemy deals when colliding with the player.")]
        private int _collisionDamage = 1;

        private int _currentHealth;
        public int HitPoints { get { return _Health; } }
        public int CollisionDamage { get { return _collisionDamage; } }

        [SerializeField, Tooltip("The room the enemy is locked to.")]
        private Room _roomAssigned;
        public Room RoomAssigned { get { return _roomAssigned; } set { _roomAssigned = value; } }

        private bool _isActive = true;
        public bool Active
        {
            get { return _isActive; }
            set
            {
                _isActive = value;
                if (value)
                {
                    _currentHealth = 1;
                    if (_cc) { _cc.enabled = true; }
                    if (_anim) { _anim.SetBool("Dead", false); }
                    if (_sr) { _sr.enabled = true; }
                }
                else
                {
                    if (_cc) { _cc.enabled = false; }
                    if (_sr) { _sr.enabled = false; }
                }
                if (OnActiveChange != null) { OnActiveChange.Invoke(_isActive); }
            }
        }

        /// <summary>
        /// A delegate that triggers when the enemy becomes active or inactive.
        /// </summary>
        /// <param name="setActive">The state, on or off.</param>
        public delegate void ActivationDelegate(bool setActive);
        /// <summary>
        /// The delegate to subscribe to for activation changes.
        /// </summary>
        public ActivationDelegate OnActiveChange;

        private Animator _anim;
        private CapsuleCollider2D _cc;
        private Rigidbody2D _rb;
        private SpriteRenderer _sr;


        private void Start()
        {
            // Cache components
            _anim = GetComponent<Animator>();
            _rb = GetComponent<Rigidbody2D>();
            _cc = GetComponent<CapsuleCollider2D>();
            _sr = GetComponent<SpriteRenderer>();

            // Set the current state.
            _currentHealth = _Health;
        }

        /// <summary>
        /// Deal X damage to the monster.
        /// </summary>
        /// <param name="damage">The damage amount to deal</param>
        /// <param name="type">The elemental type of damage for resistance and weakneses.</param>
        /// <returns></returns>
        public int DealDamage(int damage, DamageType type = DamageType.NONE)
        {
            _Health -= damage;
            if (_Health <= 0)
            {
                Death();
            }
            return damage;
        }

        /// <summary>
        /// What needs to be done when the monster runs out of health.
        /// </summary>
        void Death()
        {
            Active = false;
            _anim.SetBool("Dead", true);
            _cc.enabled = false;

            AudioManager.Play("EnemyDie");
        }
    }
}
