using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RidgidNoiseFilter: INoiseFilter
{
    
    NoiseSettings.RidgidNoiseSettings settings;
    Noise noise = new Noise();

    public RidgidNoiseFilter(NoiseSettings.RidgidNoiseSettings settings)
    {
        this.settings = settings;
    }

    public float Evaluate(Vector3 point)
    {
        float noiseValue = 0;
        float frequency = settings.baseRoughness;
        float amplitude = 1;
        float weight = 1;

        // Generate noise layers
        for (int i = 0; i < settings.numLayers; i++)
        {
            float v = 1 - Mathf.Abs(noise.Evaluate(point * frequency + settings.centre));
            v *= v;

            v *= weight;
            weight = Mathf.Clamp01(v * settings.weightMultiplier); // propagate into weight to get clearly defined ridges and crevaces across multiple superimposed layers // Clamp01 clamps the value between 0 and 1

            noiseValue += v * amplitude;

            frequency *= settings.roughness; // increment (increase) layer frequency
            amplitude *= settings.persistence; // increment (decrease) layer amplitude

        }

        noiseValue = noiseValue - settings.minValue; //Mathf.Max(0, noiseValue - settings.minValue); // minimum altitude is base sphere
        
        return noiseValue * settings.strength;
    }


}