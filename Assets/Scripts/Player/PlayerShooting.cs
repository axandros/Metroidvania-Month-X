using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [SerializeField]
    private GameObject _BulletPrefab;
    [SerializeField]
    private Transform _SpawnOffset;
    [SerializeField]
    private float _ShotCooldown = 0.25f;
    [SerializeField]
    private float _BulletSpeed=5;

    private float _lastShotTime = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        bool shoot = Input.GetButtonDown("Fire1");

        if (shoot && Time.time - _lastShotTime > _ShotCooldown)
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        _lastShotTime = Time.deltaTime;
        GameObject go = Instantiate(_BulletPrefab, _SpawnOffset.position, _SpawnOffset.rotation);
        go.transform.localScale = this.transform.localScale;
        Rigidbody2D rb = go.GetComponent<Rigidbody2D>();
        if (rb)
        {
            int direction = this.transform.localScale.x >= 0 ? 1 : -1;
            rb.velocity = new Vector2(direction*_BulletSpeed, 0);
        }
    }
}
