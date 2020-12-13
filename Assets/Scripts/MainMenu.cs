using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    GameObject PauseText;

    [SerializeField]
    GameObject _Cursor;

    [SerializeField]
    List<GameObject> _Options;

    Vector3 _cursorOffset;

    int _cursorPosition = 0;
    int _soundVolumeDec = 5;

    [SerializeField]
    TextMeshProUGUI _VolumeDisplay;
    [SerializeField]
    TextMeshProUGUI _ResolutionDisplay;

    [SerializeField]
    string PlayScene = "SampleScene";

    int _resScale = 5;
    bool _fullScreen = false;

    // Start is called before the first frame update
    void Start()
    {
        if (_Options.Count > 0)
            _cursorOffset = _Cursor.transform.position - _Options[0].transform.position;
        // TODO: Move to coroutine to start after 1/2 sec?
        AudioManager.Play("Theme");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            MoveSelector(true);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            MoveSelector(false);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)){
            SoundVolume(1);
            ResolutionUpdate(1);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)){
            SoundVolume(-1);
            ResolutionUpdate(-1);
        }
        else if (Input.GetKeyDown(KeyCode.Return))
        {
            Select();
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    void Select()
    {
        switch (_cursorPosition)
        {
            case 0: SceneManager.LoadScene("SampleScene");
                break;
            case 1: 
                break;
            case 2:
                break;
            case 3:
                _fullScreen = !_fullScreen;
                ScreenChange();
                break;
            case 4:
                Application.Quit();
                break;
        }
    }
    void MoveSelector(bool up)
    {
        if (up)
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

    void SoundVolume(int changeAmount)
    {
        if(_cursorPosition == 1)
        {
            _soundVolumeDec = Mathf.Clamp(changeAmount + _soundVolumeDec, 0, 9);
            if (_VolumeDisplay)
            {
                _VolumeDisplay.text = _soundVolumeDec.ToString();
            }
            AudioManager.SetVolume(_soundVolumeDec/9.0f);
        }
    }

    void UpdateCursor()
    {
        _Cursor.transform.position = _Options[_cursorPosition].transform.position + _cursorOffset;
    }

    void ResolutionUpdate(int Changeamount)
    {
        if (_cursorPosition == 2)
        {
            _resScale = Mathf.Clamp(_resScale + Changeamount, 1, 12);
            if (_ResolutionDisplay)
            {
                _ResolutionDisplay.text = _resScale.ToString();
            }
            ScreenChange();
        }
    }

    void ScreenChange()
    {
        int Width = 160 * _resScale;
        int Height = 144 * _resScale;
        Screen.SetResolution(Width, Height, _fullScreen);
    }
}
