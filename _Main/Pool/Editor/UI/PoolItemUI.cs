using System;
using UnityEngine.UIElements;

public class PoolItemUI
{
    private Label _nameLabel;
    private Button _deleteBtn;
    private VisualElement _rootElement;

    public event Action<PoolItemUI> OnDeleteEvent;
    public event Action<PoolItemUI> OnSelectEvent;

    public string Name {
        get => _nameLabel.text;
        set {
            _nameLabel.text = value;
        }
    }

    public PoolingItemSO itemSO;

    public bool IsActive {
        get => _rootElement.ClassListContains("active");
        set {
            if(value){
                _rootElement.AddToClassList("active");
            }else{
                _rootElement.RemoveFromClassList("active");
            }
        }
    }

    public PoolItemUI(VisualElement root, PoolingItemSO itemSO)
    {
        this.itemSO = itemSO;
        _rootElement = root.Q("pool_item");
        _nameLabel = _rootElement.Q<Label>("item_name");
        _deleteBtn = _rootElement.Q<Button>("delete_btn");
        _deleteBtn.RegisterCallback<ClickEvent>(evt => {
            OnDeleteEvent?.Invoke(this);
            evt.StopPropagation(); //Stop event propagation to parent
        });

        _rootElement.RegisterCallback<ClickEvent>(evt =>{
            OnSelectEvent?.Invoke(this);
            evt.StopPropagation(); //Stop event propagation to parent
        });
    }
}
