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
* Filename: UILoopGrid
* Created:  2017/3/16 13:24:12
* Author:   To Hard The Mind
* Purpose:  
* ==============================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LuaInterface;

public class UILoopGrid : DisableItem {

    #region member
    public bool useAnimate = false;
    public float deltaTime = 0.0f;

    public GameObject upArrow;
    public GameObject downArrow;
    public GameObject itemTemplate;
    public int amount = 190;
    public UIDragScrollView m_drag;
    public bool hasChild = false;
    [Range(1, 10)]
    public int maxPerLine = 1;
    public float cellWidth = 200f;
    public float cellHeight = 200f;

    public int testCount = 100;//仅供测试的时候使用

    [SerializeField]
    public int selectIndex = -1;

    public int selectLimit = -1;//-1无上限

    public bool isStartFill {
        get {
            return m_isStartFill;
        }
    }
    private bool m_isStartFill = false;

    public float cellLength {
        get {
            float result = cellHeight;
            if (!m_initiated) {
                InitPosAndScroll();
            }
            if (m_moveType == UIScrollView.Movement.Vertical) {
                result = cellHeight;
            }
            else if (m_moveType == UIScrollView.Movement.Horizontal) {
                result = cellWidth;
            }
            return result;
        }
    }

    private int m_childCount = -1;
    private bool m_initiated = false;
    private Queue<LoopGridLuaItem> m_itemQueuePool = new Queue<LoopGridLuaItem>();
    private UIScrollView m_scrollView;
    private UIPanel m_panel;
    private List<List<LoopGridLuaItem>> m_items = new List<List<LoopGridLuaItem>>();
    private Vector3 m_panelInitPos = Vector3.zero;                  //最初的panel位置
    private Vector2 m_initOffset = Vector2.zero;                    //panel的offset
    private UIScrollView.Movement m_moveType = UIScrollView.Movement.Vertical;

    //重刷的时候被重置的数据
    private int m_dataCount;                                        //绘制的数据
    private LuaFunction m_createTableFunc;                          //luatable的创建函数
    private LuaFunction m_childCreateTableFunc;                     //做dragcenter 的子控件函数
    public LuaTable uiCoreTable { private set; get; }               //操作UI的 table
    public LuaTable dataTable { private set; get; }                 //数据
    private LuaTable m_luaTable;

    private int m_maxArrangeNum = 0;                                //无限加载时最多的Item行或列数量
    private int m_dataArrangeNum = 0;                               //实际数据的行列数量
    private int m_fillCount = 0;                                    //填满panel的行列数量
    private int m_floorFillCount = 0;                               //正好不超过panel边缘的行列数量
    private float m_cellHalf = 0;                                   //cell中间的位置
    private float m_maxDistance = 0;                                //数据所需panel最大长度
    private float m_halfPanelWidth = 0;
    private float m_halfPanelHeight = 0;

    private int m_lastDataIndex = 0;                                //上次显示出来的第一个格子，在数据中的行列索引
    private int m_maxIndex = 0;                                     //显示出来的数据最大行列索引
    private int m_minIndex = 0;                                     //显示出来的数据最小行列索引
    private Vector3 startPos;
    private Vector3 endPos;
    private int m_cacheNum = 1;
    private HashSet<int> m_multiSelects = new HashSet<int>();
    private UIRoot m_uiroot;
    private Coroutine m_timer;
    public MonoBehaviour handleUI { get; private set; }
    #endregion

    #region pool
    [HideInInspector]
    [SerializeField]
    private GameObject m_poolGo;

    private ObjectPool<GameObject> m_itemCOPool = new ObjectPool<GameObject>();

