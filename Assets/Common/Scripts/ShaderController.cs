using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static  class ShaderController {
    public enum ShaderData { BOUNDARY, LIGHTINTENSITY, EFFECTAMOUNT, CONTRAST }
    [SerializeField]
    static Material defaultMaterial;

    const string _Boundary = "_Boundary";
    const string _LightIntensity = "_LightIntensity";
    const string _EffectAmount = "_EffectAmount";
    const string _Contrast = "_Contrast";


    public static void ChangeBoundary(Material material, ShaderData shaderData, float value)
    {
        switch (shaderData)
        {
            case ShaderData.BOUNDARY:
                material.SetFloat(_Boundary, value);
                break;
            case ShaderData.LIGHTINTENSITY:
                material.SetFloat(_LightIntensity, value);
                break;
            case ShaderData.EFFECTAMOUNT:
                material.SetFloat(_EffectAmount, value);
                break;
            case ShaderData.CONTRAST:
                material.SetFloat(_Contrast, value);
                break;
        }
    }
}
