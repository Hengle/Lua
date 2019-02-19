/*
               #########                       
              ############                     
              #############                    
             ##  ###########                   
            ###  ###### #####                  
            ### #######   ####                 
           ###  ########## ####                
          ####  ########### ####               
         ####   ###########  #####             
        #####   ### ########   #####           
       #####   ###   ########   ######         
      ######   ###  ###########   ######       
     ######   #### ##############  ######      
    #######  #####################  ######     
    #######  ######################  ######    
   #######  ###### #################  ######   
   #######  ###### ###### #########   ######   
   #######    ##  ######   ######     ######   
   #######        ######    #####     #####    
    ######        #####     #####     ####     
     #####        ####      #####     ###      
      #####       ###        ###      #        
        ###       ###        ###               
         ##       ###        ###               
__________#_______####_______####______________

                我们的未来没有BUG              
* ==============================================================================
* Filename: UILoopTable.cs
* Created:  2017/11/20 11:04:39
* Author:   To Hard The Mind
* Purpose:  
* ==============================================================================
*/
using System;
using System.Collections.Generic;
using UnityEngine;
using LuaInterface;

public class UILoopTable : UIWidgetContainer {

    #region member
    public LoopTableLuaItem itemTemplate;
    public Vector2 padding = Vector2.zero;

    private int m_cacheNum = 1;
    private UIScrollView m_scrollView = null;
    private UIPanel m_panel = null;
    private Vector3 m_panelInitPos;
    private Vector2 m_initOffset;
    private UIScrollView.Movement m_moveType = UIScrollView.Movement.Vertical;
    //重刷的时候被重置的数据
    private bool m_initiated = false;
    private int m_dataCount;                                        //绘制的数据

    private int m_maxArrangeNum = 0;                                //无限加载时最多的Item行或列数量
    private int m_dataArrangeNum = 0;                               //实际数据的行列数量
    private int m_fillCount = 0;                                    //填满panel的行列数量

    private int m_lastDataIndex = 0;                                //上次显示出来的第一个格子，在数据中的行列索引
    //private int m_maxIndex = 0;                                     //显示出来的数据最大行列索引
    //private int m_minIndex = 0;                                     //显示出来的数据最小行列索引

    private Dictionary<int, float> m_distanceDict = new Dictionary<int, float>();
    private Dictionary<float, int> m_indexDict = new Dictionary<float, int>();

    private Vector3 startPos;
    //private Vector3 endPos;??

    #endregion
    private void CalAllDistance(){
        
    }
    public float GetDistanceByIndex(int index) {
        float result = 0;

        itemTemplate.gameObject.SetActive(true);
        itemTemplate.FillItem(index, false);
        Bounds b = NGUIMath.CalculateRelativeWidgetBounds(itemTemplate.transform, false);
        float cellHeight = b.size.y + padding.y;
        float cellWidth = b.size.x + padding.x;
        float cellLen = m_moveType == UIScrollView.Movement.Vertical ? cellHeight:cellWidth;
        itemTemplate.gameObject.SetActive(false);

        if (!m_distanceDict.TryGetValue(index, out result)) {
            if (index == 0) {
                result = 0;
            }
            else {
                int lastIndex = index - 1;
                float lastLen = GetDistanceByIndex(lastIndex);
                result = cellLen + lastLen;
            }
            m_distanceDict.Add(index, result);
            m_indexDict.Add(result, index);
        }

        return result;
    }

    #region property
    private float m_cellLength = -1;
    public float cellLength {
        get {
            if (m_cellLength <= 0) {
                itemTemplate.gameObject.SetActive(true);
                Bounds b = NGUIMath.CalculateRelativeWidgetBounds(itemTemplate.transform, false);
                float cellHeight = b.size.y + padding.y;
                float cellWidth = b.size.x + padding.x;
                itemTemplate.gameObject.SetActive(false);

                if (!m_initiated) {
                    InitPosAndScroll();
                }
                if (m_moveType == UIScrollView.Movement.Vertical) {
                    m_cellLength = cellHeight;
                }
                else if (m_moveType == UIScrollView.Movement.Horizontal) {
                    m_cellLength = cellWidth;
                }
            }
            return m_cellLength;
        }
    }
    #endregion

