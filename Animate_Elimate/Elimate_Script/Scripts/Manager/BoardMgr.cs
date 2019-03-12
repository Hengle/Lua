using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardMgr : MonoSingleTon<SceneMgr>
{
    #region public data

    //中断操作开关
    public bool IsUserInteractionEnable { get; set; }

    //当前消除关卡
    public LgcLevel CurLevel { get; set; }

    //交换的回调
    public Action<LgcSwap> SwipeHandler;

    //
    public float g_fTileWidth = 2.56f;

    //
    public float g_fTileHeight = 2.56f;

    #endregion


    #region private data

    private int SwipeFromCol { get; set; }

    private int SwipeFromRow { get; set; }

    private int INVALID_COLUMN_OR_ROW = -1;

    private ElimateUnit CurSelectUnit { get; set; }

    #endregion


    #region 需要整理的部分

    [SerializeField] Transform BoardTransform;
    //单元根节点
    [SerializeField] Transform ElimateRootTransform;
    //底板根节点
    [SerializeField] Transform TileRootTransform;
    //底板prefab
    [SerializeField] GameObject TilePrefab;
    //单元prefab
    [SerializeField] GameObject[] ElimatePrefabs;

    #endregion


    private void Start()
    {
        SwipeFromCol = INVALID_COLUMN_OR_ROW;
        SwipeFromRow = INVALID_COLUMN_OR_ROW;
    }

    //更新操作
    private void Update()
    {
        HandInput();
    }


    #region private function

    private void HandInput()
    {

    }

    private Vector2 PointForCell(int col,int row)
    {
        return Vector2.zero;
    }

    //根据鼠标点获取格子的坐标
    private bool ConvertPoint(Vector2 point, out int col, out int row)
    {
        if (point.x > 0)
        {
            col = 0;
            row = 0;
            return true;
        }
        else
        {
            col = INVALID_COLUMN_OR_ROW;
            row = INVALID_COLUMN_OR_ROW;

            return false;
        }
    }

    //
    private void TrySwapHorizontal(int horizontalDetal, int vertiaclDetal)
    {
        int toCol = SwipeFromCol + horizontalDetal;
        int toRow = SwipeFromRow + vertiaclDetal;

        if (toCol < 0 || toCol >= LgcLevel.Col_Max)
        {
            return;
        }

        if (toRow < 0 || toRow >= LgcLevel.Row_Max)
        {
            return;
        }

        ElimateUnit toUnit = CurLevel.GetElimateUnit(toCol, toRow);
        if(toUnit == null)
        {
            return;
        }

        ElimateUnit fromUnit = CurLevel.GetElimateUnit(SwipeFromCol, SwipeFromRow);

        if(SwipeHandler != null)
        {
            LgcSwap sp = new LgcSwap();
            sp.first = fromUnit;
            sp.second = toUnit;
            SwipeHandler(sp);
        }
    }

    //选中
    private void SelectUnit(ElimateUnit unit)
    {
        unit.IsSelect = true;
        CurSelectUnit = unit;
    }

    //取消选中
    private void DeselectUnit()
    {
        if(CurSelectUnit != null)
        {
            CurSelectUnit.IsSelect = false;
            CurSelectUnit = null;
        }
    }

    private void ElimateScoreForChain(ElimateChain chains)
    {

    }
    #endregion


    #region public function

    public void AddViewsForElimateUnits(ElimateUnit unit)
    {
        int unitprefabindex  = (int)unit.Type;
        Vector2 pos = PointForCell(unit.Column,unit.Row);
        GameObject obj = ElimatePrefabs[unitprefabindex].Spawn(ElimateRootTransform, pos);

        //设置新生成的单元信息
        ElimateView unitview = obj.GetComponent<ElimateView>();
        unitview.transform.localScale = new Vector2(0.5f, 0.5f);
        unit.View = unitview;

        //todo 可以对unitview做处理


        //动画
        Sequence sequence = DOTween.Sequence();
        sequence.Append(unitview.transform.DOScale(1f,0.25f))
            .PrependInterval(UnityEngine.Random.Range(0f, 0.5f)
            );
    }


    public void AddViewsForElimateUnits(HashSet<ElimateUnit> unitset)
    {
        foreach(ElimateUnit v in unitset)
        {
            AddViewsForElimateUnits(v);
        }
    }

    public void AddViewsForTiles()
    {
        for(int row = 0; row < LgcLevel.Row_Max; row++)
        {
            for(int col = 0;col<LgcLevel.Col_Max; col++)
            {
                if(CurLevel.GetTileUnit(col,row) != null)
                {
                    GameObject objtile = TilePrefab.Spawn(TileRootTransform, PointForCell(col, row));
                    objtile.layer = TileRootTransform.gameObject.layer;
                }
            }
        }
    }

    public void ElimateSwap(LgcSwap swap,Action completion)
    {
        swap.first.View.ViewIn.gameObject.SetActive(true);
        swap.first.View.ViewOut.gameObject.SetActive(false);

        swap.second.View.ViewIn.gameObject.SetActive(false);
        swap.second.View.ViewOut.gameObject.SetActive(true);

        float duration = 0.3f;

        swap.first.View.transform.DOMove(swap.second.View.transform.position, duration).SetEase(Ease.OutExpo).OnComplete(() => completion());
        swap.second.View.transform.DOMove(swap.first.View.transform.position, duration).SetEase(Ease.OutExpo);
    }


    public void ElimateInvalidSwap(LgcSwap swap,Action completion)
    {
        swap.first.View.ViewIn.gameObject.SetActive(true);
        swap.first.View.ViewOut.gameObject.SetActive(false);

        swap.second.View.ViewIn.gameObject.SetActive(false);
        swap.second.View.ViewOut.gameObject.SetActive(true);

        float duration = 0.3f;

        swap.first.View.transform.DOMove(swap.second.View.transform.position, duration).SetEase(Ease.OutExpo).SetLoops(2, LoopType.Yoyo).OnComplete(() => completion());
        swap.second.View.transform.DOMove(swap.first.View.transform.position, duration).SetEase(Ease.OutExpo).SetLoops(2, LoopType.Yoyo);

    }

    public void ElimateMatchedElimates(HashSet<ElimateChain> chains,Action completion)
    {
        foreach(ElimateChain v in chains)
        {
            ElimateScoreForChain(v);

            foreach(ElimateUnit unit in v.GetElimateUnitList())
            {
                if(unit.View)
                {
                    ElimateView unitView = unit.View;
                    unitView.transform.DOScale(0.1f, 0.3f).SetEase(Ease.OutExpo).OnComplete(() => { unitView.Recycle(); unit.View = null; });

                }
            }
        }

        //todo sound

        // TODO: try to call completion in better way!
        transform.DOScale(Vector3.one, 0.3f).OnComplete(() => completion());

    }

    public void ElimateFallingElimates(List<List<ElimateUnit>> cols,Action completion)
    {
        float fduration = 0f;
        
        foreach(List<ElimateUnit> units in cols)
        {
            int unitindex = 0;
            foreach(ElimateUnit v in units)
            {
                Vector2 newpos = PointForCell(v.Column, v.Row);

                float fdealy = 0.05f + 0.15f * fduration;
                float dtime = v.View.transform.position.y - newpos.y / g_fTileHeight * 0.1f;

                fduration = Mathf.Max(fduration, dtime + fdealy);

                v.View.transform.DOLocalMove(newpos, dtime).SetDelay(fdealy).SetEase(Ease.OutExpo);

                unitindex++;
            }
            
        }
    }


    public void ElimateNewElimates(List<List<ElimateUnit>> cols, Action completion)
    {
        float flongduration = 0f;

        foreach(List<ElimateUnit> unit in cols)
        {
            int startRow  = unit[0].Row + 1;
            int unitIndex = 0;

            foreach(ElimateUnit v in unit)
            {
                int unitprefabindex = (int)v.GetUnitType();
                Vector2 pos = PointForCell(v.Column, startRow);
                GameObject obj = ElimatePrefabs[unitprefabindex].Spawn(ElimateRootTransform, pos);
                ElimateView unitview = obj.GetComponent<ElimateView>();

                unitview.transform.localScale = Vector3.one;
                v.View = unitview;

                float fdealy = 0.1f + 0.2f * (unit.Count - unitIndex - 1);
                float fduration = (startRow - v.Row) * 0.1f;
                flongduration = Mathf.Max(flongduration, fduration + fdealy);

                Vector2 newpos = PointForCell(v.Column, v.Row);

                //todo unitview 

                //动画
                Sequence sequence = DOTween.Sequence();
                sequence.Append(unitview.transform.DOLocalMove(newpos, fduration)).SetEase(Ease.OutExpo)
                    .PrependInterval(fdealy);

                unitIndex++;
            }
        }

        // TODO: try to call completion in better way!
        transform.DOScale(Vector3.one, flongduration).OnComplete(() => completion());

    }

    public void ElimateStartGame()
    {

    }

    public void ElimateGameOver()
    {

    }

    public void RemoveAllTileViews()
    {
        int count = TileRootTransform.childCount;
        for(int i= count -1; i>=0; i--)
        {
            TileRootTransform.GetChild(i).Recycle();
        }
    }

    public void RemoveAllElimateViews()
    {
        int count = ElimateRootTransform.childCount;
        for(int i= count -1;i>=0;i--)
        {
            ElimateRootTransform.GetChild(i).Recycle();
        }
    }

    #endregion

}
