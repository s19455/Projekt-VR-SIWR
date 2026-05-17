using UnityEditor;
using UnityEngine;

public static class ProjectStructureSetup
{
    private static readonly string[] FoldersToCreate = new[]
    {
        "Assets/_Project",
        "Assets/_Project/Scenes",
        "Assets/_Project/Scripts",
        "Assets/_Project/Editor",
        "Assets/_Project/Prefabs",
        "Assets/_Project/Materials",
        "Assets/_Project/Textures",
        "Assets/_Project/Animations",
        "Assets/_Project/Audio",
        "Assets/_Project/Audio/Ambient",
        "Assets/_Project/Audio/SFX",
        "Assets/_Project/Audio/Voice",
        "Assets/_Project/Audio/Music",
        "Assets/Assets_Pobrane",
        "Assets/Assets_Pobrane/Mixamo",
        "Assets/Assets_Pobrane/Sketchfab",
        "Assets/Assets_Pobrane/Asset_Store",
        "Assets/Assets_Pobrane/AI_Generated",
    };

    [MenuItem("Tools/ZooKeeperVR/4. Create Project Folders")]
    public static void CreateFolders()
    {
        int created = 0, existed = 0;
        foreach (var path in FoldersToCreate)
        {
            if (AssetDatabase.IsValidFolder(path)) { existed++; continue; }
            var idx = path.LastIndexOf('/');
            var parent = path.Substring(0, idx);
            var name = path.Substring(idx + 1);
            if (!AssetDatabase.IsValidFolder(parent))
            {
                Debug.LogError("[Struct] Parent folder missing: " + parent);
                continue;
            }
            AssetDatabase.CreateFolder(parent, name);
            created++;
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("[Struct] Folders created: " + created + ", already existed: " + existed);
    }

    [MenuItem("Tools/ZooKeeperVR/5. Reorganize Files")]
    public static void ReorganizeFiles()
    {
        Move("Assets/Editor/VRSetup.cs", "Assets/_Project/Editor/VRSetup.cs");
        Move("Assets/Editor/VRSamplesImport.cs", "Assets/_Project/Editor/VRSamplesImport.cs");
        Move("Assets/Editor/ProjectStructureSetup.cs", "Assets/_Project/Editor/ProjectStructureSetup.cs");
        Move("Assets/Scenes/SampleScene.unity", "Assets/_Project/Scenes/01_Locker_Room.unity");
        Move("Assets/Assety 3D/ComfyUI_00010_.glb", "Assets/Assets_Pobrane/AI_Generated/Test_Hunyuan_Output.glb");

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("[Struct] Reorganization done.");
    }

    private static void Move(string from, string to)
    {
        if (!AssetDatabase.LoadAssetAtPath<Object>(from))
        {
            Debug.LogWarning("[Struct] Skip move (not found): " + from);
            return;
        }
        var err = AssetDatabase.MoveAsset(from, to);
        if (!string.IsNullOrEmpty(err))
            Debug.LogError("[Struct] Move failed: " + from + " -> " + to + " : " + err);
        else
            Debug.Log("[Struct] Moved: " + from + " -> " + to);
    }

[MenuItem("Tools/ZooKeeperVR/6. Cleanup Approved Folders")]
    public static void CleanupApproved()
    {
        // User-approved deletions (consent given in chat)
        DeleteIfExists("Assets/Editor");
        DeleteIfExists("Assets/Scenes");
        DeleteIfExists("Assets/Assety 3D");
        DeleteIfExists("Assets/TutorialInfo");
        DeleteIfExists("Assets/XR 1");
        DeleteIfExists("Assets/XR 2");
        DeleteIfExists("Assets/XR/Settings 1");
        DeleteIfExists("Assets/XR/Settings 2");

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("[Struct] Cleanup done.");
    }

    private static void DeleteIfExists(string path)
    {
        if (AssetDatabase.IsValidFolder(path) || AssetDatabase.LoadAssetAtPath<Object>(path))
        {
            if (AssetDatabase.DeleteAsset(path))
                Debug.Log("[Struct] Deleted: " + path);
            else
                Debug.LogWarning("[Struct] Delete failed: " + path);
        }
        else
        {
            Debug.Log("[Struct] Skip (not found): " + path);
        }
    }

[MenuItem("Tools/ZooKeeperVR/7. Cleanup Root Duplicates")]
    public static void CleanupRootDuplicates()
    {
        // Old GLB and its auto-generated PNG thumbnails (we keep the moved copy in Assets_Pobrane/AI_Generated/)
        DeleteIfExists("Assets/ComfyUI_00010_.glb");
        DeleteIfExists("Assets/ComfyUI_00010_.glb.png");
        DeleteIfExists("Assets/ComfyUI_00010_.glb 1.png");
        DeleteIfExists("Assets/ComfyUI_00010_.glb 2.png");
        // URP template leftover
        DeleteIfExists("Assets/Readme.asset");

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("[Struct] Root duplicates cleanup done.");
    }

[MenuItem("Tools/ZooKeeperVR/8. Final Cleanup")]
    public static void FinalCleanup()
    {
        DeleteIfExists("Assets/Readme.asset");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("[Struct] Final cleanup done.");
    }



}
