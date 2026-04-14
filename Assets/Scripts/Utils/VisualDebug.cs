using System.Collections.Generic;
using UnityEngine;

public class VisualDebug : Singleton<VisualDebug>
{
    private class Sphere
    {
        public Sphere(Vector3 position, float radius, Color color, float time)
        {
            this.position = position;
            this.radius = radius;
            this.color = color;
            this.time = time;
        }

        public Vector3 position;
        public float radius;
        public Color color;
        public float time;
    }
    private class Line
    {
        public Line(Vector3 startPosition, Vector3 endPosition, Color color, float time)
        {
            this.startPosition = startPosition;
            this.endPosition = endPosition;
            this.color = color;
            this.time = time;
        }

        public Vector3 startPosition;
        public Vector3 endPosition;
        public Color color;
        public float time;
    }

    private readonly List<Sphere> _spheres = new();
    private readonly List<Line> _lines = new();

    public void DrawSphere(Vector3 position, float radius = 1.0f, Color? color = null, float time = 0.0f)
    {
#if UNITY_EDITOR
        if (color == null)
        {
            color = Color.red;
        }

        _spheres.Add(new Sphere(position, radius, color.Value, time));
#endif
    }

    public void DrawLine(Vector3 startposition, Vector3 endPosition, Color? color = null, float time = 0.0f)
    {
#if UNITY_EDITOR
        if (color == null)
        {
            color = Color.red;
        }

        _lines.Add(new Line(startposition, endPosition, color.Value, time));
#endif
    }

    public void DrawBox(Vector3 center, Vector3 halfExtents, Quaternion orientation, Color? color = null, float time = 0.0f)
    {
#if UNITY_EDITOR
        if (color == null)
        {
            color = Color.red;
        }

        List<Vector3> corners = new();
        corners.Add(center + orientation * new Vector3(-halfExtents.x, -halfExtents.y, -halfExtents.z));
        corners.Add(center + orientation * new Vector3(halfExtents.x, -halfExtents.y, -halfExtents.z));
        corners.Add(center + orientation * new Vector3(halfExtents.x, -halfExtents.y, halfExtents.z));
        corners.Add(center + orientation * new Vector3(-halfExtents.x, -halfExtents.y, halfExtents.z));
        corners.Add(center + orientation * new Vector3(-halfExtents.x, halfExtents.y, -halfExtents.z));
        corners.Add(center + orientation * new Vector3(halfExtents.x, halfExtents.y, -halfExtents.z));
        corners.Add(center + orientation * new Vector3(halfExtents.x, halfExtents.y, halfExtents.z));
        corners.Add(center + orientation * new Vector3(-halfExtents.x, halfExtents.y, halfExtents.z));

        // Bottom
        _lines.Add(new Line(corners[0], corners[1], color.Value, time));
        _lines.Add(new Line(corners[1], corners[2], color.Value, time));
        _lines.Add(new Line(corners[2], corners[3], color.Value, time));
        _lines.Add(new Line(corners[3], corners[0], color.Value, time));

        // Top
        _lines.Add(new Line(corners[4], corners[5], color.Value, time));
        _lines.Add(new Line(corners[5], corners[6], color.Value, time));
        _lines.Add(new Line(corners[6], corners[7], color.Value, time));
        _lines.Add(new Line(corners[7], corners[4], color.Value, time));

        // Vertical edges
        _lines.Add(new Line(corners[0], corners[4], color.Value, time));
        _lines.Add(new Line(corners[1], corners[5], color.Value, time));
        _lines.Add(new Line(corners[2], corners[6], color.Value, time));
        _lines.Add(new Line(corners[3], corners[7], color.Value, time));
#endif
    }

    public void DrawLine(Ray ray, float distance, Color? color = null)
        => DrawLine(ray.origin, ray.GetPoint(distance), color);

    private void OnDrawGizmos()
    {
#if UNITY_EDITOR
        List<Sphere> spheresToRemove = new();
        foreach (var sphere in _spheres)
        {
            Gizmos.color = sphere.color;
            Gizmos.DrawWireSphere(sphere.position, sphere.radius);

            if (sphere.time <= 0.0f)
            {
                spheresToRemove.Add(sphere);
            }
            else
            {
                sphere.time -= Time.deltaTime;
            }
        }
        foreach (var s in spheresToRemove)
        {
            _spheres.Remove(s);
        }

        List<Line> lineToRemove = new();
        foreach (var line in _lines)
        {
            Gizmos.color = line.color;
            Gizmos.DrawLine(line.startPosition, line.endPosition);

            if (line.time <= 0.0f)
            {
                lineToRemove.Add(line);
            }
            else
            {
                line.time -= Time.deltaTime;
            }
        }
        foreach (var l in lineToRemove)
        {
            _lines.Remove(l);
        }
#endif
    }
}