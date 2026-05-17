using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.XR.Management;
using UnityEditor.XR.Management.Metadata;
using UnityEngine;
using UnityEngine.XR.Management;

public static class VRSetup
{
    private const string XRSettingsFolder = "Assets/XR/Settings";

    [MenuItem("Tools/ZooKeeperVR/1. Configure Player Settings")]
    public static void ConfigurePlayerSettings()
    {
        PlayerSettings.colorSpace = ColorSpace.Linear;
        PlayerSettings.companyName = "s19455";
        PlayerSettings.productName = "ZooKeeperVR";

        PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel29;
        PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevelAuto;
        PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;

        PlayerSettings.SetScriptingBackend(NamedBuildTarget.Android, ScriptingImplementation.IL2CPP);
        PlayerSettings.SetApiCompatibilityLevel(NamedBuildTarget.Android, ApiCompatibilityLevel.NET_Standard);

        AssetDatabase.SaveAssets();

        Debug.Log("[VRSetup] PlayerSettings done: " +
            "ColorSpace=" + PlayerSettings.colorSpace +
            " | Company=" + PlayerSettings.companyName +
            " | Product=" + PlayerSettings.productName +
            " | minSDK=" + PlayerSettings.Android.minSdkVersion +
            " | Arch=" + PlayerSettings.Android.targetArchitectures +
            " | Backend(Android)=" + PlayerSettings.GetScriptingBackend(NamedBuildTarget.Android) +
            " | API(Android)=" + PlayerSettings.GetApiCompatibilityLevel(NamedBuildTarget.Android));
    }

    [MenuItem("Tools/ZooKeeperVR/2. Enable OpenXR (PC + Android)")]
    public static void EnableOpenXR()
    {
        EnsureFolder(XRSettingsFolder);
        const string openXrLoader = "UnityEngine.XR.OpenXR.OpenXRLoader";

        var pc = GetOrCreateSettings(BuildTargetGroup.Standalone);
        XRPackageMetadataStore.AssignLoader(pc.AssignedSettings, openXrLoader, BuildTargetGroup.Standalone);

        var android = GetOrCreateSettings(BuildTargetGroup.Android);
        XRPackageMetadataStore.AssignLoader(android.AssignedSettings, openXrLoader, BuildTargetGroup.Android);

        EditorUtility.SetDirty(pc);
        EditorUtility.SetDirty(android);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("[VRSetup] OpenXR loader assigned for Standalone and Android.");
    }

    private static XRGeneralSettings GetOrCreateSettings(BuildTargetGroup group)
    {
        var existing = XRGeneralSettingsPerBuildTarget.XRGeneralSettingsForBuildTarget(group);
        if (existing != null) return existing;

        var assetPath = XRSettingsFolder + "/" + group + "Settings.asset";
        var settings = AssetDatabase.LoadAssetAtPath<XRGeneralSettings>(assetPath);
        if (settings == null)
        {
            settings = ScriptableObject.CreateInstance<XRGeneralSettings>();
            settings.name = group + "Settings";
            AssetDatabase.CreateAsset(settings, assetPath);

            var manager = ScriptableObject.CreateInstance<XRManagerSettings>();
            manager.name = group + "Manager";
            AssetDatabase.AddObjectToAsset(manager, settings);
            settings.Manager = manager;

            AssetDatabase.SaveAssets();
        }

        EditorBuildSettings.AddConfigObject("com.unity.xr.management." + group, settings, true);
        return settings;
    }

[MenuItem("Tools/ZooKeeperVR/9. Configure OpenXR Features")]
    public static void ConfigureOpenXRFeatures()
    {
        ConfigureForGroup(BuildTargetGroup.Standalone);
        ConfigureForGroup(BuildTargetGroup.Android);
        AssetDatabase.SaveAssets();
        Debug.Log("[VRSetup] OpenXR features configured for Standalone and Android.");
    }

    private static void ConfigureForGroup(BuildTargetGroup group)
    {
        var openXrSettings = UnityEngine.XR.OpenXR.OpenXRSettings.GetSettingsForBuildTargetGroup(group);
        if (openXrSettings == null)
        {
            Debug.LogWarning("[VRSetup] OpenXRSettings null for " + group);
            return;
        }

        openXrSettings.renderMode = UnityEngine.XR.OpenXR.OpenXRSettings.RenderMode.SinglePassInstanced;

        // Enable Meta Quest Touch Controller Profile
        EnableFeatureByName(openXrSettings, "UnityEngine.XR.OpenXR.Features.Interactions.MetaQuestTouchProControllerProfile");
        EnableFeatureByName(openXrSettings, "UnityEngine.XR.OpenXR.Features.Interactions.OculusTouchControllerProfile");

        // For Android only: Meta Quest Support
        if (group == BuildTargetGroup.Android)
            EnableFeatureByName(openXrSettings, "UnityEngine.XR.OpenXR.Features.MetaQuestSupport.MetaQuestFeature");

        EditorUtility.SetDirty(openXrSettings);
        Debug.Log("[VRSetup] " + group + ": renderMode=" + openXrSettings.renderMode);
    }

    private static void EnableFeatureByName(UnityEngine.XR.OpenXR.OpenXRSettings settings, string typeName)
    {
        var features = settings.GetFeatures();
        foreach (var f in features)
        {
            if (f == null) continue;
            if (f.GetType().FullName == typeName)
            {
                f.enabled = true;
                Debug.Log("[VRSetup] Enabled feature: " + typeName);
                return;
            }
        }
        Debug.LogWarning("[VRSetup] Feature not found: " + typeName);
    }

[MenuItem("Tools/ZooKeeperVR/10. Disable OpenXR for Standalone (PC dev)")]
    public static void DisableOpenXRForStandalone()
    {
        const string openXrLoader = "UnityEngine.XR.OpenXR.OpenXRLoader";
        var pc = XRGeneralSettingsPerBuildTarget.XRGeneralSettingsForBuildTarget(BuildTargetGroup.Standalone);
        if (pc != null && pc.AssignedSettings != null)
        {
            XRPackageMetadataStore.RemoveLoader(pc.AssignedSettings, openXrLoader, BuildTargetGroup.Standalone);
            EditorUtility.SetDirty(pc);
            AssetDatabase.SaveAssets();
            Debug.Log("[VRSetup] OpenXR loader DISABLED for Standalone (PC). XR Device Simulator works without OpenXR runtime now.");
        }
        else
        {
            Debug.LogWarning("[VRSetup] Standalone XR settings not found.");
        }
    }

    [MenuItem("Tools/ZooKeeperVR/11. Re-enable OpenXR for Standalone")]
    public static void ReEnableOpenXRForStandalone()
    {
        const string openXrLoader = "UnityEngine.XR.OpenXR.OpenXRLoader";
        var pc = GetOrCreateSettings(BuildTargetGroup.Standalone);
        XRPackageMetadataStore.AssignLoader(pc.AssignedSettings, openXrLoader, BuildTargetGroup.Standalone);
        EditorUtility.SetDirty(pc);
        AssetDatabase.SaveAssets();
        Debug.Log("[VRSetup] OpenXR loader RE-ENABLED for Standalone.");
    }



    private static void EnsureFolder(string folder)
    {
        if (AssetDatabase.IsValidFolder(folder)) return;
        var parts = folder.Split('/');
        var current = parts[0];
        for (int i = 1; i < parts.Length; i++)
        {
            var next = current + "/" + parts[i];
            if (!AssetDatabase.IsValidFolder(next))
                AssetDatabase.CreateFolder(current, parts[i]);
            current = next;
        }
    }
}
