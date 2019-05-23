using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{

    [SerializeField] Transform BoardTransform;
    //单元根节点
    [SerializeField] Transform ElimateRootTransform;
    //底板根节点
    [SerializeField] Transform TileRootTransform;
    //底板prefab
    [SerializeField] GameObject TilePrefab;
    //单元prefab
    [SerializeField] GameObject[] ElimatePrefabs;

    private void Awake()
    {
        BoardMgr.Ins.Init(
            BoardTransform,
            ElimateRootTransform,
            TileRootTransform,
            TilePrefab,
            ElimatePrefabs);


        SceneMgr.Ins.DoStart();
    }
}
