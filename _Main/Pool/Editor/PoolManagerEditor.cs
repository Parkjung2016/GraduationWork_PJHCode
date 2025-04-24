using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

#if UNITY_EDITOR
public class PoolManagerEditor : EditorWindow
{
    [SerializeField] private VisualTreeAsset _visualTreeAsset = default;

    private ToolInfoSO _toolConfig;
    private PoolManagerSO _poolManager;
    private VisualTreeAsset _itemUXMLAsset; //아이템 UI
    private Button _createBtn;
    private ScrollView _itemView;
    private List<PoolItemUI> _itemList;
    private PoolItemUI _currentItem;

    private ItemInspector _inspector;

    [MenuItem("Tools/PoolManager")]
    public static void ShowWindow()
    {
        PoolManagerEditor wnd = GetWindow<PoolManagerEditor>();
        wnd.titleContent = new GUIContent("PoolManager");
    }

    public void CreateGUI()
    {
        VisualElement root = rootVisualElement;

        VisualElement content = _visualTreeAsset.Instantiate();
        content.style.flexGrow = 1f;
        root.Add(content);

        Initialize(content);
        GeneratePoolingItemUI();
    }

    private void Initialize(VisualElement content)
    {
        string filePath = "Assets/00Work/_Main/Pool/Editor/ToolConfig.asset";
        _toolConfig = AssetDatabase.LoadAssetAtPath<ToolInfoSO>(filePath);
        if (_toolConfig == null)
        {
            Debug.LogWarning("Tool config file is missing. generate new one");
            _toolConfig = ScriptableObject.CreateInstance<ToolInfoSO>();
            AssetDatabase.CreateAsset(_toolConfig, filePath);
        }

        _itemUXMLAsset =
            AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                $"Assets/00Work/_Main/Pool/Editor/UI/{_toolConfig.poolUxmlAssetName}");
        _createBtn = content.Q<Button>("create_btn");
        _itemView = content.Q<ScrollView>("item_view");

        _itemList = new List<PoolItemUI>();

        _inspector = new ItemInspector(content, this);

        _createBtn.clicked += HandleCreateItem;
        _inspector.NameChangeEvent += HandleAssetNameChange;
    }

    private void HandleAssetNameChange(PoolingItemSO item, string newName)
    {
        string typePath = AssetDatabase.GetAssetPath(item.poolType);
        string itemPath = AssetDatabase.GetAssetPath(item);

        PoolTypeSO exist =
            AssetDatabase.LoadAssetAtPath<PoolTypeSO>(
                $"{_toolConfig.poolingFolder}/{_toolConfig.typeFolder}/{newName}.asset");
        if (exist != null)
        {
            EditorUtility.DisplayDialog("Duplicated", $"give asset name {newName} is already exist", "OK");
            return;
        }

        AssetDatabase.RenameAsset(typePath, newName);
        AssetDatabase.RenameAsset(itemPath, newName);

        GeneratePoolingItemUI();
    }

    private void HandleCreateItem()
    {
        //타입 생성
        Guid typeGuid = Guid.NewGuid();
        PoolTypeSO typeSO = ScriptableObject.CreateInstance<PoolTypeSO>();
        typeSO.typeName = typeGuid.ToString();
        AssetDatabase.CreateAsset(typeSO,
            $"{_toolConfig.poolingFolder}/{_toolConfig.typeFolder}/{typeSO.typeName}.asset");

        PoolingItemSO newItem = ScriptableObject.CreateInstance<PoolingItemSO>();

        newItem.poolType = typeSO;

        AssetDatabase.CreateAsset(newItem,
            $"{_toolConfig.poolingFolder}/{_toolConfig.itemFolder}/{typeSO.typeName}.asset");
        _poolManager.poolingItemList.Add(newItem);

        EditorUtility.SetDirty(_poolManager);
        AssetDatabase.SaveAssets();

        GeneratePoolingItemUI();
    }

    private void GeneratePoolingItemUI()
    {
        _itemView.Clear();
        _itemList.Clear();
        _inspector.ClearInspector();

        string filePath = $"{_toolConfig.poolingFolder}/{_toolConfig.poolSOAssetName}";
        _poolManager = AssetDatabase.LoadAssetAtPath<PoolManagerSO>(filePath);
        if (_poolManager == null)
        {
            Debug.LogWarning("pool manager so is not exist, create new one");
            _poolManager = ScriptableObject.CreateInstance<PoolManagerSO>();
            AssetDatabase.CreateAsset(_poolManager, filePath);
        }

        foreach (PoolingItemSO item in _poolManager.poolingItemList)
        {
            TemplateContainer itemUI = _itemUXMLAsset.Instantiate();
            PoolItemUI poolItem = new PoolItemUI(itemUI, item);
            _itemView.Add(itemUI);
            _itemList.Add(poolItem);

            poolItem.Name = item.name;

            poolItem.OnSelectEvent += HandleSelectionEvent;
            poolItem.OnDeleteEvent += HandleDeleteEvent;
        }
    }

    private void HandleDeleteEvent(PoolItemUI item)
    {
        _poolManager.poolingItemList.Remove(item.itemSO);
        AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(item.itemSO.poolType)); //타입삭제
        AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(item.itemSO)); //아이템 삭제
        EditorUtility.SetDirty(_poolManager);
        AssetDatabase.SaveAssets();

        if (item == _currentItem)
        {
            _currentItem = null;
            //인스펙터 클리어도 해야함.
        }

        GeneratePoolingItemUI();
    }

    private void HandleSelectionEvent(PoolItemUI item)
    {
        _itemList.ForEach(item => item.IsActive = false);
        item.IsActive = true;
        _currentItem = item;
        _inspector.UpdateInspector(_currentItem.itemSO);
    }

    private void OnDestroy()
    {
        _inspector.Dispose();
    }
}
#endif