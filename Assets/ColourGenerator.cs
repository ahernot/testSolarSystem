using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColourGenerator
{
    ColourSettings settings;
    Texture2D texture;
    const int textureResolution = 50;
    INoiseFilter biomeNoiseFilter;

    public void UpdateSettings(ColourSettings settings)
    {
        this.settings = settings;
        
        if (texture == null || texture.height != settings.biomeColourSettings.biomes.Length) // update texture if null or if height not equal to nb of biomes
        {
            // first half of each texture strip is ocean texture
            texture = new Texture2D(textureResolution * 2, settings.biomeColourSettings.biomes.Length, TextureFormat.RGBA32, false); // width, height (each row stores the colour for a biome) // disable mipmapping (lowres texture version for viewing from far away)
        }

        // Initialise biome noise filter
        biomeNoiseFilter = NoiseFilterFactory.CreateNoiseFilter(settings.biomeColourSettings.noise);
    }

    public void UpdateElevation(MinMax elevationMinMax)
    {
        // Send information to the shader, under name "_elevationMinMax"
        settings.planetMaterial.SetVector("_elevationMinMax", new Vector4(elevationMinMax.Min, elevationMinMax.Max));
    }


    // Calculate which biome the point is in
    public float BiomePercentFromPoint(Vector3 pointOnUnitSphere)
    {
        float heightPercent = (pointOnUnitSphere.y + 1) / 2f; // =0 at south pole, =1 at north pole
        heightPercent += (biomeNoiseFilter.Evaluate(pointOnUnitSphere) - settings.biomeColourSettings.noiseOffset) * settings.biomeColourSettings.noiseStrength; // add noise
        float biomeIndex = 0;
        int numBiomes = settings.biomeColourSettings.biomes.Length;

        // Calculate blending range
        float blendRange = settings.biomeColourSettings.blendAmount / 2f + .001f; // blending over a band of width 2x blendRange (+.001f to have it != 0)

        for (int i = 0; i < numBiomes; i++)
        {
            float dst = heightPercent - settings.biomeColourSettings.biomes[i].startHeight;
            float weight = Mathf.InverseLerp(-blendRange, blendRange, dst); // depends on whether distance dst is within range of blending distance

            // Increment biome index
            biomeIndex *= (1 - weight);
            biomeIndex += i * weight;
        }

        return biomeIndex / Mathf.Max(1, (numBiomes - 1));
    }



    public void UpdateColours()
    {
        // Create Color array
        Color[] colours = new Color[texture.width * texture.height];//textureResolution];

        // Initialize colour idnex
        int colourIndex = 0;

        // Run through each biome
        foreach (var biome in settings.biomeColourSettings.biomes)
        {
            for (int i = 0; i < textureResolution * 2; i++) // loop through width of texture
            {
                Color gradientCol;
                
                if (i < textureResolution)
                {
                    gradientCol = settings.oceanColour.Evaluate(i / (textureResolution - 1f)); // get color from ocean gradient
                }
                else
                {
                    gradientCol = biome.gradient.Evaluate((i - textureResolution) / (textureResolution - 1f)); // get color from biome gradient
                }
                
                Color tintCol = biome.tint;
                colours[colourIndex] = gradientCol * (1 - biome.tintPercent) + tintCol * biome.tintPercent;
                colourIndex++;
            }

        }

        texture.SetPixels(colours);
        texture.Apply();

        // Pass to shader
        settings.planetMaterial.SetTexture("_texture", texture);
    }
    }
