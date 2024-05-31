using UnityEditor;
using UnityEngine;

public class ColoredHeader : MonoBehaviour
{
#if UNITY_EDITOR
    public HeaderSettings headerSettings = new HeaderSettings();

    private void OnValidate()
    {
        EditorApplication.delayCall += OnValidateInner;
    }

    private void OnValidateInner()
    {
        if (this == null)
        {
            return;
        }

        EditorApplication.RepaintAnimationWindow();
    }
#endif
}