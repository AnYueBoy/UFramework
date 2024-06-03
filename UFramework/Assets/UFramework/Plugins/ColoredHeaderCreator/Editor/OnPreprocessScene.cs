using UnityEditor;
using UnityEditor.Callbacks;

public static class OnPreprocessScene
{
    [PostProcessScene]
    private static void OnPostProcessScene()
    {
        if (EditorApplication.isPlaying)
        {
            return;
        }

        EditorHelper.DeleteAllHeaders();
    }
}