    #region pool
    [HideInInspector]
    [SerializeField]
    private GameObject m_poolGo;

    private List<LoopTableLuaItem> m_items = new List<LoopTableLuaItem>();
    private Queue<LoopTableLuaItem> m_itemQueuePool = new Queue<LoopTableLuaItem>();
    private ObjectPool<GameObject> m_itemCOPool = new ObjectPool<GameObject>();
    private GameObject CreateItemGO() {
        GameObject itemGO = GameObject.Instantiate(itemTemplate.gameObject);
        itemGO.gameObject.SetActive(false);
        return itemGO;
    }
    private void Init(GameObject itemCO) {
        itemCO.SetActive(false);
        itemCO.transform.parent = m_poolGo.transform;
    }
    private void CreateItemPool(GameObject itemTemplate, int poolNum) {
        if (m_poolGo == null) {
            m_poolGo = new GameObject();
            m_poolGo.name = "pool";
            m_poolGo.transform.parent = transform.parent;
            m_poolGo.transform.localScale = Vector3.one;
        }
        m_itemCOPool.Init(poolNum, CreateItemGO, Init);

        for (int i = 0, imax = transform.childCount; i < imax; i++) {
            GameObject go = transform.GetChild(0).gameObject;
            //LoopGridLuaItem item = go.GetComponent<LoopGridLuaItem>();

            //if (item == null) {
            //    item = go.AddComponent<LoopGridLuaItem>();
            //}
            //item.grid = this;

            m_itemCOPool.Store(go);
        }
    }
    private GameObject GetGridItem() {
        if (m_itemQueuePool.Count > 0) {
            return m_itemQueuePool.Dequeue().gameObject;
        }
        return m_itemCOPool.GetObject();
    }
    public void StoreQueuePoolItem() {
        while (m_itemQueuePool.Count > 0) {
            m_itemCOPool.Store(m_itemQueuePool.Dequeue().gameObject);
        }
    }
    private void StoreAllItem() {
        for (int i = 0; i < m_items.Count; i++) {
            m_itemQueuePool.Enqueue(m_items[i]);
        }
        m_items.Clear();
    }

    private LoopTableLuaItem AddOneItem(int index) {
        GameObject go = GetGridItem();

        LoopTableLuaItem item = go.GetComponent<LoopTableLuaItem>();
        if (item == null || !item.inited) {
            if (item == null) {
                item = go.AddComponent<LoopTableLuaItem>();
            }
            item.table = this;
            item.SetFirstItemData(m_dataCount, index);
            item.FindItem();
        }

        item.transform.parent = transform;
        item.transform.localScale = Vector3.one;
        item.gameObject.SetActive(true);
        item.FillItem(index, true);

        return item;
    }

    private void AddItems() {
        StoreAllItem();
        for (int i = 0; i < m_maxArrangeNum; i++) {
            LoopTableLuaItem item = AddOneItem(i);
            m_items.Add(item);
        }
        StoreQueuePoolItem();
    }

    public void ResetToBegin() {
        ResetPosition();
        //CheckDragBack();
        CheckLoopMove();

        m_scrollView.DisableSpring();
        AddItems();
    }

    private void CheckLoopMove() {
        m_panel.onClipMove = null;
        if (m_dataArrangeNum < m_maxArrangeNum) {
            m_panel.onClipMove = null;
            m_scrollView.restrictWithinPanel = true;
        }
        else {
            m_lastDataIndex = 0; //上次显示出来的第一个格子，在grid数据中的index
            //m_maxIndex = m_maxArrangeNum - 1;
            //m_minIndex = 0;
            m_panel.onClipMove = OnPanelLoopClipMove;
        }

    }

    private void ResetPosition() {
        m_scrollView.transform.localPosition = m_panelInitPos;
        m_panel.clipOffset = m_initOffset;
    }
    #endregion

