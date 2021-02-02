using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeGenerator
{
    ShapeSettings settings;
    INoiseFilter[] noiseFilters; // array of generic noise filters (it can hold any type)
    public MinMax elevationMinMax;

    public void UpdateSettings(ShapeSettings settings)
    {
        this.settings = settings;

        // Initialize noiseFilters array of noise filters
        noiseFilters = new INoiseFilter[settings.noiseLayers.Length];

        // Populate noiseFilters array
        for (int i = 0; i < noiseFilters.Length; i++)
        {
            noiseFilters[i] = NoiseFilterFactory.CreateNoiseFilter(settings.noiseLayers[i].noiseSettings);//new SimpleNoiseFilter(settings.noiseLayers[i].noiseSettings); // new noise filter with settings
        }

        // Generate elevationMinMax
        elevationMinMax = new MinMax();
    }

    // Calculate point on planet
    public float CalculateUnscaledElevation(Vector3 pointOnUnitSphere)
    {
        float firstLayerValue = 0;
        float elevation = 0; // initialize added elevation due to noise

        // Process first noise layer
        if (noiseFilters.Length > 0)
        {
            firstLayerValue = noiseFilters[0].Evaluate(pointOnUnitSphere);
            if (settings.noiseLayers[0].enabled)
            {
                elevation = firstLayerValue;
            }
        }

        // Loop through all the overlapping noise layers to calculate total added elevation due to noise
        for (int i = 1; i < noiseFilters.Length; i++)
        {
            if (settings.noiseLayers[i].enabled) // check if layer is enabled
            {
                float mask = (settings.noiseLayers[i].useFirstLayerAsMask) ? firstLayerValue : 1; // if true, then =firstLayerValue; else =1 (no mask)
                elevation += noiseFilters[i].Evaluate(pointOnUnitSphere) * mask; // calculate added elevation due to noise layer
            }
        }

        // Add elevation to elevationMinMax
        elevationMinMax.AddValue(elevation);

        // Calculate point on planet, factoring in displacement due to total noise elevation
        return elevation;
    }



    public float GetScaledElevation(float unscaledElevation)
    {
        float elevation = Mathf.Max(0, unscaledElevation);
        elevation = settings.planetRadius * (1 + elevation);
        return elevation;
    }
}
