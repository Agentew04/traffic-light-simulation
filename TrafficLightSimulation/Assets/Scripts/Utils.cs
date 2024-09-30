using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static float Remap(this float value, Vector2 inputScale, Vector2 outputScale) {
        return (value - inputScale.x) / (inputScale.y - inputScale.x) * (outputScale.y - outputScale.x) + outputScale.x;
    }
}
