using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseFilter
{
    
    NoiseSettings settings;
    Noise noise = new Noise();

    public NoiseFilter(NoiseSettings settings)
    {
        this.settings = settings;
    }

    public float Evaluate(Vector3 point)
    {
        float noiseValue = 0;
        float frequency = settings.baseRoughness;
        float amplitude = 1;

        // Generate noise layers
        for (int i = 0; i < settings.numLayers; i++)
        {
            float v = noise.Evaluate(point * frequency + settings.centre);
            noiseValue += (v + 1) * .5f * amplitude; // (noise.Evaluate(point * settings.roughness + settings.centre) + 1) * .5f; // between -1 and 1, cast between 0 and 1

            frequency *= settings.roughness; // increment (increase) layer frequency
            amplitude *= settings.persistence; // increment (decrease) layer amplitude
        }

        noiseValue = Mathf.Max(0, noiseValue - settings.minValue); // minimum altitude is base sphere
        
        return noiseValue * settings.strength;
    }


}
