using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class CustomEdge : Edge
{
    private Vector2 startPoint;
    private Vector2 endPoint;

    // Constructor to initialize the edge with start and end points
    public CustomEdge(Vector2 startPoint, Vector2 endPoint)
    {
        this.startPoint = startPoint;
        this.endPoint = endPoint;
        RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
    }

    // Callback method to draw the edge when the geometry changes
    private void OnGeometryChanged(GeometryChangedEvent evt)
    {
        // Clear existing drawing
        Clear();

        // Draw a straight line from start to end point
        VisualElement line = new VisualElement();
        line.style.backgroundColor = Color.white;
        line.style.position = Position.Absolute;
        line.style.left = startPoint.x;
        line.style.top = startPoint.y;
        line.style.width = endPoint.x - startPoint.x;
        line.style.height = endPoint.y - startPoint.y;
        Add(line);
    }
}