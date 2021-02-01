using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface INoiseFilter
{
    // Method that any class needs to have in order to qualify as a noise filter
    float Evaluate(Vector3 point);
}
