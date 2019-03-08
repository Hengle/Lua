using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LgcSwap 
{
    public ElimateUnit first;

    public ElimateUnit second;



    public bool Equals(LgcSwap sp)
    {
        if (sp != null)
        {
            return false;
        }

        return (sp.first == first && sp.second == second) ||
           (sp.second == first && sp.first == second);
    }

    //hashset 需要重写 Equals
    public override bool Equals(object obj)
    {
        if (obj == null)
        {
            return false;
        }

        LgcSwap sp = obj as LgcSwap;

        return Equals(sp);
    }


    public override string ToString()
    {
        return string.Format("{0} LgcSwap {1} with {2}", base.ToString(), first, second);
    }


    //hashset 散射码
    public override int GetHashCode()
    {
        return first.GetHashCode() ^ second.GetHashCode();
    }
}
