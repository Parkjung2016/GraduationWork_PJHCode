using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

#if UNITY_EDITOR
public class ItemInspector : IDisposable
{
    private TextField _assetNameField;
    private Button _nameChangeBtn;

    private IMGUIContainer _typeView;
    private IMGUIContainer _itemView;

    private PoolManagerEditor _manager;
    private PoolingItemSO _targetItem;
    private Editor _typeEditor, _itemEditor;

    public event Action<PoolingItemSO, string> NameChangeEvent;

    public ItemInspector(VisualElement content, PoolManagerEditor managerWindow)
    {
        _manager = managerWindow;

        _assetNameField = content.Q<TextField>("asset_name_field");
        _nameChangeBtn = content.Q<Button>("btn_change");

        _typeView = content.Q<IMGUIContainer>("type_inspector_view");
        _itemView = content.Q<IMGUIContainer>("item_inspector_view");


        _typeView.onGUIHandler += HandleTypeViewGUI;
        _itemView.onGUIHandler += HandleItemViewGUI;

        _nameChangeBtn.clicked += HandleNameChange;
    }

    private void HandleNameChange()
    {
        if (_targetItem == null) return;
        if (string.IsNullOrEmpty(_assetNameField.value)) return;

        if (EditorUtility.DisplayDialog("Delete", "Rename this asset?", "Yes", "No"))
        {
            NameChangeEvent?.Invoke(_targetItem, _assetNameField.value);
        }
    }

    private void HandleTypeViewGUI()
    {
        if (_targetItem == null) return;
        Editor.CreateCachedEditor(_targetItem.poolType, null, ref _typeEditor);
        _typeEditor.OnInspectorGUI();
    }

    private void HandleItemViewGUI()
    {
        if (_targetItem == null) return;
        Editor.CreateCachedEditor(_targetItem, null, ref _itemEditor);
        _itemEditor.OnInspectorGUI();
    }

    public void UpdateInspector(PoolingItemSO item)
    {
        _assetNameField.SetValueWithoutNotify(item.poolType.name);
        _targetItem = item;
    }

    public void ClearInspector()
    {
        _assetNameField.SetValueWithoutNotify("");
        _targetItem = null;
    }

    public void Dispose()
    {
        UnityEngine.Object.DestroyImmediate(_itemEditor);
        UnityEngine.Object.DestroyImmediate(_typeEditor);
    }
}
#endif