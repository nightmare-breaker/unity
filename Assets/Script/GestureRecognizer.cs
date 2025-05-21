using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Gesture Recognizer with full $1 Unistroke Recognizer preprocessing,
/// densely sampled bidirectional templates, and detailed debug logs.
/// Includes both forward and reverse templates for direction invariance.
/// </summary>
public class GestureRecognizer : MonoBehaviour
{
    [Header("Draw Area")] public RectTransform drawAreaRect;
    [Header("Recognition Settings")]
    [Tooltip("Number of points to resample")] public int NumPoints = 64;
    [Tooltip("Square size for scaling")] public float TemplateSize = 300f;
    [Tooltip("Score threshold for accepting a match")] [Range(0f,1f)] public float ScoreThreshold = 0.85f;
    public UnityEvent<string> OnSpellRecognized = new UnityEvent<string>();

    private List<Vector2> points = new List<Vector2>();
    private LineRenderer lineRenderer;
    private DollarRecognizer recognizer;
    private bool isDrawing;

    void Start()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.positionCount = 0;
        lineRenderer.widthMultiplier = 0.1f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.useWorldSpace = true;
        lineRenderer.numCapVertices = 10;

        recognizer = new DollarRecognizer();
        CreateTemplates();
        Debug.Log("[GestureRecognizer] Loaded bidirectional templates");
    }

    /// <summary>
    /// Creates densely sampled templates for each shape in both forward and reverse directions.
    /// </summary>
    private void CreateTemplates()
    {
        float r = TemplateSize * 0.4f;
        Vector2 c = new Vector2(TemplateSize / 2, TemplateSize / 2);
        Func<List<Vector2>, List<Vector2>> proc = pts => ProcessPoints(pts);

        // Helper: linear interpolation between two points
        List<Vector2> Line(Vector2 a, Vector2 b, int count)
        {
            var pts = new List<Vector2>(count + 1);
            for (int i = 0; i <= count; i++)
                pts.Add(Vector2.Lerp(a, b, i / (float)count));
            return pts;
        }

        var rawTemplates = new Dictionary<string, List<Vector2>>
        {
            {"Triangle", new List<Vector2>()},
            {"Rectangle", new List<Vector2>()},
            {"Circle", new List<Vector2>()},
            {"Z", new List<Vector2>()},
            {"Spiral", new List<Vector2>()},
            {"Heart", new List<Vector2>()}
        };

        // Build raw
        // Triangle
        var p0 = c + new Vector2(0, -r);
        var p1 = c + new Vector2(-r, r);
        var p2 = c + new Vector2(r, r);
        rawTemplates["Triangle"].AddRange(Line(p0, p1, NumPoints/3));
        rawTemplates["Triangle"].AddRange(Line(p1, p2, NumPoints/3));
        rawTemplates["Triangle"].AddRange(Line(p2, p0, NumPoints/3));

        // Rectangle
        var q0 = c + new Vector2(-r, -r);
        var q1 = c + new Vector2(-r, r);
        var q2 = c + new Vector2(r, r);
        var q3 = c + new Vector2(r, -r);
        rawTemplates["Rectangle"].AddRange(Line(q0, q1, NumPoints/4));
        rawTemplates["Rectangle"].AddRange(Line(q1, q2, NumPoints/4));
        rawTemplates["Rectangle"].AddRange(Line(q2, q3, NumPoints/4));
        rawTemplates["Rectangle"].AddRange(Line(q3, q0, NumPoints/4));

        // Circle
        for (int i = 0; i < NumPoints; i++)
        {
            float theta = 2 * Mathf.PI * i / NumPoints;
            rawTemplates["Circle"].Add(c + new Vector2(Mathf.Cos(theta), Mathf.Sin(theta)) * r);
        }

        // Z
        var z0 = c + new Vector2(-r, r);
        var z1 = c + new Vector2(r, r);
        var z2 = c + new Vector2(-r, -r);
        var z3 = c + new Vector2(r, -r);
        rawTemplates["Z"].AddRange(Line(z0, z1, NumPoints/3));
        rawTemplates["Z"].AddRange(Line(z1, z2, NumPoints/3));
        rawTemplates["Z"].AddRange(Line(z2, z3, NumPoints/3));

        // Spiral
        for (int i = 0; i < NumPoints; i++)
        {
            float frac = i / (float)(NumPoints - 1);
            float a = frac * 3 * 2 * Mathf.PI;
            rawTemplates["Spiral"].Add(c + new Vector2(Mathf.Cos(a), Mathf.Sin(a)) * frac * r);
        }

        // Heart
        for (int i = 0; i < NumPoints; i++)
        {
            float t = 2 * Mathf.PI * i / (NumPoints - 1);
            float x = 16 * Mathf.Pow(Mathf.Sin(t), 3);
            float y = 13 * Mathf.Cos(t) - 5 * Mathf.Cos(2*t) - 2 * Mathf.Cos(3*t) - Mathf.Cos(4*t);
            rawTemplates["Heart"].Add(c + new Vector2(x, y) * (r / 18f));
        }

        // Process and save both directions
        foreach (var kv in rawTemplates)
        {
            var raw = kv.Value;
            var procPts = proc(raw);
            // forward
            recognizer.SavePattern(kv.Key, procPts);
            // reverse
            var rev = proc(Enumerable.Reverse(raw).ToList());
            recognizer.SavePattern(kv.Key, rev);
            Debug.Log($"[GestureRecognizer] '{kv.Key}': raw={raw.Count}, proc={procPts.Count}");
        }
    }

    void Update()
    {
        bool inside = RectTransformUtility.RectangleContainsScreenPoint(drawAreaRect, Input.mousePosition, Camera.main);
        if (Input.GetMouseButtonDown(0) && inside)
        {
            isDrawing = true; points.Clear(); lineRenderer.positionCount = 0;
            Debug.Log("[GestureRecognizer] Start drawing");
        }
        if (Input.GetMouseButton(0) && isDrawing && inside)
        {
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(drawAreaRect, Input.mousePosition, Camera.main, out var lp))
            {
                if (points.Count == 0 || Vector2.Distance(points[^1], lp) > 0.1f)
                {
                    points.Add(lp);
                    lineRenderer.positionCount = points.Count;
                    lineRenderer.SetPosition(points.Count - 1, drawAreaRect.TransformPoint(lp));
                }
            }
        }
        if (Input.GetMouseButtonUp(0) && isDrawing)
        {
            isDrawing = false; lineRenderer.positionCount = 0;
            RecognizeManage recognizeManage = FindFirstObjectByType<RecognizeManage>();
            Debug.Log($"[GestureRecognizer] End drawing: raw pts={points.Count}");
            if (points.Count < 10) 
            { 
                recognizeManage.RecognizeFail();
                Debug.Log("Too few points"); return; 
            }
            var candidate = ProcessPoints(points);
            Debug.Log($"Processed pts={candidate.Count}");
            var res = recognizer.Recognize(candidate);            
            if (res.Match != null) Debug.Log($"Match={res.Match.Name}, Score={res.Score:F2}");
            else Debug.Log("No match");
            if (res.Match != null && res.Score >= ScoreThreshold)
            {
                Debug.Log($"Invoke OnSpellRecognized: {res.Match.Name}");
                OnSpellRecognized.Invoke(res.Match.Name);
                recognizeManage.recognizedText.text = "인식된 도형\n" + (string) res.Match.Name;
                recognizeManage.Recognized();
            }
            else 
            {
                recognizeManage.RecognizeFail();
                Debug.Log($"Failed or low score: {res.Score:F2} < {ScoreThreshold:F2}");
            }
        }
    }
    private List<Vector2> ProcessPoints(List<Vector2> raw)
    {
        Debug.Log($"Raw count: {raw.Count}");
        var pts = Resample(raw, NumPoints); Debug.Log($"Resampled: {pts.Count}");
        float angle = IndicativeAngle(pts); Debug.Log($"Angle: {angle:F2}");
        var rot = RotateBy(pts, -angle); Debug.Log("Rotated");
        var sc = ScaleToSquare(rot, TemplateSize); Debug.Log($"Scaled: {TemplateSize}");
        var tr = TranslateToOrigin(sc); Debug.Log("Translated");
        return tr;
    }

    private List<Vector2> Resample(List<Vector2> pts, int n)
    {
        float I = PathLength(pts)/(n-1), D = 0;
        var newPts = new List<Vector2>{pts[0]};
        for (int i = 1; i < pts.Count; i++)
        {
            float d = Vector2.Distance(pts[i-1], pts[i]);
            if (D + d >= I)
            {
                float qx = pts[i-1].x + (I-D)*(pts[i].x-pts[i-1].x)/d;
                float qy = pts[i-1].y + (I-D)*(pts[i].y-pts[i-1].y)/d;
                var q = new Vector2(qx, qy);
                newPts.Add(q);
                pts.Insert(i, q);
                D = 0;
            }
            else D += d;
        }
        if (newPts.Count < n) newPts.Add(pts[^1]);
        return newPts;
    }

    private float PathLength(List<Vector2> pts) { float d = 0; for(int i=1;i<pts.Count;i++) d += Vector2.Distance(pts[i-1], pts[i]); return d; }
    private float IndicativeAngle(List<Vector2> pts){var c=pts[0];return Mathf.Atan2(pts[1].y-c.y, pts[1].x-c.x);}    
    private List<Vector2> RotateBy(List<Vector2> pts, float angle){
        float cos=Mathf.Cos(angle), sin=Mathf.Sin(angle); var c=Centroid(pts);
        return pts.Select(p=> new Vector2((p.x-c.x)*cos-(p.y-c.y)*sin+c.x, (p.x-c.x)*sin+(p.y-c.y)*cos+c.y)).ToList();
    }
    private Vector2 Centroid(List<Vector2> pts){return new Vector2(pts.Average(p=>p.x), pts.Average(p=>p.y));}
    private List<Vector2> ScaleToSquare(List<Vector2> pts, float size){
        float minX=pts.Min(p=>p.x), minY=pts.Min(p=>p.y), maxX=pts.Max(p=>p.x), maxY=pts.Max(p=>p.y);
        float scale=Math.Max(maxX-minX, maxY-minY);
        return pts.Select(p=>new Vector2((p.x-minX)/scale*size, (p.y-minY)/scale*size)).ToList();
    }
    private List<Vector2> TranslateToOrigin(List<Vector2> pts){var c=Centroid(pts); return pts.Select(p=>p-c).ToList();}
}
