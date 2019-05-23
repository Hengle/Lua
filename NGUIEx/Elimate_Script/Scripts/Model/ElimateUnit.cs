using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ElimateUnit 
{
    public int Column { get; set; }

    public int Row { get; set; }

    public ElimateType Type { get; set; }

    public ElimateView View { get; set; }

    public ElimateType GetUnitType()
    {
        return Type;
    }

    public bool IsSelect
    {
        get
        {
            return View.IsSelected;
        }

        set
        {
            View.IsSelected = value;
        }
    }

    #region 
     
    //获取枚举长度
    public static int TotalTypeCount()
    {
        return Enum.GetNames(typeof(ElimateType)).Length;
    }

    //随机类型
    public static ElimateType RandomType()
    {
        return (ElimateType)UnityEngine.Random.Range((int)AnimalType.Elephant, TotalTypeCount());
    }

    //测试数据 object类里的tostring()
    public override string ToString()
    {
        return string.Format("type:{0} square:({1}, {2})", Type, Column, Row);
    }

    #endregion

}
