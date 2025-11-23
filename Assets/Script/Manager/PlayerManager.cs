using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager
{
    #region Singleton
    private static PlayerManager _instance;
    public static PlayerManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new PlayerManager();
            }
            return _instance;
        }
    }
    #endregion
    int _power = 50;
    //进入节点 扣体力
    public void UsePower(int power)
    {
        _power -= power;
        if (_power < 0)
        {
            _power = 0;
            Debug.LogError("体力为0");
        }
    }
    public string GetPowerString(int consumePower)
    {
        return $"{consumePower}/{_power}";
    }
    public int GetPower()
    {
        return _power;
    }
    //TODO: 战斗 判断体力是否足够 需要根据战斗ID获取体力消耗
    public bool IsEnoughPower(int battleId)
    {
        return _power > 0;
    }

}
