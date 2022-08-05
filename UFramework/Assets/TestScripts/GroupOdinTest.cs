using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class GroupOdinTest : MonoBehaviour
{
    #region Box Group

    [BoxGroup("Some Title")] public string a;
    [BoxGroup("Some Title")] public string b;

    #endregion

    #region Button

    public string buttonName = "Dynamic button name";
    public bool toggle;

    [Button("$buttonName")]
    private void DefaultSizeButton()
    {
        toggle = !toggle;
    }

    [ShowIf("toggle")] public string text;

    #endregion

    #region Button Group

    [ButtonGroup("A")]
    private void PrintA()
    {
    }

    [GUIColor(0.5f, 1, 1, 1)]
    [ButtonGroup("A")]
    private void PrintB()
    {
    }

    #endregion

    #region Enum Paging

    [EnumPaging] public SomeEnum someEnumFiled;

    public enum SomeEnum
    {
        A,
        B,
        C
    }

    #endregion

    #region  InlineButton

    [InlineButton("A")] public int inlineButton;

    private void A()
    {
        
    }

    #endregion
}