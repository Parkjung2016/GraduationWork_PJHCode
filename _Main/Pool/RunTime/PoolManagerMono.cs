using System.Collections;
using PJH.Utility.Managers;
using UnityEngine;

public class PoolManagerMono : MonoBehaviour
{
    private PoolManagerSO _poolManager;
    private bool _isSpawned;


    private IEnumerator Start()
    {
        PoolManagerMono[] objs = FindObjectsByType<PoolManagerMono>(FindObjectsSortMode.None);
        if (objs.Length > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            _isSpawned = true;
            yield return new WaitUntil(() => AddressableManager.isLoaded);
            _poolManager = AddressableManager.Load<PoolManagerSO>("PoolManager");
            if (_poolManager != null)
            {
                yield return _poolManager.InitializePool(transform);
                DontDestroyOnLoad(gameObject);
            }
        }
    }


    private void OnDestroy()
    {
        if (!_isSpawned) return;
        _poolManager.ReleasePoolAsset();
    }
}