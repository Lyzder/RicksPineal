using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SceneMusicMap", menuName = "Audio/Scene Music Map")]
public class SceneMusicMap : ScriptableObject
{

    public List<SceneMusicEntry> musicEntries = new();

    public string GetMusicAddressForScene(string sceneName)
    {
        var entry = musicEntries.Find(e => e.sceneName == sceneName);
        return entry?.musicAddress;
    }

    public SceneMusicEntry GetMusicEntryForScene(string sceneName)
    {
        SceneMusicEntry entry = musicEntries.Find(e => e.sceneName == sceneName);
        return entry ?? null;
    }
}