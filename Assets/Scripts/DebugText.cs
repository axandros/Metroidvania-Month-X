using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class DebugText : MonoBehaviour
{
    private struct DebugString
    {
        public string text;
        public float time;
        public int displayed;
        public override string ToString()
        {
            return text + " | " + time + ", " + displayed;
        }
    }
    Text _text;
    static DebugText _debugText;

    List<DebugString> _debugStrings;
    
    // Start is called before the first frame update
    void Start()
    {
        _debugText = this;
        _text = GetComponent<Text>();
        _debugStrings = new List<DebugString>();
    }

    // Update is called once per frame
    void Update()
    {
        string display = "";

        int i = 0;
        while(i < _debugStrings.Count)
        {
            if(Time.time > _debugStrings[i].time && _debugStrings[i].displayed > 0)
            {
                Debug.Log("Discarding " + _debugStrings[i].text);
                _debugStrings.RemoveAt(i);
            }
            else
            {
                DebugString str = _debugStrings[i];
                display += str.text + "\n";
                str.displayed += 1;
                _debugStrings[i] = str;
                
                //Debug.Log(_debugStrings[i].text);
                i++;
            }
        }

        _text.text = display;
    }


    public static void Log(string DisplayText, float displayTime = 0)
    {
        if (_debugText)
        {
            _debugText.internalLog(DisplayText, displayTime);
        }
        else
        {
            Debug.Log(DisplayText);
        }
    }

    private void internalLog(string DisplayText, float displayTime = 0)
    {
        DebugString str = new DebugString();
        str.text = DisplayText;
        str.time = Time.time + displayTime;
        str.displayed = 0;
        _debugStrings.Add(str);
    }
}
