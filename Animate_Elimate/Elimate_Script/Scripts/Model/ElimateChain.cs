using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ElimateChain 
{
    private List<ElimateUnit> lisElimateUnit;

    public ElimateChainType ChainType { get; set; }

    public int Score { get; set; }


    #region public function

    public void AddElimateUnit(ElimateUnit unit)
    {
        if(lisElimateUnit == null)
        {
            lisElimateUnit = new List<ElimateUnit>();
        }

        lisElimateUnit.Add(unit);
    }

    public List<ElimateUnit> GetElimateUnitList()
    {
        return lisElimateUnit;
    }

    public override string ToString()
    {
        return string.Format("type:{0} animals:{1}", ChainType, lisElimateUnit);
    }


    #endregion


}
