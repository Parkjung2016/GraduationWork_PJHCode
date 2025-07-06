using Main.Shared;
using UnityEngine;

public class AgentSocket : MonoBehaviour
{
    public Define.ESocketType socketType;

    private MonoBehaviour _item;
#if UNITY_EDITOR
    private void OnValidate()
    {
        ChangeName();
    }

    public void ChangeSocketType(Define.ESocketType type)
    {
        socketType = type;
        ChangeName();
    }

    private void ChangeName()
    {
        gameObject.name = $"Socket_{socketType}";
    }
#endif
    public void ChangeItem(MonoBehaviour item, Vector3 position, Quaternion rotation)
    {
        item.transform.SetParent(transform);
        item.transform.SetLocalPositionAndRotation(position, rotation);
        item.transform.localScale = Vector3.one;

        _item = item;
    }

    public T GetItem<T>() where T : MonoBehaviour
    {
        return _item as T;
    }
}