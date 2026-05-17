using System.Linq;
using UnityEditor;
using UnityEditor.PackageManager.UI;
using UnityEngine;

public static class VRSamplesImport
{
    [MenuItem("Tools/ZooKeeperVR/3. Import XR Toolkit Starter Assets")]
    public static void ImportStarterAssets()
    {
        var samples = Sample.FindByPackage("com.unity.xr.interaction.toolkit", null);
        if (samples == null)
        {
            Debug.LogError("[VRSetup] Could not find samples for com.unity.xr.interaction.toolkit.");
            return;
        }

        int imported = 0;
        foreach (var s in samples)
        {
            if (s.displayName == "Starter Assets" || s.displayName == "XR Device Simulator" || s.displayName == "Hands Interaction Demo")
            {
                bool ok = s.Import(Sample.ImportOptions.OverridePreviousImports);
                Debug.Log("[VRSetup] Import '" + s.displayName + "' -> " + (ok ? "OK" : "FAIL"));
                if (ok) imported++;
            }
        }

        AssetDatabase.Refresh();
        Debug.Log("[VRSetup] Imported " + imported + " samples from XR Interaction Toolkit.");
    }
}
