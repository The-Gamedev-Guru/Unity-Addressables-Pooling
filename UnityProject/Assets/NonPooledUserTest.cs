using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class NonPooledUserTest : MonoBehaviour
{
    [SerializeField] private AssetReference assetReferenceToInstantiate = null;
    private AsyncOperationHandle<GameObject> _asyncOperationHandle;
    
    void Start()
    {
        _asyncOperationHandle = assetReferenceToInstantiate.InstantiateAsync(transform);
    }

    void OnDestroy()
    {
        Addressables.Release(_asyncOperationHandle);
    }
}
