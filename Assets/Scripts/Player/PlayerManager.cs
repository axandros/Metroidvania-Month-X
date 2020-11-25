using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlatformerCharacter))]
[RequireComponent(typeof(PlayerHealth))]
[RequireComponent(typeof(PlayerShooting))]
public class PlayerManager : MonoBehaviour
{
    PlayerShooting _ps;
    PlatformerCharacter _pc;
    PlayerHealth _ph;

    // Start is called before the first frame update
    void Start()
    {
        _ps = GetComponent<PlayerShooting>();
        _pc = GetComponent<PlatformerCharacter > ();
        _ph = GetComponent<PlayerHealth>();
    }
}
