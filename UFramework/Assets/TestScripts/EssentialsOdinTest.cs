using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor.Examples;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

public class EssentialsOdinTest : MonoBehaviour
{
    [Title("仅资源")] [AssetsOnly] public List<GameObject> onlyAssets;
    [Title("场景资源")] [SceneObjectsOnly] public List<GameObject> onlyScenesObjects;

    #region Custom Staice Slier

    [CustomValueDrawer("DrawStatic")] public float customDrawerStatic;

    private static float DrawStatic(float value, GUIContent label)
    {
        return EditorGUILayout.Slider(label, value, 0f, 10f);
    }

    #endregion

    #region Custom Instance Slier

    public float from = 2, to = 10;

    [CustomValueDrawer("DrawInstance")] public float customDrawerInstance;

    private float DrawInstance(float value, GUIContent label)
    {
        return EditorGUILayout.Slider(label, value, from, to);
    }

    #endregion

    #region Custom Append Slier

    [CustomValueDrawer("DrawAppendRange")] public float appendRange;

    private float DrawAppendRange(float value, GUIContent label, Func<GUIContent, bool> callNextDrawer)
    {
        SirenixEditorGUI.BeginBox();
        callNextDrawer(label);
        float result = EditorGUILayout.Slider(value, from, to);
        SirenixEditorGUI.EndBox();
        return result;
    }

    #endregion

    [CustomValueDrawer("DrawArrayNoLabel")]
    public float[] customDrawerArrayNodeLabel = new[] { 3f, 5f, 10f };

    private float DrawArrayNoLabel(float value)
    {
        return EditorGUILayout.Slider(value, from, to);
    }

    #region Delay Arrribute

    [DelayedProperty, ShowInInspector, OnValueChanged("ValueChanged")]
    private int delayValue;

    private void ValueChanged()
    {
        Debug.Log($"finial value : {delayValue}");
    }

    #endregion

    #region Detail Info Box

    [DetailedInfoBox("Click the detailedInfoBox...", "...to reveal more information!\n" + "This all")]
    public int filed;

    #endregion

    #region Enable GUIAttribute

    [ShowInInspector] public int GUIDisableProperty => 10;
    [ShowInInspector, EnableGUI] public int GUIEnableProperty => 10;

    #endregion

    #region GUI Color

    [GUIColor(0.3f, 0.8f, 0.8f, 1f)] public int colorInt;

    [ButtonGroup, GUIColor(0, 1, 0)]
    private void Apply()
    {
        Debug.Log("Apply");
    }

    [ButtonGroup, GUIColor(1, 0.6f, 0, 4f)]
    private void Cancel()
    {
        Debug.Log("Cancel");
    }

    [InfoBox("You can also reference a color member to dynamically change the color of a property.")]
    [GUIColor("GetButtonColor"), Button("I Am fabulous", ButtonSizes.Small)]
    private void IAMFabulous()
    {
        Debug.Log("IAmFabulous");
    }

    [Button(ButtonSizes.Large),
     GUIColor("@Color.Lerp(Color.red,Color.green,Mathf.Abs(Mathf.Sin((float)EditorApplication.timeSinceStartup)))")]
    private void Expressive()
    {
    }

    private Color GetButtonColor()
    {
        GUIHelper.RequestRepaint();
        return Color.HSVToRGB(Mathf.Cos((float)EditorApplication.timeSinceStartup + 1f) * 0.225f + 0.325f,
            1, 1);
    }

    #endregion

    #region Hide Label

    [Title("Wide Colors"), HideLabel, ColorPalette("Fall")]
    public Color wideColor;

    #endregion

    #region Property Order

    [PropertyOrder(1), ShowInInspector] private int second;

    [PropertyOrder(2), ShowInInspector, InfoBox("Info Box")]
    private int first;

    #endregion

    #region Property Space

    [ShowInInspector, PropertySpace] public string property;

    [ShowInInspector, PropertySpace(SpaceBefore = 0, SpaceAfter = 100)]
    public string otherProperty;

    #endregion

    #region Read Only

    // [ReadOnly, ShowInInspector] private string readyOnlyContent = "This is read only content";

    #endregion

    #region Require

