using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Rooms/Room")]
public class RoomDefinition : ScriptableObject
{
    // [ValueDropdown("GetScenes")]
    public string Scene;
    public WorldContainer World;

    IList GetScenes()
    {
        var scenes = AssetDatabase
            .FindAssets("t:scene")
            .Select(
                guid =>
                    AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GUIDToAssetPath(guid))
                    as SceneAsset
            )
            .Select(scene => scene.name);
        List<string> opts = new();
        opts.AddRange(scenes);
        return opts;
    }
}
