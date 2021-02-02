using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainFace {

    ShapeGenerator shapeGenerator; // planet shape generator
    Mesh mesh;
    int resolution;
    Vector3 localUp;
    Vector3 axisA;
    Vector3 axisB;

    // Class constructor
    public TerrainFace(ShapeGenerator shapeGenerator, Mesh mesh, int resolution, Vector3 localUp)
    {
        this.shapeGenerator = shapeGenerator;
        this.mesh = mesh;
        this.resolution = resolution;
        this.localUp = localUp;

        axisA = new Vector3(localUp.y, localUp.z, localUp.x); // perpendicular to 
        axisB = Vector3.Cross(localUp, axisA); // perpendicular to localUp and axisA
    }

    public void ConstructMesh()
    {
        Vector3[] vertices = new Vector3[resolution * resolution];
        int[] triangles = new int[(resolution - 1) * (resolution - 1) * 6]; // number of triangles for a square mesh with resolution vertices per side, times 3 for the number of vertex triplets
        int triIndex = 0;

        Vector2[] uv = (mesh.uv.Length == vertices.Length) ? mesh.uv : new Vector2[vertices.Length]; // save uv before resetting mesh, to avoid recalculating

        for (int y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++)
            {
                int i = x + y*resolution; // number of total iterations

                Vector2 percent = new Vector2(x, y) / (resolution - 1); // percent on the x-axis and y-axis, between 0 and 1
                Vector3 pointOnUnitCube = localUp + (percent.x - .5f)*2*axisA + (percent.y - .5f)*2*axisB; // sum of base vectors
                Vector3 pointOnUnitSphere = pointOnUnitCube.normalized;

                float unscaledElevation = shapeGenerator.CalculateUnscaledElevation(pointOnUnitSphere);
                vertices[i] = pointOnUnitSphere * shapeGenerator.GetScaledElevation(unscaledElevation);
                uv[i].y = unscaledElevation;

                // Create the triangles, if not on right or bottom edges
                if (x != resolution - 1 && y != resolution - 1)
                {
                    // Create triangle South-East
                    triangles[triIndex] = i;
                    triangles[triIndex + 1] = i + resolution + 1;
                    triangles[triIndex + 2] = i + resolution;

                    // Create triangle East-South
                    triangles[triIndex + 3] = i;
                    triangles[triIndex + 4] = i + 1;
                    triangles[triIndex + 5] = i + resolution + 1;

                    // Increment triIndex
                    triIndex += 6;
                }
            }
        }

        // Refresh mesh
        mesh.Clear(); // clear all data to avoid errors when downgrading resolution
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        mesh.uv = uv; // reassigned saved uv
    }


    public void UpdateUVs(ColourGenerator colourGenerator)
    {
        Vector2[] uv = mesh.uv;

        for (int y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++)
            {
                int i = x + y*resolution; // number of total iterations

                Vector2 percent = new Vector2(x, y) / (resolution - 1); // percent on the x-axis and y-axis, between 0 and 1
                Vector3 pointOnUnitCube = localUp + (percent.x - .5f)*2*axisA + (percent.y - .5f)*2*axisB; // sum of base vectors
                Vector3 pointOnUnitSphere = pointOnUnitCube.normalized;

                uv[i].x = colourGenerator.BiomePercentFromPoint(pointOnUnitSphere);
            }
        }

        // Update class parameter
        mesh.uv = uv;

    }

}