    private GameObject CreateItemGO() {
        GameObject itemGO = GameObject.Instantiate(itemTemplate);
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

        #if UNITY_EDITOR
        if (transform.childCount <= 0 && Application.isPlaying) {
            Debug.LogError("这个并没有使用对象池，请点击脚本齿轮上的Execute.不然手机上打开界面会变慢");
        }
        #endif

        for (int i = 0, imax = transform.childCount; i < imax; i++) {
            GameObject go = transform.GetChild(0).gameObject;
            LoopGridLuaItem item = go.GetComponent<LoopGridLuaItem>();

            if (item == null) {
                item = go.AddComponent<LoopGridLuaItem>();
            }
            item.grid = this;

            m_itemCOPool.Store(go);
        }
    }
    private LuaTable CreateFillTable() {
        LuaTable result = null;

        if (m_createTableFunc != null) {
            result = m_createTableFunc.callReturnTable();
            if (result == null) throw new LuaException("must a function return table");
        }
        else if (m_luaTable != null) {
            return m_luaTable;
        }
        else {
            if (GameLogger.IsEnable) GameLogger.Error("c# m_createTableFunc is null");
        }
        return result;
    }
    public LuaTable CreateChildFillTable() {
        LuaTable result = null;
        if (m_childCreateTableFunc != null) {
            result = m_createTableFunc.callReturnTable();
            if (result == null) throw new LuaException("must a function return table");
        }
        return result;
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
            for (int j = 0; j < m_items[i].Count; j++) {
                m_itemQueuePool.Enqueue(m_items[i][j]);
            }
        }
        m_items.Clear();
    }
    #endregion

    #region virtual
#if UNITY_EDITOR
    void OnValidate() {
        if (!Application.isPlaying && NGUITools.GetActive(this)) {
            if (m_panel != null) {
                RefreshData();
            }
        }
    }
