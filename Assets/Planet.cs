using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{

    [Range(2, 256)]
    public int resolution = 10;
    public bool autoUpdate = true; // auto updating
    public enum FaceRenderMask { All, Top, Bottom, Left, Right, Front, Back };
    public FaceRenderMask faceRenderMask;

    // Create shape and colour settings
    public ShapeSettings shapeSettings;
    public ColourSettings colourSettings;

    // Foldout status of custom settings fields in inspector
    [HideInInspector]
    public bool shapeSettingsFoldout;
    [HideInInspector]
    public bool colourSettingsFoldout;


    // Create shape generator object
    ShapeGenerator shapeGenerator = new ShapeGenerator();

    // Create colour generator object
    ColourGenerator colourGenerator = new ColourGenerator();

    // Create mesh filter and terrain face arrays
    [SerializeField, HideInInspector]
    MeshFilter[] meshFilters;
    TerrainFace[] terrainFaces;



    // Initialize mesh
    void Initialize()
    {

        // Initialize shapeGenerator
        shapeGenerator.UpdateSettings(shapeSettings);

        // Initialize colourGenerator
        colourGenerator.UpdateSettings(colourSettings);
        
        // Reinitialise mesh filters if array is empty
        if (meshFilters == null || meshFilters.Length == 0)
        {
            meshFilters = new MeshFilter[6];
        }
        
        terrainFaces = new TerrainFace[6];

        Vector3[] directions = {Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back};

        for (int i = 0; i < 6; i++)
        {
            if (meshFilters[i] == null)
            {
                GameObject meshObj = new GameObject("mesh");
                meshObj.transform.parent = transform;

                meshObj.AddComponent<MeshRenderer>(); // .sharedMaterial = new Material(Shader.Find("Standard")); // mesh renderer
                meshFilters[i] = meshObj.AddComponent<MeshFilter>();
                meshFilters[i].sharedMesh = new Mesh();
            }

            // Set material
            meshFilters[i].GetComponent<MeshRenderer>().sharedMaterial = colourSettings.planetMaterial;

            terrainFaces[i] = new TerrainFace(shapeGenerator, meshFilters[i].sharedMesh, resolution, directions[i]);

            // Render face by face
            bool renderFace = faceRenderMask == FaceRenderMask.All || (int)faceRenderMask - 1 == i;
            meshFilters[i].gameObject.SetActive(renderFace);
        }

    }


    // Generate the whole planet
    public void GeneratePlanet()
    {
        Initialize();
        GenerateMesh();
        GenerateColours();
    }


    // Update the planet's shape
    public void OnShapeSettingsUpdated()
    {
        if (autoUpdate)
        {
            Initialize();
            GenerateMesh();
        }
    }


    // Update the planet's colour
    public void OnColourSettingsUpdated()
    {
        if (autoUpdate)
        {
            Initialize();
            GenerateColours();
        }
    }


    // Generate mesh
    void GenerateMesh()
    {
        // Run through all 6 faces
        for (int i = 0; i < 6; i++)
        {
            if (meshFilters[i].gameObject.activeSelf)
            {
                terrainFaces[i].ConstructMesh(); //face.ConstructMesh();
            }
        }

        // Update colours
        colourGenerator.UpdateElevation(shapeGenerator.elevationMinMax);
    }


    // Generate colour of the meshes
    void GenerateColours()
    {
        colourGenerator.UpdateColours();

        // Run through all 6 faces
        for (int i = 0; i < 6; i++)
        {
            if (meshFilters[i].gameObject.activeSelf)
            {
                terrainFaces[i].UpdateUVs(colourGenerator);
            }
        }

    }
}
