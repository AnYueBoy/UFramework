using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class MiscOdinTest : MonoBehaviour
{
    #region CustomContextMenu

    [CustomContextMenu("Say Hello/Twice", "SayHello")]
    public int myProperty;

    private void SayHello()
    {
        Debug.Log("Hello Twice");
    }

    #endregion

    #region Disable Content Menum

    [DisableContextMenu] public int[] noRightClickList = new int[] { 2, 3, 5 };

    #endregion

    #region Draw With Unity

    [DrawWithUnity] public GameObject objectDrawnWithUnity;

    #endregion

    #region Inline Property

    public Vector3 vector3;

    [InlineProperty] public Vector3 otherVector3;

    #endregion
}