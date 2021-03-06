using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utilities {

    public static void DrawCircle(GameObject container, float radius, float lineWidth, bool enableOnly = true)
    {
        if(container.GetComponent<LineRenderer>() == null) container.AddComponent<LineRenderer>();
        else
        {
            container.GetComponent<LineRenderer>().enabled = true;
            //Only enable if component is not null
            if(enableOnly) return;
        }

        LineRenderer line = container.GetComponent<LineRenderer>();
        line.enabled = true;

        line.useWorldSpace = false;
        line.startWidth = lineWidth;
        line.endWidth = lineWidth;

        updateCircle(line, radius);
    }

    public static void updateCircle(LineRenderer line, float radius)
    {
        line.positionCount = 0;

        int segments = 45;
        int pointCount = segments + 1;
        line.positionCount = pointCount;
        Vector3[] points = new Vector3[pointCount];

        for (int i = 0; i < pointCount; i++)
        {
            float rad = Mathf.Deg2Rad * (i * 360f / segments);
            points[i] = new Vector3(Mathf.Sin(rad) * radius, 0, Mathf.Cos(rad) * radius);
        }

        line.SetPositions(points);
    }

}