    #region private
    private void EnableDestory() {
        bool active = gameObject.activeSelf;
        if (!active) {
            var tr = transform.parent;
            transform.parent = null;

            gameObject.SetActive(true);
            gameObject.SetActive(false);
            transform.parent = tr;
        }
    }
    private void InitPosAndScroll() {
        m_scrollView = transform.parent.GetComponent<UIScrollView>();
        if (m_scrollView == null) Debug.LogException(new Exception("父节点必须有ScrollView这个组件"));
        m_scrollView.momentumAmount = 100;

        m_panel = m_scrollView.GetComponent<UIPanel>();
        if (m_panel == null) Debug.LogException(new Exception("父节点必须有UIPanel这个组件"));

        m_panelInitPos = m_scrollView.transform.localPosition;
        m_initOffset = m_panel.clipOffset;
        m_moveType = m_scrollView.movement;
        EnableDestory();

        if (itemTemplate != null) {
            itemTemplate.gameObject.SetActive(false);
        }
        m_initiated = true;
    }
    private void Clear() {
        m_dataCount = 0;
        m_maxArrangeNum = 0;
        m_dataArrangeNum = 0;
        m_fillCount = 0;

        m_lastDataIndex = 0;
        //m_maxIndex = 0;
        //m_minIndex = 0;
    }
    private void SlotData(int dataCount) {
        m_dataCount = dataCount;
        if (itemTemplate == null) Debug.LogException(new Exception("模板不能为空"));

        m_dataArrangeNum = m_dataCount;

        float len = cellLength;
        float allLen = m_moveType == UIScrollView.Movement.Horizontal ? m_panel.width : m_panel.height;

        m_fillCount = Mathf.CeilToInt(allLen / len);

        m_maxArrangeNum = Math.Min(m_dataArrangeNum, m_fillCount + m_cacheNum);

        CreateItemPool(itemTemplate.gameObject, 0);
    }
    #endregion

    #region menu
    [ContextMenu("Execute")]
    private void Execute() {
        CreateScrollView(100);
    }
    public void CreateScrollView(int dataCount) {
        if (!m_initiated) {
            InitPosAndScroll();
        }
        Clear();
        SlotData(dataCount);
        ResetToBegin();
    }
    #endregion

    #region callback
    private void CalDistance(float distance, out float targetDis, out int index) {
        //FixBoxPostion();
        distance = Mathf.Abs(distance);
        targetDis = distance;
        index = Mathf.FloorToInt(targetDis / cellLength);
    }

    private void OnPanelLoopClipMove(UIPanel panel) {
        if (m_dataCount <= 0) {
            return;
        }
        Vector3 delata = m_panelInitPos - panel.transform.localPosition;
        float distance = -1;

        int index;//当前显示出来的第一个格子，在grid数据中的index
        distance = delata.y != 0 ? delata.y : delata.x;
        // 满的时候向上滑不管它
        if (distance > 0 && m_moveType == UIScrollView.Movement.Vertical) {
            distance = 0;
        }
        if (distance < 0 && m_moveType == UIScrollView.Movement.Horizontal) {
            distance = 0;
        }

        CalDistance(distance, out distance, out index);
        // 拖拽不满一个单元格
        if (index == m_lastDataIndex) return;

        // 拉到底了
        if (index + m_fillCount >= m_dataArrangeNum) {
            index = m_dataArrangeNum - m_fillCount;
        }
        // 重刷
        int offset = Math.Abs(index - m_lastDataIndex);

        // 判断要把最上（左）的item移动到最下（右）,还是相反
        if (m_lastDataIndex < index) {
            for (int i = 1; i <= offset; i++) {
                //上（左）移动到下（右）
                //MoveGridItem(true);
            }

        }
        else {
            for (int i = 1; i <= offset; i++) {
                //上（左）移动到下（右）
                //MoveGridItem(false);
            }
        }

        m_lastDataIndex = index;
    }
    #endregion

}