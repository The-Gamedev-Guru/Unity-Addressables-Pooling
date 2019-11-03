using System.Collections;
using System.Collections.Generic;
using GamedevGuru;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class GamedevGuruPoolUserTest : MonoBehaviour
{
    [SerializeField] private AssetReference assetReferenceToInstantiate = null;

    IEnumerator Start()
    {
        var wait = new WaitForSeconds(8f);
        
        // 1. Wait for pool to warm up.
        yield return wait;
        
        // 2. Take an object out of the pool.
        var pool = GamedevGuruPool.GetPool(assetReferenceToInstantiate);
        var newObject = pool.Take(transform);
        
        // 3. Return it.
        yield return wait;
        pool.Return(newObject);
        
        // 4. Disable the pool, freeing resources.
        yield return wait;
        pool.enabled = false;
        
        // 5. Re-enable pool, put the asset back in memory.
        yield return wait;
        pool.enabled = true;
    }
}
