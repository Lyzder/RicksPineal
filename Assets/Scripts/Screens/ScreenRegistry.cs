using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ScreenRegistry : MonoBehaviour
{

    public List<ScreenData> screens;
    public Dictionary<Vector2Int, ScreenData> screensDict;

    // Start is called before the first frame update
    void Start()
    {
        screensDict = new Dictionary<Vector2Int, ScreenData>();

        foreach (var screen in screens)
        {
            screensDict[screen.gridPosition] = screen;
        }
    }
}
