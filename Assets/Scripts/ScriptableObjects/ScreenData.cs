using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScreenData", menuName = "Level/Screen Data")]
public class ScreenData : ScriptableObject
{
    public Vector2Int gridPosition;

    [Header("Relations")]
    public ScreenData upScreen;
    public ScreenData downScreen;
    public ScreenData leftScreen;
    public ScreenData rightScreen;

    [Header("Screen data")]
    public Vector3 cameraPosition;
    public bool hasRespawn;
    public Vector2 respawnPosition;
}
