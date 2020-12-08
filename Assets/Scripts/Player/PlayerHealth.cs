using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlatformerCharacter))]
public class PlayerHealth : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private int _Health = 10;
    [SerializeField]
    private float _IFrameDuration = 1.5f;
    [SerializeField, Tooltip("The number of unity units to knock the character back, assuming flat ground.")]
    float KnockbackDistance = 2;
    public int HitPoints { get { return _Health; } }
    public float IFrameDuraiton { get { return _IFrameDuration; } }

    private float _timeLastHit = 0;
    private PlatformerCharacter _pc;

    void Start()
    {
        _pc = GetComponent<PlatformerCharacter>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        EnemyHealth eh = collision.gameObject.GetComponent<EnemyHealth>();
        if (eh && eh.CollisionDamage > 0 && Time.time - _timeLastHit > _IFrameDuration)
        {
            _pc.Knockback(collision.transform);
            ApplyDamage(eh.CollisionDamage);
            _timeLastHit = Time.time;
        }
    }
    /// <summary>2
    /// Deal damage to the player.
    /// </summary>
    /// <param name="damage">Damage to deal.</param>
    /// <param name="type">Optional damage type for weakness/resistance. defaults to NONE</param>
    /// <returns>Returns the actual damage dealt to the player.</returns>
    public int ApplyDamage(int damage, DamageType type = DamageType.NONE)
    {
        // Do any math for weakness/resistance.

        // Apply the damage to the player.
        _Health -= damage;

        AudioManager.Play("Hurt");

        // Return actual damage dealt.
        return damage;
    }
}
