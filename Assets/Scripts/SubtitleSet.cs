using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "SubtitleSet", menuName = "Scriptable Objects/SubtitleSet")]
public class SubtitleSet : ScriptableObject
{
    public List<SubtitleData> subtitles;

    public SubtitleData GetSubtitle(string id)
    {
        return subtitles.Find(s => s.id == id);
    }
}
