using UnityEditor;
using UnityEditor.SceneManagement;

public static class SceneSwitcher
{
    const string LOCKER = "Assets/_Project/Scenes/01_Locker_Room.unity";
    const string SAVANNA = "Assets/_Project/Scenes/02_Savanna.unity";

    [MenuItem("Tools/ZooKeeperVR/Open Locker Room _F1", priority = 100)]
    public static void OpenLockerRoom() => OpenScene(LOCKER);

    [MenuItem("Tools/ZooKeeperVR/Open Savanna _F2", priority = 101)]
    public static void OpenSavanna() => OpenScene(SAVANNA);

    [MenuItem("Tools/ZooKeeperVR/Toggle Scene %#s", priority = 102)]
    public static void ToggleScene()
    {
        string current = EditorSceneManager.GetActiveScene().path;
        OpenScene(current == LOCKER ? SAVANNA : LOCKER);
    }

    static void OpenScene(string path)
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            EditorSceneManager.OpenScene(path);
    }
}
