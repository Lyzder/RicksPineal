using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenTracker : MonoBehaviour
{
    public ScreenData currentScreen;
    public ScreenDimensions screenDimensions;
    private ScreenRegistry screenRegistry;

    // Update is called once per frame
    void Update()
    {
        if (screenRegistry == null)
        {
            screenRegistry = GameObject.FindGameObjectWithTag("LevelScreens").GetComponent<ScreenRegistry>();
        }
        if (screenRegistry != null)
        {
            CheckGridPosition();
        }
    }

    private void CheckGridPosition()
    {
        Vector2 playerPos = transform.position;
        Vector2 screenOrigin = currentScreen.gridPosition;

        // Convert world position to grid position
        Vector2Int newGridPos = new Vector2Int(
            Mathf.FloorToInt(playerPos.x / screenDimensions.horizontalSize),
            Mathf.FloorToInt(playerPos.y / screenDimensions.verticalSize)
        );

        if (newGridPos != currentScreen.gridPosition)
        {
            ScreenData nextScreen = GetScreenByGridPos(newGridPos);
            if (nextScreen != null)
            {
                ScreenTransitionManager.Instance.MoveToScreen(nextScreen);
                currentScreen = nextScreen;
            }
        }
    }

    private ScreenData GetScreenByGridPos(Vector2Int pos)
    {
        // This could be replaced with a dictionary for faster lookup
        ScreenData screen;
        if (screenRegistry.screensDict.TryGetValue(pos, out screen))
        {
            return screen;
        }
        else
        {
            return null;
        }
    }
}
