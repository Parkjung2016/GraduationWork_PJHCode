using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(menuName = "SO/Pool/Item")]
public class PoolingItemSO : ScriptableObject
{
    public PoolTypeSO poolType;
    public AssetReference prefab;
    public int initCount;
}
