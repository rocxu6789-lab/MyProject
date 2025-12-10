using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcesManager
{
    #region Singleton
    private static ResourcesManager _instance;
    public static ResourcesManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new ResourcesManager();
            }
            return _instance;
        }
    }
    #endregion

    private Dictionary<string, Texture> _spriteCache = new();
    public void LoadSprite(string path, Action<Sprite> callback)
    {
        ResourceRequest request = Resources.LoadAsync<Sprite>(path);
        request.completed += (AsyncOperation operation) =>
        {
            callback(request.asset as Sprite);
        };
    }
    public void LoadTexture(string path, Action<Texture> callback)
    {
        if (_spriteCache.TryGetValue(path, out Texture texture))
        {
            callback(texture);
        }
        else
        {
            ResourceRequest request = Resources.LoadAsync<Texture>(path);
            request.completed += (AsyncOperation operation) =>
            {
                if (request.asset == null)
                {
                    Debug.LogError($"Texture is null: {path}");
                    return;
                }
                Texture texture = request.asset as Texture;
                _spriteCache.Add(path, texture);
                callback(texture);
            };
        }
    }

    public void LoadGameObject(string path, Action<GameObject> callback)
    {
        ResourceRequest request = Resources.LoadAsync<GameObject>(path);
        request.completed += (AsyncOperation operation) =>
        {
            callback(request.asset as GameObject);
        };
    }
    public void SetParent(GameObject go, Transform parent, bool worldPositionStays = false)
    {
        go.transform.SetParent(parent, worldPositionStays);
        go.transform.localPosition = Vector3.zero;
        go.transform.localScale = Vector3.one;
        go.transform.localRotation = Quaternion.identity;
        go.transform.SetAsLastSibling();
    }

}
