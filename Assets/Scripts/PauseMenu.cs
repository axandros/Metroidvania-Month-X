using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    bool _isPaused = false;

    [SerializeField]
    GameObject PauseText;

    [SerializeField]
    GameObject _Cursor;

    [SerializeField]
    List<GameObject> _Options;

    Vector3 _cursorOffset;

    int _cursorPosition = 0;

    private void Start()
    {
        if (_Options.Count > 0)
            _cursorOffset = _Cursor.transform.position - _Options[0].transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            MoveSelector(true);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            MoveSelector(false);
        }
        else if (Input.GetKeyDown(KeyCode.Return))
        {
            Select();
        }
    }

    void Select()
    {
        switch (_cursorPosition)
        {
            case 0: SetPause(false); break;
            case 1:
                Time.timeScale = 1;
                SceneManager.LoadScene("MainMenu");
                break;
        }
    }

    void MoveSelector(bool up)
    {
        if(up)
        {
            _cursorPosition--;
            if (_cursorPosition < 0)
            {
                _cursorPosition = _Options.Count - 1;
            }
        }
        else
        {
            _cursorPosition++;
            if (_cursorPosition >= _Options.Count)
            {
                _cursorPosition = 0;
            }
        }
        UpdateCursor();
    }

    void UpdateCursor()
    {
        _Cursor.transform.position = _Options[_cursorPosition].transform.position + _cursorOffset;
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
