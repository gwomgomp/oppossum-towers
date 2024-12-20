using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;

public class Scatterer : MonoBehaviour {
    public GameObject[] elements;
    public int limit;
    public float distance;
    public float width;
    public float height;
    public int layerCount;
    public float layerSpacing;

    List<List<Vector3>> pointLayers = new();
    bool elementsRendered = false;

    void OnValidate() {
        elementsRendered = false;
        pointLayers = new List<List<Vector3>>();
        for (int layer = 0; layer < layerCount; layer++) {
            var points = BuildPoints(layer);
            pointLayers.Add(points);
        }
    }

    void Update() {
        if (!elementsRendered && elements.Length > 0) {
            InstantiateElements();
        }
    }

    private List<Vector3> BuildPoints(int layer) {
        List<Vector3> newPoints = new();
        List<Vector2> active = new();

        var cellSize = Mathf.Max(Mathf.Floor(distance / Mathf.Sqrt(2)), 1);

        var cellCountWidth = Mathf.CeilToInt(width / cellSize) + 1;
        var cellCountHeight = Mathf.CeilToInt(height / cellSize) + 1;
        var cells = new Vector2[cellCountWidth, cellCountHeight];

        var initialPoint = new Vector2(Random.Range(0, width), Random.Range(0, height));
        InsertPoint(cells, initialPoint, cellSize);
        newPoints.Add(TranslatePoint(initialPoint, layer));
        active.Add(initialPoint);

        while (active.Count > 0) {
            var point = active[Mathf.FloorToInt(Random.Range(0, active.Count))];
            (bool success, Vector2 newPoint) = GeneratePoint(cellSize, cellCountWidth, cellCountHeight, cells, point);

            if (success) {
                InsertPoint(cells, newPoint, cellSize);
                newPoints.Add(TranslatePoint(newPoint, layer));
                active.Add(newPoint);
            } else {
                active.Remove(point);
            }
        }

        return newPoints;
    }

    private Vector3 TranslatePoint(Vector2 point, int layer) {
        return new Vector3(point.x - (width / 2), point.y - (height / 2), layer * layerSpacing) + transform.position;
    }

    private void InstantiateElements() {
        while (transform.childCount > 0) {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }

        var availableElements = new List<GameObject>(elements);
        foreach (var layer in pointLayers) {
            foreach (var point in layer) {
                var index = Mathf.FloorToInt(Random.Range(0, availableElements.Count));
                GameObject element = availableElements[index];
                if (element != null) {
                    availableElements.RemoveAt(index);
                    var instance = Instantiate(element, point, Random.rotation, transform);
                    instance.layer = gameObject.layer;

                    if (availableElements.Count == 0) {
                        availableElements = new List<GameObject>(elements);
                    }
                }
            }
        }

        elementsRendered = true;
    }

    private (bool, Vector2) GeneratePoint(float cellSize,
                                          int cellCountWidth,
                                          int cellCountHeight,
                                          Vector2[,] cells,
                                          Vector2 point) {
        for (int tries = 0; tries < limit; tries++) {
            var angle = Random.Range(0, 360);
            var newRadius = Random.Range(distance, 2 * distance);
            var newX = point.x + newRadius * Mathf.Cos(Mathf.Deg2Rad * angle);
            var newY = point.y + newRadius * Mathf.Sin(Mathf.Deg2Rad * angle);
            var newPoint = new Vector2(newX, newY);

            if (IsValidPoint(cells, cellSize, cellCountWidth, cellCountHeight, newPoint)) {
                return (true, newPoint);
            }
        }

        return (false, Vector2.zero);
    }

    private bool IsValidPoint(Vector2[,] cells,
                      float cellSize,
                      int cellCountWidth,
                      int cellCountHeight,
                      Vector2 newPoint) {
        if (newPoint.x < 0 || newPoint.x >= width || newPoint.y < 0 || newPoint.y >= height) {
            return false;
        }

        int xIndex = Mathf.FloorToInt(newPoint.x / cellSize);
        int yIndex = Mathf.FloorToInt(newPoint.y / cellSize);
        int lowerX = Mathf.Max(xIndex - 1, 0);
        int upperX = Mathf.Min(xIndex + 1, cellCountWidth - 1);
        int lowerY = Mathf.Max(yIndex - 1, 0);
        int upperY = Mathf.Min(yIndex + 1, cellCountHeight - 1);

        for (int x = lowerX; x <= upperX; x++) {
            for (int y = lowerY; y <= upperY; y++) {
                if (cells[x, y] == null || Vector2.Distance(newPoint, cells[x, y]) < distance) {
                    return false;
                }
            }
        }

        return true;
    }

    private void InsertPoint(Vector2[,] cells, Vector2 point, float cellSize) {
        var xIndex = Mathf.FloorToInt(point.x / cellSize);
        var yIndex = Mathf.FloorToInt(point.y / cellSize);
        cells[xIndex, yIndex] = point;
    }
}
