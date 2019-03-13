using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneMgr : MonoSingleTon<SceneMgr>
{

    #region private data

    private LgcLevel CurLev { get; set; }

    #endregion


    #region private function

    private void LoadLevel()
    {
        CurLev = new LgcLevel();

        BoardMgr.Ins.CurLevel = CurLev;
        BoardMgr.Ins.RemoveAllTileViews();
        BoardMgr.Ins.AddViewsForTiles();

        //todo ui 
    }

    private void StarGame()
    {
        CurLev.ResetComboMultiplier();

        Shuffle();
    }

    //重新开局
    private void Shuffle()
    {
        BoardMgr.Ins.RemoveAllElimateViews();
        HashSet<ElimateUnit> newUnit = CurLev.Shuffle();

        BoardMgr.Ins.AddViewsForElimateUnits(newUnit);
    }


    private void HandleMatches()
    {
        HashSet<ElimateChain> chains = CurLev.RemoveChains();

        if(chains.Count == 0)
        {
            StartNextTurn();
            return;
        }


    }

    private void StartNextTurn()
    {
        CurLev.ResetComboMultiplier();
        CurLev.FindPossibleSwape();

        BoardMgr.Ins.IsUserInteractionEnable = true;
    }

    #endregion


    #region public function

    public void DoStart()
    {
        LoadLevel();

        //置换回调
        BoardMgr.Ins.SwipeHandler = (LgcSwap swap) =>
        {
            BoardMgr.Ins.IsUserInteractionEnable = false;

            if (CurLev.IsPossibleSwape(swap))
            {
                CurLev.PerformSwap(swap);
                BoardMgr.Ins.ElimateSwap(swap, () =>
                 {
                     HandleMatches();
                 });
            }
            else
            {
                BoardMgr.Ins.ElimateSwap(swap, () =>
                {
                    BoardMgr.Ins.IsUserInteractionEnable = true;
                }
                );
            }
        };


        //重新开局回调

        StarGame();
    }


    #endregion
}
