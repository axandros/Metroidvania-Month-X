using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class GameWinScript : MonoBehaviour
{
    [SerializeField]
    CanvasGroup _WinScreen;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlatformerCharacter ph = collision.gameObject.GetComponent<PlatformerCharacter>();
        if (ph != null)
        {
            // Remove Player Control
            ph.enabled = false;
            // Play Win Sound
            AudioManager.Play("Win");
            // Open win Screen
            if (_WinScreen) {
                StartCoroutine(Enlighten());
                    }
        }
    }

    IEnumerator Enlighten(){
        while (_WinScreen.alpha < 1)
        {
            _WinScreen.alpha += 0.22f;
            yield return new WaitForSeconds(0.2f);
        }
    }
}
