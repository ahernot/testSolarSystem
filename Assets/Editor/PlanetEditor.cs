using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Planet))]
public class NewBehaviourScript : Editor
{
    Planet planet;

    // Create editors
    Editor shapeEditor;
    Editor colourEditor;



    // Override the default OnInspectorGUI
    public override void OnInspectorGUI()
    {
        using (var check = new EditorGUI.ChangeCheckScope())
        {
            // Display the default inspector GUI
            base.OnInspectorGUI();

            // Generate planet when edited
            if (check.changed)
            {
                planet.GeneratePlanet();
            }
        }

        // Manually generate planet
        if (GUILayout.Button("Generate Planet"))
        {
            planet.GeneratePlanet();
        }

        // Display the custom editor
        DrawSettingsEditor(planet.shapeSettings, planet.OnShapeSettingsUpdated, ref planet.shapeSettingsFoldout, ref shapeEditor);
        DrawSettingsEditor(planet.colourSettings, planet.OnColourSettingsUpdated, ref planet.colourSettingsFoldout, ref colourEditor);
    }



    /// <summary>Draw a settings editor</summary>
    /// <param name="settings">the settings</param>
    /// <param name="onSettingsUpdated"></param>
    /// <param name="foldout">the folded status of the editor, passed by reference to be modifiable by this function</param>
    /// <param name="editor">the editor</param>
    void DrawSettingsEditor(Object settings, System.Action onSettingsUpdated, ref bool foldout, ref Editor editor)
    {

        if (settings != null)
        {
            // Draw editor title bar and update foldout
            foldout = EditorGUILayout.InspectorTitlebar(foldout, settings);

            using (var check = new EditorGUI.ChangeCheckScope())
            {
                // Draw editor if foldout is true
                if (foldout)
                {
                    CreateCachedEditor(settings, null, ref editor); // create editor from cache
                    editor.OnInspectorGUI(); // display editor

                    // Update editor if parameters changed
                    if (check.changed)
                    {
                        if (onSettingsUpdated != null)
                        {
                            onSettingsUpdated();
                        }

                    }
                }
            }
        }
    }



    private void OnEnable()
    {
        planet  = (Planet)target;
    }
    
}
