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
    public bool IsGuide = false;
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
    public bool IsHaveNextNode(out MangaNodeData nodeData)
    {
        if (_currNodeData == null)
        {
            nodeData = null;
            return false;
        }
        var index = _currNodeData.Config.Index;
        var nextNodeId = _currNodeData.Config.LevelID.GetNextId(index);
        nodeData = GetNodeDataByID(nextNodeId);
        return nodeData != null;
    }
    public bool IsHavePreNode(out MangaNodeData nodeData)
    {
        if (_currNodeData == null)
        {
            nodeData = null;
            return false;
        }
        var index = _currNodeData.Config.Index;
        var preNodeId = _currNodeData.Config.LevelID.GetPreId(index);
        nodeData = GetNodeDataByID(preNodeId);
        return nodeData != null;
    }

    public int GetConsumePower()
    {
        return CurrNodeData.Config.MangaTicketNum;
    }
    #endregion

    #region  漫画页面管理
    /// <summary>
    /// 选中的选项索引 这个变量可以todo到节点对象里面
    /// </summary>
    public int SelectOptionIndex { get; set; } = -1;
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
    public bool IsOption()
    {
        return Config.TriggerType == (int)TriggerType.Option;
    }
    public string GetOptionName(int index)
    {
        if (index < 0 || index >= Config.Option.Count)
        {
            return "";
        }
        return Config.Option[index];
    }
    public int GetOptionSkip(int index)
    {
        if (index < 0 || index >= Config.OptionSkip.Count)
        {
            return 0;
        }
        return Config.OptionSkip[index];
    }

    public int GetOptionEnd()
    {
        return Config.OptionEnd;
    }
    public List<int> GetOptionBackList()
    {
        return Config.OptionBack;
    }

    public bool IsBattle()
    {
        return Config.TriggerType == (int)TriggerType.Battle;
    }
    public int GetBattleId()
    {
        return Config.BattleId;
    }
    public bool IsEnd()
    {
        return Config.TriggerType == (int)TriggerType.End;
    }
    public string GetEndSkip()
    {
        return Config.EndSkip;
    }
}