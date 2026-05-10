using UnityEditor;
using UnityEditor.SceneManagement;

/// Play 버튼을 누르면 항상 MainMenu 씬에서 시작하도록 강제한다.
[InitializeOnLoad]
public static class PlayModeStarter
{
    static PlayModeStarter()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state != PlayModeStateChange.ExitingEditMode) return;
        var mainMenu = AssetDatabase.LoadAssetAtPath<SceneAsset>("Assets/Scenes/MainMenu.unity");
        EditorSceneManager.playModeStartScene = mainMenu;
    }
}
