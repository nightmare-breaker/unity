using System.Collections.Generic;
using UnityEngine;

public class GestureRecognizer : MonoBehaviour
{
    public RectTransform drawAreaRect;

    private List<Vector3> points = new List<Vector3>();
    private LineRenderer lineRenderer;
    private const float minDistance = 0.1f;

    private DollarRecognizer recognizer;
    private bool isDrawing = false;

    void Start()
    {
        // LineRenderer 설정
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.positionCount = 0;
        lineRenderer.widthMultiplier = 0.05f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.cyan;
        lineRenderer.endColor = Color.cyan;
        lineRenderer.useWorldSpace = true;

        recognizer = new DollarRecognizer();

        List<Vector2> triangle = new List<Vector2>()
        {
            new Vector2(0,0),
            new Vector2(1,1),
            new Vector2(2,0)
        };
        recognizer.SavePattern("Triangle", triangle);
    }

    void Update()
    {
        bool isInsideDrawArea = RectTransformUtility.RectangleContainsScreenPoint(
            drawAreaRect, Input.mousePosition, Camera.main);

        if (Input.GetMouseButtonDown(0) && isInsideDrawArea)
        {
            isDrawing = true;
            points.Clear();
            lineRenderer.positionCount = 0;
        }

        if (Input.GetMouseButton(0) && isDrawing && isInsideDrawArea)
        {
            Vector2 localPoint;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(drawAreaRect, Input.mousePosition, Camera.main, out localPoint))
            {
                Vector3 worldPos = drawAreaRect.TransformPoint(localPoint);

                if (points.Count == 0 || Vector3.Distance(points[points.Count - 1], worldPos) > minDistance)
                {
                    points.Add(worldPos);
                    lineRenderer.positionCount = points.Count;
                    lineRenderer.SetPosition(points.Count - 1, worldPos);
                }
            }
        }

        if (Input.GetMouseButtonUp(0) && isDrawing)
        {
            isDrawing = false;
            List<Vector2> points2D = ConvertToVector2(points);

            DollarRecognizer.Result result = recognizer.Recognize(points2D);
            Debug.Log($"Recognized Gesture: {result.Match?.Name ?? "None"} (Score: {result.Score:F3})");
        }
    }

    private List<Vector2> ConvertToVector2(List<Vector3> input)
    {
        List<Vector2> result = new List<Vector2>();
        foreach (Vector3 v in input)
        {
            result.Add(new Vector2(v.x, v.y));
        }
        return result;
    }
}