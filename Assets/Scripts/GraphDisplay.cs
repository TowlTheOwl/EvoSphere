using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Numerics;
using Vector2 = UnityEngine.Vector2;
using System.Linq;
using UnityEditor;
using Unity.VisualScripting;

public class OverlayCanvasGraph : MonoBehaviour
{
    public RectTransform graphContainer;  // The container where the lines will be drawn
    public UILineDrawer lineDrawer;         // Prefab for the line (Image component)

    public Vector2 bottomLeft = new();
    public Vector2 GraphSize = new();
    public float maxValue = 500f;
    
    private List<(int, int, int, int)> data;
    private int previous;
    void Awake() {
        data = GameManager.data;
        previous = 0;
    }

    void Update() {
        if (gameObject.activeSelf) {
            lineDrawer.DrawLine(bottomLeft, new Vector2(bottomLeft.x, bottomLeft.y+GraphSize.y), Color.black);
            lineDrawer.DrawLine(new Vector2(bottomLeft.x, bottomLeft.y+GraphSize.y), new Vector2(bottomLeft.x+GraphSize.x, bottomLeft.y+GraphSize.y), Color.black);
            lineDrawer.DrawLine(new Vector2(bottomLeft.x+GraphSize.x, bottomLeft.y+GraphSize.y), new Vector2(bottomLeft.x+GraphSize.x, bottomLeft.y), Color.black);
            lineDrawer.DrawLine(new Vector2(bottomLeft.x+GraphSize.x, bottomLeft.y), bottomLeft, Color.black);
            // Draw Herbivore Graph
            if (data.Count != previous) {
                previous = data.Count;
                DrawGraph(data, Color.green, Color.red);
            }
        }
    }

    // Draw the graph using a list of data points (Vector2: x = time, y = value)
    public void DrawGraph(List<(int, int, int, int)> dataPoints, Color herbivoreColor, Color carnivoreColor)
    {
        int len = dataPoints.Count;
        lineDrawer.ClearLines();  // Clear any previous lines

        if (len > 1) {
            Vector2 prevHerb = new(bottomLeft.x, bottomLeft.y+dataPoints[0].Item3/maxValue);
            Vector2 prevCarn = new(bottomLeft.x, bottomLeft.y+dataPoints[0].Item4/maxValue);
            // Draw a line between each adjacent data point
            for (int i = 1; i < dataPoints.Count; i++)
            {
                // x = i * GraphSize.x / (len - 1)
                // len = 2 -> 0, Graphsize.x
                // len = 3 -> 0, Graphsize.x/2, Graphsize.x
                Vector2 newPoint = new(bottomLeft.x + i * GraphSize.x / (len-1), bottomLeft.y+dataPoints[i].Item3);
                lineDrawer.DrawLine(prevHerb, newPoint, herbivoreColor);
                prevHerb = newPoint;
                
                newPoint = new(bottomLeft.x + i * GraphSize.x / (len-1), bottomLeft.y+dataPoints[i].Item4);
                lineDrawer.DrawLine(prevCarn, newPoint, carnivoreColor);
                prevCarn = newPoint;
            }
        }
    }
}
