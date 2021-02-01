using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinMax
{
    
    public float Min { get; private set; } // public to get, private to set
    public float Max { get; private set; }

    public MinMax()
    {
        Min = float.MaxValue;
        Max = float.MinValue;
    }

    // Update Min adn Max values
    public void AddValue(float v)
    {
        if (v > Max)
        {
            Max = v;
        }

        if (v < Min)
        {
            Min = v;
        }
    }
}
