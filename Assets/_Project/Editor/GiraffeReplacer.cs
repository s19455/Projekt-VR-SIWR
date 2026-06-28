using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

public static class GiraffeReplacer
{
    private const string RIGGED_GLB = "Assets/Assets_Pobrane/AI_Generated/Animals/girraffe_rigged.glb";
    private const string CONTROLLER_PATH = "Assets/_Project/Animations/GiraffeAnimator.controller";

    [MenuItem("Tools/ZooKeeperVR/Giraffe/1. Create Animator Controller")]
    public static void CreateAnimatorController()
    {
        AnimationClip bendClip = null;
        foreach (var a in AssetDatabase.LoadAllAssetsAtPath(RIGGED_GLB))
        {
            if (a is AnimationClip c && c.name == "GiraffeBend") { bendClip = c; break; }
        }
        if (bendClip == null)
        {
            Debug.LogError("[GiraffeReplacer] Clip 'GiraffeBend' not found in " + RIGGED_GLB);
            return;
        }

        // ensure folder
        if (!AssetDatabase.IsValidFolder("Assets/_Project/Animations"))
            AssetDatabase.CreateFolder("Assets/_Project", "Animations");

        // delete old controller if exists
        if (AssetDatabase.LoadAssetAtPath<AnimatorController>(CONTROLLER_PATH) != null)
            AssetDatabase.DeleteAsset(CONTROLLER_PATH);

        var controller = AnimatorController.CreateAnimatorControllerAtPath(CONTROLLER_PATH);
        controller.AddParameter("Bend", AnimatorControllerParameterType.Trigger);

        var sm = controller.layers[0].stateMachine;
        var idle = sm.AddState("Idle");
        var bend = sm.AddState("Bend");
        bend.motion = bendClip;

        var i2b = idle.AddTransition(bend);
        i2b.AddCondition(AnimatorConditionMode.If, 0, "Bend");
        i2b.hasExitTime = false;
        i2b.duration = 0.1f;

        var b2i = bend.AddTransition(idle);
        b2i.hasExitTime = true;
        b2i.exitTime = 0.95f;
        b2i.duration = 0.2f;

        sm.defaultState = idle;
        AssetDatabase.SaveAssets();
        Debug.Log("[GiraffeReplacer] Controller created: " + CONTROLLER_PATH);
    }

    [MenuItem("Tools/ZooKeeperVR/Giraffe/2. Replace 3 Giraffes")]
    public static void ReplaceGiraffes()
    {
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(RIGGED_GLB);
        var controller = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(CONTROLLER_PATH);
        if (prefab == null) { Debug.LogError("[GiraffeReplacer] Prefab not found: " + RIGGED_GLB); return; }
        if (controller == null) { Debug.LogError("[GiraffeReplacer] Controller not found: " + CONTROLLER_PATH); return; }

        var giraffesParent = GameObject.Find("Giraffes");
        if (giraffesParent == null) { Debug.LogError("[GiraffeReplacer] Giraffes parent not in scene"); return; }

        // Stare children — usuń wszystko pod Giraffes
        var toDelete = new System.Collections.Generic.List<GameObject>();
        foreach (Transform child in giraffesParent.transform) toDelete.Add(child.gameObject);
        foreach (var go in toDelete) Object.DestroyImmediate(go);
        Debug.Log("[GiraffeReplacer] Deleted " + toDelete.Count + " old children");

        var configs = new[] {
            new { Id = "giraffe_01", Pos = new Vector3(2.68f,  0, 16.21f), Rot = new Vector3(0, 270, 0), Tr = new Vector3(2.68f,  4.5f, 15.0f) },
            new { Id = "giraffe_02", Pos = new Vector3(-3.39f, 0, 16.92f), Rot = new Vector3(0, 250, 0), Tr = new Vector3(-3.39f, 4.5f, 16.0f) },
            new { Id = "giraffe_03", Pos = new Vector3(-2.71f, 0, 11.96f), Rot = new Vector3(0, 290, 0), Tr = new Vector3(-2.71f, 4.5f, 11.0f) },
        };

        int i = 0;
        foreach (var cfg in configs)
        {
            i++;
            var inst = (GameObject)PrefabUtility.InstantiatePrefab(prefab, giraffesParent.transform);
            inst.name = "Giraffe_0" + i;
            inst.transform.position = cfg.Pos;
            inst.transform.eulerAngles = cfg.Rot;
            inst.transform.localScale = new Vector3(0.025f, 0.025f, 0.025f);

            var anim = inst.GetComponent<Animator>();
            if (anim == null) anim = inst.AddComponent<Animator>();
            anim.runtimeAnimatorController = controller;

            // HeadTrigger jako child Giraffes (osobne — żeby działało z animacją mesh deformacji)
            var trigger = new GameObject("HeadTrigger_0" + i);
            trigger.transform.SetParent(giraffesParent.transform);
            trigger.transform.position = cfg.Tr;
            var sc = trigger.AddComponent<SphereCollider>();
            sc.isTrigger = true;
            sc.radius = 1.5f;
            var feed = trigger.AddComponent<GiraffeFeed>();

            var so = new SerializedObject(feed);
            so.FindProperty("giraffeId").stringValue = cfg.Id;
            so.FindProperty("giraffeAnimator").objectReferenceValue = anim;
            so.ApplyModifiedProperties();

            Debug.Log("[GiraffeReplacer] " + inst.name + " @ " + cfg.Pos + " | trigger @ " + cfg.Tr);
        }

        EditorUtility.SetDirty(giraffesParent);
        EditorSceneManagerCompat.MarkSceneDirty();
        Debug.Log("[GiraffeReplacer] Replacement done. Save scene (Ctrl+S).");
    }
}

static class EditorSceneManagerCompat
{
    public static void MarkSceneDirty()
    {
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
            UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
    }
}
