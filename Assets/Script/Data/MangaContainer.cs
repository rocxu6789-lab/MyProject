using System.Collections;
using System.Collections.Generic;
using Config;
using UnityEngine;

/// <summary>
/// 漫画数据容器
/// 管理漫画节点和页面的数据访问
/// </summary>
public class MangaContainer
{
    #region Singleton
    private static MangaContainer _instance;
    public static MangaContainer Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new MangaContainer();
            }
            return _instance;
        }
    }
    #endregion

    #region 初始化
    public string InitNodeId = "1_1_1";//初始节点id
    public void Init()
    {
        InitAllNodeData();
    }
    #endregion

    #region  节点管理
    MangaNodeData _currNodeData = null;
    public MangaNodeData CurrNodeData
    {
        get
        {
            return _currNodeData;
        }
        set
        {
            _currNodeData = value;
            InitAllPageData();
        }

    }
    List<MangaNodeData> _mangaNodeDataList = new List<MangaNodeData>();
    void InitAllNodeData()
    {
        foreach (var node in TableManager.Tables.MangaNode.DataList)
        {
            var mangaNodeData = new MangaNodeData(node);
            _mangaNodeDataList.Add(mangaNodeData);
        }
    }
    public MangaNodeData GetNodeDataByID(string nodeId)
    {
        return _mangaNodeDataList.Find(data => data.Config.ID == nodeId);
    }
    //尝试跳转到下一个节点
    public bool TryGoToNextNode()
    {
        if (_currNodeData == null)
        {
            Debug.LogError("当前节点数据为空");
            return false;
        }
        var index = _currNodeData.Config.Index;
        var nextNodeId = _currNodeData.Config.LevelID.GetNextId(index);
        var nextNodeData = GetNodeDataByID(nextNodeId);
        if (nextNodeData == null)
        {
            return false;
        }
        CurrNodeData = nextNodeData;
        return true;
    }
    //尝试跳转到上一个节点
    public bool TryGoToPreNode()
    {
        if (_currNodeData == null)
        {
            Debug.LogError("当前节点数据为空");
            return false;
        }
        var index = _currNodeData.Config.Index;
        var preNodeId = _currNodeData.Config.LevelID.GetPreId(index);
        var preNodeData = GetNodeDataByID(preNodeId);
        if (preNodeData == null)
        {
            return false;
        }
        CurrNodeData = preNodeData;
        return true;
    }
    #endregion

    #region  漫画页面管理
    public MangaPageData CurPageData { get; set; }
    List<MangaPageData> _mangaPageDataList = new();
    void InitAllPageData()
    {
        foreach (var page in TableManager.Tables.MangaPage.DataList)
        {
            if (page.NodeId != _currNodeData.Config.ID)
            {
                continue;
            }
            var mangaPageData = new MangaPageData(page);
            _mangaPageDataList.Add(mangaPageData);
        }
        //获取第一页
        CurPageData = _mangaPageDataList[0];
    }
    public MangaPageData GetPageDataByID(string pageId)
    {
        return _mangaPageDataList.Find(data => data.Config.ID == pageId);
    }
    #endregion

}

public class MangaNodeData
{
    public Table_MangaNode Config { get; private set; }
    public int StartIndex { get; private set; }
    public int EndIndex { get; private set; }
    public MangaNodeData(Table_MangaNode config)
    {
        Config = config;
        StartIndex = config.PageRange[0];
        EndIndex = config.PageRange[1];
    }
}
public class MangaPageData
{
    public Table_MangaPage Config { get; private set; }
    public MangaPageData(Table_MangaPage config)
    {
        Config = config;
    }
}