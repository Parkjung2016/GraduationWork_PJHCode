using System.Collections;
using Main.Core;
using Main.Runtime.Manager;
using UnityEngine;
using Debug = Main.Core.Debug;

public class PoolManagerMono : MonoBehaviour
{
    private PoolManagerSO _poolManager;
    private bool _isSpawned;


    private IEnumerator Start()
    {
        yield return new WaitUntil(() => AddressableManager.IsLoaded);
        _poolManager = AddressableManager.Load<PoolManagerSO>("PoolManager");
        PoolManagerMono[] objs = FindObjectsByType<PoolManagerMono>(FindObjectsSortMode.None);
        if (objs.Length > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            _isSpawned = true;
            yield return _poolManager.InitializePool(transform);
            DontDestroyOnLoad(gameObject);
        }

        Debug.Log("Ǯ�� �Ϸ�");
    }


    private void OnDestroy()
    {
        if (!_isSpawned) return;
        _poolManager.ReleasePoolAsset();
    }
}