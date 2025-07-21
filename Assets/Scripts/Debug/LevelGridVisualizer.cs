using UnityEngine;

#if UNITY_EDITOR
[ExecuteAlways]
#endif
public class GridVisualizer : MonoBehaviour
{
    public Vector2Int gridSize = new(10, 5); // How many screens wide/tall
    public ScreenDimensions screenDimensions;
    public Vector2 worldOrigin = new(-32f, -18f); // Bottom-left corner of (0,0)

    public Color lineColor = Color.green;
    public bool drawLabels = true;

    private void OnDrawGizmos()
    {
        if (screenDimensions == null) return;

        Gizmos.color = lineColor;

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Vector2 pos = worldOrigin + new Vector2(x * screenDimensions.horizontalSize, y * screenDimensions.verticalSize);
                Vector3 center = new(pos.x + screenDimensions.horizontalSize / 2, pos.y + screenDimensions.verticalSize / 2);
                Vector3 size = new(screenDimensions.horizontalSize, screenDimensions.verticalSize);

                // Draw screen box
                Gizmos.DrawWireCube(center, size);

#if UNITY_EDITOR
                if (drawLabels)
                {
                    UnityEditor.Handles.Label(center, $"({x},{y})");
                }
#endif
            }
        }
    }
}
