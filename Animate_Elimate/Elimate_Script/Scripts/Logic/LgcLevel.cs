using MiniJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LgcLevel
{


    #region private data

    private int TargetScore { get; set; }

    private int MaxMumMove { get; set; }

    private int ComboMultiplier { get; set; }

    private const int Col_Max = 9;

    private const int Row_Max = 9;

    private const int Min_Chain_Len = 3;

    private ElimateUnit[,] ArrayUnit = new ElimateUnit[Col_Max, Row_Max];

    private ElimateTile[,] ArrayTile = new ElimateTile[Col_Max, Row_Max];

    private HashSet<LgcSwap> PossibleSwapSet;
    #endregion



    public LgcLevel()
    {
        //load config by level index
        InitData();
    }

    #region private loadfile

    private void InitData()
    {
        Dictionary<string, object> dictionary = LoadJSON("Levels/Level_" + 1);
        List<object> tilesList = (List<object>)dictionary["tiles"];

        for (int row = 0; row < tilesList.Count; row++)
        {
            List<object> tilesRow = (List<object>)tilesList[row];

            for (int column = 0; column < tilesRow.Count; column++)
            {
                int tile = Convert.ToInt32(tilesRow[column]);
                int tileRow = Row_Max - row - 1;

                if (tile == 1)
                {
                    ArrayTile[column, tileRow] = new ElimateTile();
                }
            }
        }

        TargetScore = Convert.ToInt32(dictionary["targetScore"]);
        MaxMumMove  = Convert.ToInt32(dictionary["moves"]);
    }

    private Dictionary<string, object> LoadJSON(string filename)
    {
        TextAsset jsonText = Resources.Load<TextAsset>(filename);

        if (jsonText == null)
        {
            Debug.LogError(string.Format("There's no '{0}' json file in Resources folder!", filename));
        }

        Dictionary<string, object> dic = Json.Deserialize(jsonText.text) as Dictionary<string, object>;

        if (dic == null)
        {
            Debug.LogError(string.Format("'{0}' json file is invalid!", filename));
        }

        return dic;
    }
    #endregion

    #region private create

    private HashSet<ElimateUnit> CreateAllElimateUnit()
    {
        HashSet<ElimateUnit> unitset = new HashSet<ElimateUnit>();

        for (int row = 0; row < Row_Max; row++)
        {
            for (int column = 0; column < Col_Max; column++)
            {
                if (ArrayTile[column, row] != null)
                {
                    ElimateType type;
                    do
                    {
                        type = ElimateUnit.RandomType();
                    }
                    while ((column >= 2 &&
                            ArrayUnit[column - 1, row] != null && ArrayUnit[column - 1, row].Type == type &&
                            ArrayUnit[column - 2, row] != null && ArrayUnit[column - 2, row].Type == type) ||
                           (row >= 2 &&
                             ArrayUnit[column, row - 1] != null && ArrayUnit[column, row - 1].Type == type &&
                             ArrayUnit[column, row - 2] != null && ArrayUnit[column, row - 2].Type == type));

                    ElimateUnit unit = CreateElimateUnit(column, row, type);
                    unitset.Add(unit);
                }
            }
        }

        return unitset;
    }

    private ElimateUnit CreateElimateUnit(int col, int row,ElimateType etype)
    {
        ElimateUnit unit = new ElimateUnit();

        unit.Type = etype;
        unit.Column = col;
        unit.Row = row;

        ArrayUnit[col, row] = unit;
        return unit;
    }

    private void VerifyLog(int col,int row)
    {
        if (col < 0 || col >= Col_Max)
        {
            Debug.LogError("Invalid col: " + col);
        }

        if (row < 0 || row >= Row_Max)
        {
            Debug.LogError("Invalid row: " + row);
        }
    }

    //是否有连组合结构
    private bool IsHasChainForUnit(int col,int row)
    {
        ElimateType unittype = ArrayUnit[col, row].Type;

        int horilen = 1;
        for (int i = col - 1; i >= 0 && ArrayUnit[i, row] != null && ArrayUnit[i, row].Type == unittype; i--, horilen++) ;
        for (int i = col + 1; i < Col_Max && ArrayUnit[i, row] != null && ArrayUnit[i, row].Type == unittype; i++, horilen++) ;
        if (horilen >= Min_Chain_Len) return true;

        int vertlen = 1;
        for (int i = row - 1; i >= 0 && ArrayUnit[col, i] != null && ArrayUnit[col, i].Type == unittype; i--, vertlen++) ;
        for (int i = row + 1; i < Row_Max && ArrayUnit[col, i] != null && ArrayUnit[col, i].Type == unittype; i++, vertlen++) ;
        return (vertlen >= Min_Chain_Len);
    }
    #endregion



    #region public match
    private HashSet<ElimateChain> FindHorizontalMatches()
    {
        HashSet<ElimateChain> chainset = new HashSet<ElimateChain>();

        for(int row= 0; row<Row_Max; row++)
        {
            for(int col=0;col<Col_Max;col++)
            {
                if(ArrayUnit[col,row] != null)
                {
                    ElimateType matchtype = ArrayUnit[col, row].Type;

                    if(ArrayUnit[col + 1,row] != null && ArrayUnit[col + 1, row].Type == matchtype &&
                       ArrayUnit[col + 2,row] != null && ArrayUnit[col + 2, row].Type == matchtype)
                    {
                        ElimateChain chain = new ElimateChain();
                        chain.ChainType = ElimateChainType.ECT_Horizontal;

                        do
                        {
                            chain.AddElimateUnit(ArrayUnit[col, row]);
                            col += 1;
                        }
                        while (col < Col_Max && 
                        ArrayUnit[col, row] != null &&
                        ArrayUnit[col, row].Type == matchtype);

                        chainset.Add(chain);
                        continue;
                    }

                    col += 1;
                }
            }
        }


        return chainset;

    }

    private HashSet<ElimateChain> FindVerticalMatches()
    {
        HashSet<ElimateChain> chainset = new HashSet<ElimateChain>();

        for (int col = 0; col < Col_Max; col++)
        {
            for (int row = 0; row < Row_Max; row++)
            {
                if (ArrayUnit[col, row] != null)
                {
                    ElimateType matchtype = ArrayUnit[col, row].Type;

                    if (ArrayUnit[col,row+1] != null && ArrayUnit[col, row+1].Type == matchtype &&
                        ArrayUnit[col,row+2] != null && ArrayUnit[col, row+2].Type == matchtype)
                    {
                        ElimateChain chain = new ElimateChain();
                        chain.ChainType = ElimateChainType.ECT_Vertical;

                        do
                        {
                            chain.AddElimateUnit(ArrayUnit[col, row]);
                            row += 1;
                        }
                        while (row < Row_Max &&
                        ArrayUnit[col, row] != null &&
                        ArrayUnit[col, row].Type == matchtype);

                        chainset.Add(chain);
                        continue;
                    }

                    row += 1;

                }
            }
        }


        return chainset;
    }


    private void RemoveElimates(HashSet<ElimateChain> chains)
    {
        foreach(ElimateChain chain in chains)
        {
            foreach(ElimateUnit unit in chain.GetElimateUnitList())
            {
                ArrayUnit[unit.Column, unit.Row] = null;
            }
        }
    }

    #endregion


    #region public get set
    public ElimateUnit GetElimateUnit(int col ,int row)
    {
        VerifyLog(col, row);
        return ArrayUnit[col, row];
    }

    public ElimateTile GetTileUnit(int col, int row)
    {
        VerifyLog(col, row);
        return ArrayTile[col, row];
    }

    public void SetComboMultiplier(int val = 1)
    {
        ComboMultiplier = val;
    }
    #endregion


    #region public logic
    
    //交换LgcSwap里两个对象在列表中的位置
    public void PerformSwap(LgcSwap sp)
    {
        int col_a = sp.first.Column;
        int row_a = sp.first.Row;

        int col_b = sp.second.Column;
        int row_b = sp.second.Row;

        //swap position
        ArrayUnit[col_a,row_a] = sp.second;
        ArrayUnit[col_b, row_b] = sp.first;

        //swap the sp
        sp.first.Column = col_b;
        sp.first.Row = row_b;
        sp.second.Column = col_a;
        sp.second.Row = row_a;
    }

    //查找是否存在链结构
    public void FindPossibleSwape()
    {
        HashSet<LgcSwap> spset = new HashSet<LgcSwap>();

        for (int row = 0; row < Row_Max; row++)
        {
            for (int col = 0; col < Col_Max; col++)
            {
                ElimateUnit unit = ArrayUnit[col, row];

                if (unit != null)
                {
                    if (col < Col_Max - 1)
                    {
                        //是否没有Tile或没有ElimateUnit
                        ElimateUnit otherunit = ArrayUnit[col + 1, row];
                        if (otherunit != null)
                        {
                            ArrayUnit[col, row] = otherunit;
                            ArrayUnit[col + 1, row] = unit;
                        }

                        if (IsHasChainForUnit(col + 1, row) || IsHasChainForUnit(col,row))
                        {
                            LgcSwap sp = new LgcSwap();
                            sp.first = unit;
                            sp.second = otherunit;
                            spset.Add(sp);
                        }

                        ArrayUnit[col, row] = unit;
                        ArrayUnit[col + 1, row] = otherunit;
                    }

                    if(row < Row_Max -1)
                    {
                        ElimateUnit otherunit = ArrayUnit[col, row + 1];
                        if (otherunit != null)
                        {
                            ArrayUnit[col, row] = otherunit;
                            ArrayUnit[col, row + 1] = unit;

                            if (IsHasChainForUnit(col, row + 1) || IsHasChainForUnit(col, row))
                            {
                                LgcSwap sp = new LgcSwap();
                                sp.first = unit;
                                sp.second = otherunit;
                                spset.Add(sp);
                            }

                            ArrayUnit[col, row] = unit;
                            ArrayUnit[col, row+1] = otherunit;
                        }
                    }
                }
            }
        }

        //找到是否交换后有链结构的组合
        PossibleSwapSet = spset;
    }

    //是否可交换 （hashset通过散射码查找效率非常高）
    public bool IsPossibleSwape(LgcSwap sp)
    {      
        return PossibleSwapSet.Contains(sp);
    }

    //移除
    public HashSet<ElimateChain> RemoveChains()
    {
        HashSet<ElimateChain> h_chainset = FindHorizontalMatches();
        HashSet<ElimateChain> v_chainset = FindVerticalMatches();

        RemoveElimates(h_chainset);
        RemoveElimates(v_chainset);

        //合并两个HashSet
        h_chainset.UnionWith(v_chainset);

        // ..TODO 计算链组合个数以及每个单位的计分

        return h_chainset;
    }


    //空位填补
    public List<List<ElimateUnit>> FillHoles()
    {
        List<List<ElimateUnit>> unitColumnlis = new List<List<ElimateUnit>>();

        for (int col = 0; col < Col_Max; col++)
        {
            List<ElimateUnit> unitlist = null;
            for (int row = 0; row < Row_Max; row++)
            {
                if (ArrayTile[col, row] != null && ArrayUnit[col, row] == null)
                {
                    for (int lookup = row + 1; lookup < Row_Max; lookup++)
                    {
                        ElimateUnit unit = ArrayUnit[col, lookup];
                        if (unit != null)
                        {
                            ArrayUnit[col, lookup] = null;
                            ArrayUnit[col, row] = unit;
                            unit.Row = row;

                            if (unitlist == null)
                            {
                                unitlist = new List<ElimateUnit>();
                                unitColumnlis.Add(unitlist);
                            }
                            unitlist.Add(unit);
                            break;
                        }
                    }
                }
            }
        }

        return unitColumnlis;
    }



    #endregion 
}