    [Required] public GameObject requireNode;
    [Required("$dynamicMessage")] public GameObject dynamicNode;
    // private string dynamicMessage = "DynamicMessage";

    #endregion

    #region Searchable

    [Searchable] public List<Perk> perks = new List<Perk>()
    {
        new Perk()
        {
            name = "Old Sage",
            effect = new List<Effect>()
            {
                new Effect()
                {
                    skill = Skill.Wisdom, valuel = 2
                },
                new Effect()
                {
                    skill = Skill.Intelligence, valuel = 1
                }
            }
        },
        new Perk()
        {
            name = "Criminal",
            effect = new List<Effect>()
            {
                new Effect()
                {
                    skill = Skill.Dexterity, valuel = 3
                },
                new Effect()
                {
                    skill = Skill.Constitution, valuel = 4,
                }
            }
        },
        new Perk()
        {
            name = "Born",
            effect = new List<Effect>()
            {
                new Effect()
                {
                    skill = Skill.Charisma, valuel = 2
                },
                new Effect()
                {
                    skill = Skill.Wisdom, valuel = 3
                }
            }
        }
    };

    public enum Skill
    {
        Strength,
        Dexterity,
        Constitution,
        Intelligence,
        Wisdom,
        Charisma
    }

    [Serializable]
    public class Effect
    {
        public Skill skill;
        public float valuel;
    }

    [Serializable]
    public class Perk
    {
        public string name;
        public List<Effect> effect;
    }

    #endregion

    #region Validate Input Attribute

    [ValidateInput("MustBeNull", "This field should be null.")]
    public GameObject assetNode;

    private bool MustBeNull(GameObject verifyNode)
    {
        return verifyNode == null;
    }

    #endregion

    #region Value DropDown

    [ValueDropdown("TextureSizes")] public int someSize1;
    private int[] TextureSizes = new[] { 256, 512, 1024 };

    [ValueDropdown("FriendlyTextureSizes")]
    public int someSize2;

    private IEnumerable FriendlyTextureSizes = new ValueDropdownList<int>()
    {
        { "Small", 256 },
        { "Medium", 512 },
        { "Large", 1024 }
    };

    [ValueDropdown("FriendlyTextureSizes", AppendNextDrawer = true, DisableGUIInAppendedDrawer = true)]
    public int someSize3;

    [ValueDropdown("GetListOfMonoBehaviours", AppendNextDrawer = false)]
    public MonoBehaviour someMonoBehaviour;

    private IEnumerable<MonoBehaviour> GetListOfMonoBehaviours()
    {
        return GameObject.FindObjectsOfType<MonoBehaviour>();
    }

    [ValueDropdown("KeyCodes")] public KeyCode FilteredEnum;
    private IEnumerable<KeyCode> KeyCodes = Enumerable.Range((int)KeyCode.Alpha0, 10).Cast<KeyCode>();

    [ValueDropdown("TreeViewOfInts", ExpandAllMenuItems = false)]
    public List<int> IntTreview = new List<int>() { 1, 2, 7 };

    private IEnumerable TreeViewOfInts = new ValueDropdownList<int>()
    {
        { "Node 1/Node 1.1", 1 },
        { "Node 1/Node 1.2", 2 },
        { "Node 2/Node 2.1", 3 },
        { "Node 3/Node 3.1", 4 },
        { "Node 3/Node 3.2", 5 },
        { "Node 1/Node 3.1/Node 3.1.1", 6 },
        { "Node 1/Node 3.1/Node 3.1.2", 7 },
    };

    [ValueDropdown("GetAllSceneObjects", IsUniqueList = true)]
    public List<GameObject> UniqueGameobjectList;

    private IEnumerable GetAllSceneObjects()
    {
        Func<Transform, string> getPath = null;
        getPath = x => (x ? getPath(x.parent) + "/" + x.gameObject.name : "");
        return GameObject.FindObjectsOfType<GameObject>().Select(x => new ValueDropdownItem(getPath(x.transform), x));
    }
    
    [ValueDropdown("GetAllSceneObjects", IsUniqueList = true, DropdownTitle = "Select Scene Object", DrawDropdownForListElements = false, ExcludeExistingValuesInList = true)]
    public List<GameObject> UniqueGameobjectListMode2;

    #endregion
}