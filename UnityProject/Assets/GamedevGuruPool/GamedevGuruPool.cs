
// Copyright 2019 The Gamedev Guru (http://thegamedev.guru)
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// https://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.using System.Collections;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Assertions;

namespace GamedevGuru
{
public class GamedevGuruPool : MonoBehaviour
{
    public bool IsReady { get { return loadingCoroutine == null; } }

    [SerializeField] private int elementCount = 8;
    [SerializeField] private AssetReference assetReferenceToInstantiate = null;
    
    private static Dictionary<object, GamedevGuruPool> allAvailablePools = new Dictionary<object, GamedevGuruPool>();
    private Stack<GameObject> pool = null;
    private Coroutine loadingCoroutine;

    public static GamedevGuruPool GetPool(AssetReference assetReference)
    {
        var exists = allAvailablePools
            .TryGetValue(assetReference.RuntimeKey, out GamedevGuruPool pool);
        if (exists)
        {
            return pool;
        }

        return null;
    }

    public GameObject Take(Transform parent)
    {
        Assert.IsTrue(IsReady, $"Pool {name} is not ready yet");
        if (IsReady == false) return null;
        if (pool.Count > 0)
        {
            var newGameObject = pool.Pop();
            newGameObject.transform.SetParent(parent, false);
            newGameObject.SetActive(true);
            return newGameObject;
        }

        return null;
    }

    public void Return(GameObject gameObjectToReturn)
    {
        gameObjectToReturn.SetActive(false);
        gameObjectToReturn.transform.parent = transform;
        pool.Push(gameObjectToReturn);
    }
    

    void OnEnable()
    {
        Assert.IsTrue(elementCount > 0, "Element count must be greater than 0");
        Assert.IsNotNull(assetReferenceToInstantiate, "Prefab to instantiate must be non-null");
        allAvailablePools[assetReferenceToInstantiate.RuntimeKey] = this;
        loadingCoroutine = StartCoroutine(SetupPool());
    }

    void OnDisable()
    {
        allAvailablePools.Remove(assetReferenceToInstantiate);
        foreach (var obj in pool)
        {
            Addressables.ReleaseInstance(obj);
        }
        pool = null;
    }

    private IEnumerator SetupPool()
    {
        pool = new Stack<GameObject>(elementCount);
        for (var i = 0; i < elementCount; i++)
        {
            var handle = assetReferenceToInstantiate.InstantiateAsync(transform);
            yield return handle;
            var newGameObject = handle.Result;
            pool.Push(newGameObject);
            newGameObject.SetActive(false);
        }

        loadingCoroutine = null;
    }
}
}