#endif
    #endregion

    #region public
    public void SetUICoreTable(LuaTable tb) {
        uiCoreTable = tb;
    }

    public bool CanAddMultiSelect() {
        return selectLimit == -1 || m_multiSelects.Count < selectLimit;
    }
    public void AddMultiSelect(int index) {
        if (!m_multiSelects.Contains(index)) {
            m_multiSelects.Add(index);
        }
    }
    public void RemoveMultiSelect(int index) {
        m_multiSelects.Remove(index);
    }
    public bool ContainsMultiSelect(int index) {
        return m_multiSelects.Contains(index);
    }
    public void ClearMultiSelect() {
        m_multiSelects.Clear();
		selectIndex = -1;
	}
    public int GetMultiSelectCount() {
        return m_multiSelects.Count;
    }
    public LuaTable GetMultiSelect() {
        IntPtr L = LuaScriptMgr.Instance.GetL();
        int oldTop = LuaDLL.lua_gettop(L);

        LuaDLL.lua_newtable(L);
        LuaTable tb = LuaScriptMgr.ToLuaTable(L, -1);

        int num2 = 0;
        var itr = m_multiSelects.GetEnumerator();
        while (itr.MoveNext()) {
            LuaDLL.lua_pushnumber(L, (double)itr.Current);
            LuaDLL.lua_pushnumber(L, (double)(++num2));
            LuaDLL.lua_insert(L, -2);
            LuaDLL.lua_settable(L, -3);
        }
        LuaDLL.lua_settop(L, oldTop);

        return tb;
    }
    /// <summary>
    /// 移动到对应数据index的列
    /// </summary>
    /// <param name="index"></param>
    public void MoveToIndex(int index) {
        bool curIsFill = m_dataArrangeNum >= m_fillCount;
        if (!curIsFill) {
            return;
        }
        index = Mathf.CeilToInt((float)index / (float)maxPerLine);
        MoveRelative(index);
    }

    public void MoveRelativeSmooth(int delta) {
        if (m_scrollView.canMoveVertically) {
            float maxHeight = m_dataArrangeNum * cellHeight - m_panel.height;
            float targetHeight = Mathf.Min(maxHeight, (delta - m_lastDataIndex) * cellHeight);
            m_scrollView.MoveRelativeSmooth(new Vector3(0, targetHeight));
        }
        else {
            float maxWidth = m_panel.width - m_dataArrangeNum * cellWidth;
            float targetWidth = Mathf.Max(maxWidth, (m_lastDataIndex - delta) * cellWidth);
            m_scrollView.MoveRelativeSmooth(new Vector3(targetWidth, 0));
        }
    }

    public void MoveRelative(int delta) {
        m_scrollView.DisableSpring();

        if (m_scrollView.canMoveVertically) {
            float maxHeight = m_dataArrangeNum * cellHeight - m_panel.height;
            float targetHeight = Mathf.Min(maxHeight, (delta - m_lastDataIndex) * cellHeight);
            m_scrollView.MoveRelative(new Vector3(0, targetHeight));
        }
        else {
            float maxWidth = m_panel.width - m_dataArrangeNum * cellWidth;
            float targetWidth = Mathf.Max(maxWidth, (m_lastDataIndex - delta) * cellWidth);
            m_scrollView.MoveRelative(new Vector3(targetWidth, 0));
        }

        gameObject.SetActive(false);
        gameObject.SetActive(true);
    }

    public void InvalidatePanel() {
        m_initiated = false;
    }
    public void CreateDragCenterScrollView(int dataCount, LuaFunction luaFun, LuaTable data) {
        if (m_childCount == -1) {
            LoopGridLuaItem[] items = itemTemplate.GetComponentsInChildren<LoopGridLuaItem>(true);
            m_childCount = Mathf.Max(items.Length - 1, 1);
        }
        dataCount = Mathf.CeilToInt((float)dataCount / (float)m_childCount);
        m_childCreateTableFunc = luaFun;
        CreateScrollView(dataCount, null, data);
    }
    public void CreateScrollView2(int dataCount, LuaTable t, LuaTable data) {
        if (!m_initiated) {
            InitPosAndScroll();
        }
        Clear();
        SlotData(dataCount);
        m_luaTable = t;
        dataTable = data;
        ResetToBegin();
    }
    public void CreateScrollView(int dataCount, LuaFunction createTableFunc) {
        CreateScrollView(dataCount, createTableFunc, null);
    }
    public void CreateScrollView(int dataCount, LuaFunction createTableFunc, LuaTable data) {
        try {
            if (!m_initiated) {
                InitPosAndScroll();
            }
            Clear();
            SlotData(dataCount);
            m_createTableFunc = createTableFunc;
            dataTable = data;
            ResetToBegin();
        }
        catch (Exception e) {
            if (GameLogger.IsEnable) GameLogger.Error(e.ToString());
        }

    }
    public void ResetToBegin() {
        ResetPosition();
        CheckIsNeedArrows();
        CheckDragBack();
        CheckLoopMove();
        m_scrollView.DisableSpring();
        if (!useAnimate || !Application.isPlaying) {
            AddItems();
        }
        else {
            if (m_uiroot == null) { 
                m_uiroot = UIRoot.list[0];
            }
            if (m_timer != null) {
                m_uiroot.StopCoroutine(m_timer);
                m_timer = null;
            }
            m_timer = m_uiroot.StartCoroutine(AddItemsAsync());
        }
    }

    public void RefreshData(int dataCount, LuaTable tb) {
        dataTable = tb;
        m_dataCount = dataCount;
        RefreshData();
    }

    /// <summary>
    /// 某个数据发生变化，直接改变数据,然后调用这个函数进行重新填充（比如折叠的item展开）
    /// 特点不会把scroll重置到原始状态
    /// </summary>
    public void RefreshData() {
        int dataArrangeNum = Mathf.CeilToInt((float)m_dataCount / (float)maxPerLine);
        bool preIsLoop = m_panel.onClipMove == OnPanelLoopClipMove;
        bool curIsLoop = dataArrangeNum >= m_fillCount + m_cacheNum;

        bool olduseAnimate = useAnimate;
        useAnimate = false;
        //调整item数量
        if (m_dataArrangeNum == dataArrangeNum) {
            for (int i = m_items.Count - 1; i >= 0; i--) {
                List<LoopGridLuaItem> list = m_items[i];
                for (int j = list.Count - 1; j >= 0; j--) {
                    LoopGridLuaItem item = list[j];
                    item.UpdateItem();
                }
            }
        }
        else if (preIsLoop && curIsLoop) {
            int tmpIndex = m_lastDataIndex;

            if (m_dataArrangeNum > dataArrangeNum && CheckIndexOut()) {
                tmpIndex = Mathf.Max(0, m_dataCount - m_fillCount * maxPerLine);
            }
            m_dataArrangeNum = dataArrangeNum;
            ResetToBegin();
            MoveToIndex(tmpIndex);
        }
        else {
            m_dataArrangeNum = dataArrangeNum;
            ResetToBegin();
        }
        useAnimate = olduseAnimate;
    }

    /// <summary>
    /// 通过索引得到Go
    /// </summary>
    public GameObject GetGameObjectByIndex(int index)
    {
        int i = Mathf.CeilToInt(index / (float)maxPerLine);
        int j = index - (i-1) * maxPerLine;
        if (i <= m_items.Count && j <= m_items[i-1].Count)
        {
            LoopGridLuaItem item = m_items[i - 1][j - 1];
            return item.gameObject;
        }
        return null;
    }
    /// <summary>
    /// 隐藏所有item
    /// </summary>
    public void HideAllItem()
    {
        for(int i = 0; i < m_items.Count; i++)
        {
            for(int j = 0;j < m_items[i].Count; j++)
            {
                LoopGridLuaItem item = m_items[i][j];
                item.gameObject.SetActive(false);
            }
        }
    }
    #endregion

    #region menu

    [ContextMenu("Execute")]
    public void Execute() {
        ResetChild();

        CreateScrollView(testCount, null);

        HideAllItem();
    }

    [ContextMenu("Play")]
    private void Play() {
        CreateScrollView(testCount, null);
    }

    [ContextMenu("ResetChild")]
    private void ResetChild() {
        m_itemCOPool.Clear();
        m_itemQueuePool.Clear();
        m_items.Clear();
        if (m_poolGo != null) {
            NGUITools.Destroy(m_poolGo);
        }
        if (m_drag != null) {
            NGUITools.Destroy(m_drag.gameObject);
        }
        m_drag = null;
        m_poolGo = null;

        Transform parent = transform.parent;
        for (int i = parent.childCount - 1; i >= 0; i--) {
            var child = parent.GetChild(i);
            if (child.name == "pool" || child.name == "dragBox") {
                NGUITools.Destroy(child);
            }
        }

        for (int i = transform.childCount - 1, imin = 0; i >= imin; i--) {
            NGUITools.Destroy(transform.GetChild(i).gameObject);
        }
        m_scrollView = null;
        m_panel = null;
        m_initiated = false;

        if (itemTemplate != null)
        {
            itemTemplate.SetActive(true);
        }
    }
    #endregion

    #region private
    private void CalDistance(float distance, out float targetDis, out int index) {
        FixBoxPostion();
        distance = Mathf.Abs(distance);
        targetDis = distance;
        index = Mathf.FloorToInt(targetDis / cellLength);
        ShowArrow(index, distance);
    }
    private void CreateDragBox() {
        GameObject go = new GameObject();
        go.name = "dragBox";
        go.transform.parent = m_scrollView.transform;
        go.transform.localPosition = Vector3.zero;
        go.transform.localScale = Vector3.one;
        go.layer = gameObject.layer;

        UIWidget uw = go.AddComponent<UIWidget>();
        UIDragScrollView ds = go.AddComponent<UIDragScrollView>();
        ds.scrollView = m_scrollView;

        uw.width = 2;
        uw.height = 2;
        uw.depth = -100;
        m_drag = ds;
        InitBoxSize();
    }
    private void InitBoxSize() {
        BoxCollider bx = m_drag.gameObject.GetComponent<BoxCollider>();
        if (bx == null) {
            bx = m_drag.gameObject.AddComponent<BoxCollider>();
        }

        Vector3 size = bx.size;
        size.x = m_panel.width;
        size.y = m_panel.height;
        bx.size = size;
        bx.center = new Vector2(m_halfPanelWidth - cellWidth * 0.5f,
                                -(m_halfPanelHeight - cellHeight * 0.5f));
        FixBoxPostion();
    }
    private void FixBoxPostion() {
        BoxCollider bx = m_drag.GetComponent<BoxCollider>();
        if (!bx) {
            return;
        }
        Vector3 center = m_panel.baseClipRegion;
        center.z = 0;
        Vector3 offset = m_panel.clipOffset;

        center = center + offset;
        center.x = center.x - m_halfPanelWidth + cellWidth * 0.5f;
        center.y = center.y + m_halfPanelHeight - cellHeight * 0.5f;
        m_drag.transform.localPosition = center;
    }
    private void InitPosAndScroll() {
        m_scrollView = transform.parent.GetComponent<UIScrollView>();
        if (m_scrollView == null) Debug.LogException(new Exception("父节点必须有ScrollView这个组件"));
#if UNITY_STANDALONE_WIN
        m_scrollView.momentumAmount = amount / 3;
#else
        m_scrollView.momentumAmount = amount;
#endif
        m_panel = m_scrollView.GetComponent<UIPanel>();
        if (m_panel == null) Debug.LogException(new Exception("父节点必须有UIPanel这个组件"));
        m_halfPanelWidth = m_panel.width / 2;
        m_halfPanelHeight = m_panel.height / 2;

        m_panelInitPos = m_scrollView.transform.localPosition;
        m_initOffset = m_panel.clipOffset;
        m_moveType = m_scrollView.movement;
        EnableDisable();
        if (m_drag == null) {
            CreateDragBox();
        }
        else {
            InitBoxSize();
        }
        if (itemTemplate != null) {
            itemTemplate.SetActive(false);
        }
        CreateItemPool(itemTemplate, 0);

        m_initiated = true;
    }

    private void Clear() {
        m_dataCount = 0;
        m_maxArrangeNum = 0;
        m_maxDistance = 0;
        m_dataArrangeNum = 0;
        m_fillCount = 0;
        m_floorFillCount = 0;
        m_cellHalf = 0;

        m_lastDataIndex = 0;
        m_maxIndex = 0;
        m_minIndex = 0;
    }

    private void ResetPosition() {
        m_scrollView.transform.localPosition = m_panelInitPos;
        m_panel.clipOffset = m_initOffset;
    }

    private void SlotData(int dataCount) {
        m_dataCount = dataCount;
        if (itemTemplate == null) Debug.LogException(new Exception("模板不能为空"));

        m_dataArrangeNum = Mathf.CeilToInt((float)m_dataCount / (float)maxPerLine);

        float len = cellLength;
        float allLen = m_moveType == UIScrollView.Movement.Horizontal ? m_panel.width : m_panel.height;

        m_fillCount = Mathf.CeilToInt(allLen / len);
        m_floorFillCount = Mathf.FloorToInt(allLen / len);
        m_cellHalf = len * 0.5f;

        m_maxArrangeNum = Math.Min(m_dataArrangeNum, m_fillCount + m_cacheNum);
        m_maxDistance = (m_dataArrangeNum - 0.05f) * len - allLen;
#if UNITY_EDITOR
        if (!Application.isPlaying) {
            CreateItemPool(itemTemplate, 0);
        }
#endif
    }

    IEnumerator AddItemsAsync() {
        m_isStartFill = true;
        StoreAllItem();
        StoreQueuePoolItem();
        for (int i = 0; i < m_maxArrangeNum; i++) {
            for (int j = 0; j < maxPerLine; j++) {
                if (i * maxPerLine + j >= m_dataCount) { break; }
                LoopGridLuaItem item = AddOneItem(i, j);
                if (m_items.Count - 1 < i) {
                    m_items.Add(new List<LoopGridLuaItem>());
                }
                m_items[i].Add(item);
                UITweener ut = item.GetComponent<UITweener>();
                if (ut) {
                    ut.ResetToBeginning();
                    ut.PlayForward();
                }
                else {
                    Debug.LogError("请UI同学绑定个Tween动画控件");
                }
                yield return new WaitForSeconds(deltaTime);
            }
        }
        m_isStartFill = false;
        m_timer = null;
    }

    private void AddItems() {
        m_isStartFill = true;
        StoreAllItem();
        for (int i = 0; i < m_maxArrangeNum; i++) {
            for (int j = 0; j < maxPerLine; j++) {
                if (i * maxPerLine + j >= m_dataCount) { break; }

                LoopGridLuaItem item = AddOneItem(i, j);

                if (m_items.Count - 1 < i) {
                    m_items.Add(new List<LoopGridLuaItem>());
                }
                m_items[i].Add(item);
            }
        }
        StoreQueuePoolItem();
        m_isStartFill = false;
    }
    private LoopGridLuaItem AddOneItem(int gridIndex, int lineIndex) {
        GameObject go = GetGridItem();
        var cb = go.GetComponent<UIToggle>();
        if (selectLimit == 1) {
            if (cb != null) {
                cb.group = 1024;
            }
        }
        else if (selectLimit > 1) {
            if (cb != null) {
                cb.group = 0;
            }
        }

        LoopGridLuaItem item = go.GetComponent<LoopGridLuaItem>();
        if (item == null || !item.inited) {
            if (item == null) {
                item = go.AddComponent<LoopGridLuaItem>();
            }
            item.grid = this;
            LuaTable bind = CreateFillTable();
            item.SetFirstItemData(m_dataCount, gridIndex * maxPerLine + lineIndex);
            item.FindItem(bind);
        }

        item.transform.parent = transform;
        item.transform.localScale = itemTemplate.transform.localScale;
        item.gameObject.SetActive(true);
        item.FillItem(gridIndex * maxPerLine + lineIndex, gridIndex, lineIndex);
        return item;
    }
    private void CheckIsNeedArrows() {
        if (upArrow) {
            upArrow.SetActive(false);
            UIEventListener listener = UIEventListener.Get(upArrow);
            listener.onClick = UpArrowClick;
        }
        if (downArrow) {
            downArrow.SetActive(m_dataArrangeNum > m_floorFillCount);
            UIEventListener listener = UIEventListener.Get(downArrow);
            listener.onClick = DownArrowClick;
        }
    }
    private void UpArrowClick(GameObject go) {
        MoveRelativeSmooth(m_lastDataIndex - 1);
    }
    private void DownArrowClick(GameObject go) {
        MoveRelativeSmooth(m_lastDataIndex + 1);
    }
    private void CheckLoopMove() {
        m_panel.onClipMove = null;
        if (m_dataArrangeNum < m_fillCount) {
            m_panel.onClipMove = OnPanelNormalClipMove;
            // m_scrollView.restrictWithinPanel = true;
        }
        else {
            m_lastDataIndex = 0; //上次显示出来的第一个格子，在grid数据中的index
            m_maxIndex = m_maxArrangeNum - 1;
            m_minIndex = 0;

            startPos = m_panel.transform.localPosition;
            endPos = startPos;
            if (m_moveType == UIScrollView.Movement.Vertical) {
                endPos.y = endPos.y + m_dataArrangeNum * cellHeight - m_panel.height;
            }
            else if (m_moveType == UIScrollView.Movement.Horizontal) {
                endPos.x = endPos.x - (m_dataArrangeNum * cellWidth - m_panel.width);
            }

            // m_scrollView.restrictWithinPanel = false;
            //m_scrollView.onPressTrue = CheckIsOutBound;
            m_scrollView.onMomentumMove += KeepInBound;
            m_scrollView.onDragFinished += KeepInBound;

            m_panel.onClipMove += OnPanelLoopClipMove;
        }

    }

    private void CheckDragBack() {
        m_scrollView.onMomentumMove = null;
        m_scrollView.onDragFinished = null;
        // 面板没被占满拖拽回滚
        if (!m_scrollView.disableDragIfFits && m_dataArrangeNum < m_fillCount) {
            m_scrollView.onMomentumMove = OnMoveBack;
            m_scrollView.onDragFinished = OnMoveBack;
        }
    }

    private void MoveGridItem(bool isTopToBottom) {

        List<LoopGridLuaItem> items;
        // 判断是否是 上（左）移动到下（右)
        int curIndex;
        int itemIndex;
        int sign;
        if (isTopToBottom) {
            curIndex = m_maxIndex + 1;
            itemIndex = 0;
            sign = 1;
        }
        else {
            curIndex = m_minIndex - 1;
            itemIndex = m_items.Count - 1;
            sign = -1;
        }

        items = m_items[itemIndex];

        int targetIndex = itemIndex == 0 ? m_items.Count - 1 : 0;

        m_items.Remove(items);
        m_items.Insert(targetIndex, items);

        for (int i = 0; i < items.Count; i++) {
            if (curIndex * maxPerLine + i < 0) {
                break;
            }
            if (curIndex * maxPerLine + i > m_dataCount - 1) {
                break;
            }
            LoopGridLuaItem item = items[i];
            item.FillItem(curIndex * maxPerLine + i, curIndex, i);
        }

        m_minIndex += sign;
        m_maxIndex += sign;
    }

    private void ShowArrow(int index, float distance) {
        if (index == 0 && distance >= m_cellHalf) {
            index = 1;
        }

        if (upArrow) {
            upArrow.gameObject.SetActive(index > 0);
        }

        if (downArrow) {
            downArrow.gameObject.SetActive(m_maxDistance > distance);
        }

    }
    private bool CheckIndexOut() {
        bool result = false;
        for (int i = m_items.Count - 1; i >= 0; i--) {
            List<LoopGridLuaItem> list = m_items[i];
            for (int j = 0, jmax = list.Count; j < jmax; j++) {
                LoopGridLuaItem item = list[j];
                result = item.CheckIndexOutRange();
                if (result) {
                    return true;
                }
            }
        }
        return result;
    }
    #endregion

    #region callback
    private void KeepInBound() {
        float currentDist = 0;
        float startDist = 0;
        float endDist = 0;
        float st = 13f;

        if (m_moveType == UIScrollView.Movement.Vertical) {
            currentDist = m_panel.transform.localPosition.y;
            startDist = startPos.y;
            endDist = endPos.y;
            if (currentDist < startDist) {
                SpringPanel.Begin(m_panel.gameObject, startPos, st).strength = st;
            }
            else if (currentDist > endDist) {
                SpringPanel.Begin(m_panel.gameObject, endPos, st).strength = st;
            }
        }
        else if (m_moveType == UIScrollView.Movement.Horizontal) {
            currentDist = m_panel.transform.localPosition.x;
            startDist = startPos.x;
            endDist = endPos.x;

            if (currentDist > startDist) {
                SpringPanel.Begin(m_panel.gameObject, startPos, st).strength = st;
            }
            else if (currentDist < endDist) {
                SpringPanel.Begin(m_panel.gameObject, endPos, st).strength = st;
            }
        }
    }
    private void OnMoveBack() {
        if (m_panel == null) Debug.LogException(new Exception("父节点必须有UIPanel这个组件"));
        SpringPanel.Begin(m_panel.gameObject, m_panelInitPos, 13f).strength = 8f;
    }
    private void OnPanelNormalClipMove(UIPanel panel) {
        Vector3 delta = m_panelInitPos - panel.transform.localPosition;
        float distance = -1;
        int index;//当前显示出来的第一个格子，在grid数据中的index
        if (m_moveType == UIScrollView.Movement.Vertical) {
            distance = delta.y;
        }
        else if (m_moveType == UIScrollView.Movement.Horizontal) {
            distance = delta.x;
        }
        // 满的时候向上滑不管它
        if (distance > 0 && m_moveType == UIScrollView.Movement.Vertical) return;
        if (distance < 0 && m_moveType == UIScrollView.Movement.Horizontal) return;

        CalDistance(distance, out distance, out index);
    }
    private void OnPanelLoopClipMove(UIPanel panel) {
        if (m_dataCount <= 0) {
            return;
        }
        Vector3 delata = m_panelInitPos - panel.transform.localPosition;
        float distance = -1;

        int index;//当前显示出来的第一个格子，在grid数据中的index
        if (m_moveType == UIScrollView.Movement.Vertical) {
            distance = delata.y;
        }
        else if (m_moveType == UIScrollView.Movement.Horizontal) {
            distance = delata.x;
        }
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
        int sign = Math.Sign(index - m_lastDataIndex);
        int offset = Math.Abs(index - m_lastDataIndex);
        int maxFill = m_fillCount + m_cacheNum;

        if (maxFill < offset) {
            int delta = (offset - maxFill) * sign;
            offset = maxFill;
            m_maxIndex += delta;
            m_minIndex += delta;
        }

        // 判断要把最上（左）的item移动到最下（右）,还是相反
        if (m_lastDataIndex < index) {
            for (int i = 1; i <= offset; i++) {
                //上（左）移动到下（右）
                MoveGridItem(true);
            }
        }
        else {
            for (int i = 1; i <= offset; i++) {
                //上（左）移动到下（右）
                MoveGridItem(false);
            }
        }

        m_lastDataIndex = index;
    }
    #endregion

}