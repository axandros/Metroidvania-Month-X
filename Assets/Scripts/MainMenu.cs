﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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
    float _soundVolumeDec = 9;

    [SerializeField]
    TextMeshProUGUI _VolumeDisplay;

    // Start is called before the first frame update
    void Start()
    {
        if (_Options.Count > 0)
            _cursorOffset = _Cursor.transform.position - _Options[0].transform.position;

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
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)){
            SoundVolume(-1);
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
            case 0: break;
            case 1: 
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

    void SoundVolume(int amount)
    {
        if(_cursorPosition == 1)
        {
            _soundVolumeDec = Mathf.Clamp(amount + _soundVolumeDec, 0, 9);
            if (_VolumeDisplay)
            {
                _VolumeDisplay.text = _soundVolumeDec.ToString();
            }
        }
    }

    void UpdateCursor()
    {
        _Cursor.transform.position = _Options[_cursorPosition].transform.position + _cursorOffset;
    }
}