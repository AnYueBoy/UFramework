using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor.Examples;
using UnityEngine;
using Object = UnityEngine.Object;

public class TypeSpecificsOndinTest : MonoBehaviour
{
    #region Assets List

    [AssetList, PreviewField(70, ObjectFieldAlignment.Center)]
    public Texture2D singleObject;

    [AssetList(Path = "/Plugins/Sirenix/")]
    public List<ScriptableObject> assetsList;

    [FoldoutGroup("Filtered Odin ScriptableObjects", expanded: false)] [AssetList(Path = "Plugins/Sirenix/")]
    public ScriptableObject Object;

    [AssetList(AutoPopulate = true, Path = "Plugins/Sirenix/")]
    [FoldoutGroup("Filtered Odin ScriptableObjects", expanded: false)]
    public List<ScriptableObject> AutoPopulatedWhenInspected;

    [AssetList(LayerNames = "Default")] [FoldoutGroup("Filtered AssetLists examples")]
    public GameObject[] AllPrefabsWithLayerName;

    [AssetList(AssetNamePrefix = "Game")] [FoldoutGroup("Filtered AssetLists examples")]
    public List<GameObject> PrefabsStartingWithRock;

    [FoldoutGroup("Filtered AssetLists examples")] [AssetList(Tags = "Player,Finish")]
    public List<GameObject> GameObjectsWithTag;

    [FoldoutGroup("Filtered AssetLists examples")] [AssetList(CustomFilterMethod = "HasRigidbodyComponent")]
    public List<GameObject> MyRigidbodyPrefabs;

    private bool HasRigidbodyComponent(GameObject obj)
    {
        return obj.GetComponent<Rigidbody>() != null;
    }

    #endregion

    #region Asset Selecotr

    [AssetSelector] public Material AnyAllMaterials;

    [AssetSelector] public Material[] ListOfAllMaterials;

    [AssetSelector(FlattenTreeView = true)]
    public GameObject NoTreeView;

    [AssetSelector(Paths = "Assets/MyScriptableObjects")]
    public ScriptableObject ScriptableObjectsFromFolder;

    [AssetSelector(Paths = "Assets/MyScriptableObjects|Assets/Other/MyScriptableObjects")]
    public Material ScriptableObjectsFromMultipleFolders;

    [AssetSelector(Filter = "name t:type l:label")]
    public Object AssetDatabaseSearchFilters;

    [Title("Other Minor Features")] [AssetSelector(DisableListAddButtonBehaviour = true)]
    public List<GameObject> DisableListAddButtonBehaviour;

    [AssetSelector(DrawDropdownForListElements = false)]
    public List<GameObject> DisableListElementBehaviour;

    [AssetSelector(ExcludeExistingValuesInList = false)]
    public List<GameObject> ExcludeExistingValuesInList;

    [AssetSelector(IsUniqueList = false)] public List<GameObject> DisableUniqueListBehaviour;

    [AssetSelector(ExpandAllMenuItems = true)]
    public List<GameObject> ExpandAllMenuItems;

    [AssetSelector(DropdownTitle = "Custom Dropdown Title")]
    public List<GameObject> CustomDropdownTitle;

    #endregion

    #region Child Game Objects Only Attribute

    [ChildGameObjectsOnly] public Transform ChildOrSelfTransform;
    [ChildGameObjectsOnly] public GameObject ChildGameObject;

    [ChildGameObjectsOnly(IncludeSelf = false)]
    public Transform[] Lights;

    #endregion

    #region Color Palette Attribute

    [ColorPalette] public Color ColorOptions;

    [ColorPalette("Clovers")] public Color[] ColorArray;

    [FoldoutGroup("Color Palettes", expanded: false)] [ListDrawerSettings(IsReadOnly = true)] [PropertyOrder(9)]
    public List<ColorPalette> ColorPalettes;

    [Serializable]
    public class ColorPalette
    {
        [HideInInspector] public string Name;

        [LabelText("$Name")] [ListDrawerSettings(IsReadOnly = true, Expanded = false)]
        public Color[] Colors;
    }

    [FoldoutGroup("Color Palettes"), Button(ButtonSizes.Large), GUIColor(0, 1, 0), PropertyOrder(8)]
    private void FetchColorPalettes()
    {
        this.ColorPalettes = Sirenix.OdinInspector.Editor.ColorPaletteManager.Instance.ColorPalettes
            .Select(x => new ColorPalette()
            {
                Name = x.Name,
                Colors = x.Colors.ToArray()
            })
            .ToList();
    }

    #endregion

    #region Display As String Attribute

    [DisplayAsString] public Color SomeColor;

    #endregion

    #region Enum Paging Attribute

