using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UILineDrawer : MonoBehaviour
{
    public RectTransform canvasRect;
    public GameObject lineSegmentPrefab;

    private readonly List<GameObject> segments = new();

    public void DrawLine(Vector2 localStart, Vector2 localEnd, Color color)
    {
        GameObject line = Instantiate(lineSegmentPrefab, canvasRect);
        RectTransform rt = line.GetComponent<RectTransform>();
        Image img = line.GetComponent<Image>();
        img.color = color;
        segments.Add(line);

        Vector2 direction = localEnd - localStart;
        float length = direction.magnitude;

        rt.pivot = new Vector2(0, 0.5f);
        rt.sizeDelta = new Vector2(length, 2); // Width = line length, height = thickness
        rt.localPosition = localStart;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        rt.localRotation = Quaternion.Euler(0, 0, angle);
    }

    public void ClearLines()
    {
        foreach (var obj in segments)
        {
            Destroy(obj);
        }
        segments.Clear();
    }
}
