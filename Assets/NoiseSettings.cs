using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] // show up in inspector
public class NoiseSettings
{
    public float strength = 1;
    [Range(1, 8)]
    public int numLayers = 1; // number of superimposed noise layers
    public float baseRoughness = 1;
    public float roughness = 2;
    public float persistence = .5f; // each noise layer's contribution is half of the previous one
    public Vector3 centre; // centre of noise
    public float minValue;

}
