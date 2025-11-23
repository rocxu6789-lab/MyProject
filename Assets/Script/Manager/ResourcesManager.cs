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
        ResourceRequest request = Resources.LoadAsync<Texture>(path);
        request.completed += (AsyncOperation operation) =>
        {
            callback(request.asset as Texture);
        };
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
