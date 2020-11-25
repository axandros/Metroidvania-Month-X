using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PaletteSwapScript : MonoBehaviour
{
    //[SerializeField]
    //private Texture2D _SourcePalette;
    [SerializeField]
    private Texture2D _TargetPalette;

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
        
        ApplyTex();
    }

    private void ApplyTex()
    {
        if (_TargetPalette )//&& _SourcePalette)
        {
            //_mat.SetTexture(shPropTex, _TargetPalette);
            MPB.SetTexture("_Target", _TargetPalette);
            //MPB.SetTexture("_Source", _SourcePalette);
            _ren.SetPropertyBlock(MPB);
        }
    }
}
