using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    bool _isPaused = false;

    [SerializeField]
    Text PauseText;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        SetPause(!_isPaused);
    }

    public void SetPause(bool state)
    {
        if (_isPaused != state)
        {
            if (state)
            {
                Time.timeScale = 0;
                if (PauseText) { PauseText.gameObject.SetActive(true); }
            }
            else
            {
                Time.timeScale = 1;
                if (PauseText) { PauseText.gameObject.SetActive(false); }
            }
            _isPaused = state;
        }
    }
}
