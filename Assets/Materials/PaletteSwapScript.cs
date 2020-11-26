using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Palette
{
    public Texture2D texture;
    public string name;
}

public class PaletteSwapScript : MonoBehaviour
{
    [SerializeField]
    private Texture2D _MainPalette;

    [SerializeField]
    private List<Palette> PaletteList;

    //private Material _mat;
    static readonly int shPropTex = Shader.PropertyToID("_Target");

    MaterialPropertyBlock _mpb;
    Renderer _ren;
    public MaterialPropertyBlock MPB{
        get {
            if (_mpb == null) {
                _mpb = new MaterialPropertyBlock();
            }
            return _mpb;
        }
   }


    private void OnValidate()
    {
        //_mat = GetComponent<Renderer>().material = new Material(GetComponent<Renderer>().material);
        _ren = GetComponent<Renderer>();
        
        ApplyTex(_MainPalette);
    }

    private void ApplyTex(Texture2D tex)
    {
        if (tex )//&& _SourcePalette)
        {
            MPB.SetTexture(shPropTex, tex);
            //MPB.SetTexture("_Target", _TargetPalette);
            _ren.SetPropertyBlock(MPB);
        }
    }

    public void Flash(Texture2D tex, float speed, float duration)
    {

    }
    
}
