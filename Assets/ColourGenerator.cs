using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColourGenerator
{
   ColourSettings settings;
   Texture2D texture;
   const int textureResolution = 50;

   public void UpdateSettings(ColourSettings settings)
   {
       this.settings = settings;
       
       if (texture == null)
       {
           texture = new Texture2D(textureResolution, 1); // width, height
       }
   }

   public void UpdateElevation(MinMax elevationMinMax)
   {
       // Send information to the shader, under name "_elevationMinMax"
       settings.planetMaterial.SetVector("_elevationMinMax", new Vector4(elevationMinMax.Min, elevationMinMax.Max));
   }

   public void UpdateColours()
   {
       Color[] colours = new Color[textureResolution];

       for (int i = 0; i < textureResolution; i++)
       {
           colours[i] = settings.gradient.Evaluate(i / (textureResolution - 1f));
       }

       texture.SetPixels(colours);
       texture.Apply();

       // Pass to shader
       settings.planetMaterial.SetTexture("_texture", texture);
   }
}