    [EnumPaging] public SomeEnum SomeEnumField;

    [EnumPaging, OnValueChanged("SetCurrentTool")]
    public UnityEditor.Tool sceneTool;

    private void SetCurrentTool()
    {
        UnityEditor.Tools.current = this.sceneTool;
    }

    public enum SomeEnum
    {
        A,
        B,
        C
    }

    #endregion

    #region Enum Toggle

    [Title("Default")] public SomeBitmaskEnum DefaultEnumBitmask;

    [Title("Standard Enum")] [EnumToggleButtons]
    public SomeEnum SomeEnumOtherField;

    public enum SomeBitmaskEnum
    {
        A = 1 << 1,
        B = 1 << 2,
        C = 1 << 3,
        All = A | B | C
    }

    #endregion

    #region File Path

    [FilePath] public string unityProjectPath;

    [FilePath(ParentFolder = "Assets/Plugins/Sirenix")]
    public string RelativeToParentPath;

    [FilePath(Extensions = "cs")] [BoxGroup("Conditions")]
    public string ScriptFiles;

    [FilePath(AbsolutePath = true)] [BoxGroup("Conditions")]
    public string AbsolutePath;

    [FilePath(RequireExistingPath = true)] [BoxGroup("Conditions")]
    public string ExistingPath;

    #endregion

    #region Floder Path

    [FolderPath] public string UnityProjectPath;

    #endregion

    #region Hide In Inline Eidtors

    [InlineEditor(Expanded = false)] public MyInlineScriptableObject InlineObject;

    #endregion

    #region Hide In Tables

    public MyItem Item = new MyItem();

    [TableList] public List<MyItem> Table = new List<MyItem>()
    {
        new MyItem(),
        new MyItem(),
        new MyItem(),
    };

    [Serializable]
    public class MyItem
    {
        public string A;

        public int B;

        [HideInTables] public int Hidden;
    }

    #endregion

    #region Hide Mono Script

    public HideMonoScriptScriptableObject Hidden;

    public ShowMonoScriptScriptableObject Shown;

    [OnInspectorInit]
    private void CreateData()
    {
        Hidden = ExampleHelper.GetScriptableObject<HideMonoScriptScriptableObject>("Hidden");
        Shown = ExampleHelper.GetScriptableObject<ShowMonoScriptScriptableObject>("Shown");
    }

    [OnInspectorDispose]
    private void CleanupData()
    {
        if (Hidden != null) DestroyImmediate(Hidden);
        if (Shown != null) DestroyImmediate(Shown);
    }

    #endregion

    #region Multi Line Property

    [Multiline(10)] public string UnityMultilineField = "";
    [MultiLineProperty(10)] public string WideMultilineTextField = "";

    #endregion

    #region Preview Filed

    [PreviewField] public Object regularPreviewFiled;

    #endregion

    #region Scene Objects Only

    [SceneObjectsOnly] public GameObject sceneObject;

    #endregion

    #region Table List

    #endregion

    #region Table Matrix

    [TableMatrix(HorizontalTitle = "Square Celled Matrix", SquareCells = true)]
    public Texture2D[,] SquareCelledMatrix;

    [TableMatrix(SquareCells = true)] public Mesh[,] PrefabMatrix;

    [OnInspectorInit]
    private void CreateDatas()
    {
        SquareCelledMatrix = new Texture2D[8, 4]
        {
            { ExampleHelper.GetTexture(), null, null, null },
            { null, ExampleHelper.GetTexture(), null, null },
            { null, null, ExampleHelper.GetTexture(), null },
            { null, null, null, ExampleHelper.GetTexture() },
            { ExampleHelper.GetTexture(), null, null, null },
            { null, ExampleHelper.GetTexture(), null, null },
            { null, null, ExampleHelper.GetTexture(), null },
            { null, null, null, ExampleHelper.GetTexture() },
        };

        PrefabMatrix = new Mesh[8, 4]
        {
            { ExampleHelper.GetMesh(), null, null, null },
            { null, ExampleHelper.GetMesh(), null, null },
            { null, null, ExampleHelper.GetMesh(), null },
            { null, null, null, ExampleHelper.GetMesh() },
            { null, null, null, ExampleHelper.GetMesh() },
            { null, null, ExampleHelper.GetMesh(), null },
            { null, ExampleHelper.GetMesh(), null, null },
            { ExampleHelper.GetMesh(), null, null, null },
        };
    }

    #endregion

    #region Toggle

    [Toggle("Enable")] public MyToggleable toggleer = new MyToggleable();

    [Serializable]
    public class MyToggleable
    {
        public bool Enable;
        public int MyValue;
    }

    #endregion
}