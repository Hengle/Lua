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
                我们的未来没有BUG              
* ==============================================================================
* Filename: LoopGridLuaItem
* Created:  2017/3/16 15:49:59
* Author:   HaYaShi ToShiTaKa
* Purpose:  
* ==============================================================================
*/
using UnityEngine;
using LuaInterface;
using System.Collections.Generic;
using SimpleFramework;

public class LoopGridLuaItem : UICore {

    #region member
    protected UILoopGrid m_grid;
    protected LuaTable m_bindTable;
    protected int m_dataCount;
    protected int m_index;
    private bool m_inited = false;
    private UIToggle m_toggle;

    private UIScrollView.Movement m_moveType;
    private UIScrollView m_scrollView;

    private List<LoopGridLuaItem> m_childItems;
    #endregion

    #region property
    public bool inited {
        get {
            return m_inited;
        }
    }
    public int index {
        get {
            return m_index;
        }
    }
    public int gridIndex { private set; get; }
    public int lineIndex { private set; get; }
    public UILoopGrid grid {
        set {
            m_grid = value;
            m_scrollView = m_grid.transform.parent.GetComponent<UIScrollView>();
            m_moveType = m_scrollView.movement;
        }
        get {
            return m_grid;
        }
    }
    public UIToggle toggle {
        get {
            return m_toggle;
        }
    }
    #endregion

    #region virtual
    public virtual void SetFirstItemData(int dataCount, int index) {
        m_dataCount = dataCount;
        m_index = index;
        m_inited = true;
    }
    public virtual void FindItem(LuaTable bind) {
        DoInitUICore(bind);
        BindChild();
        DoFindItem();
    }
    public void FillItem(int index, int gridIndex, int lineIndex) {
        m_index = index;
        this.gridIndex = gridIndex;
        this.lineIndex = lineIndex;
        if (m_moveType == UIScrollView.Movement.Horizontal) {
            transform.localPosition = new Vector3(m_grid.cellWidth * gridIndex, -m_grid.cellHeight * lineIndex, 0);
        }
        else if (m_moveType == UIScrollView.Movement.Vertical) {
            transform.localPosition = new Vector3(m_grid.cellWidth * lineIndex, -m_grid.cellHeight * gridIndex, 0);
        }
        DoCallLuaFill(false);
        DoCallChildFill();
        DoClickToggle();
    }
    public void FillChildItem(int itemIndex, int gridIndex, int dataIndex) {
        m_index = dataIndex;
        DoCallLuaFill(true);
        DoClickToggle();
    }

    #endregion

    #region public api
    public bool CheckIndexOutRange() {
        bool result = true;
        do {
            if (m_dataCount <= 0) break;
            if (IsIndexOut()) break;
            result = false;
        } while (false);

        return result;
    }
    public void UpdateItem() {
        FillItem(m_index, gridIndex, lineIndex);
    }
    #endregion

    #region private
    private void DoInitUICore(LuaTable bind) {
        m_bindTable = bind;
        if (m_bindTable == null)
        {
            if (GameLogger.IsEnable) GameLogger.Error("m_bindTable is nil");
            return;
        }
        Init(m_bindTable);
        m_bindTable.Set("_grid", m_grid);
        m_bindTable.Set("_core", this);
    }
    private void DoCallChildFill() {
        if (m_childItems == null) return;
        for (int i = 0, imax = m_childItems.Count; i < imax; i++) {
            LoopGridLuaItem item = m_childItems[i];
            item.FillChildItem(m_index , i, m_index * imax + i);
        }

    }
    private void DoCallLuaFill(bool isChild) {
        if (m_bindTable == null) return;
        LuaFunction func = m_bindTable.getTable<LuaFunction>("FillItem");

        if (func != null) {
            var L = LuaScriptMgr.Instance.lua.L;
            int oldTop = func.BeginPCall();
            m_bindTable.push(L);
            int luaIndex = m_index + 1;// lua那边 index 比 c# 大1
            LuaScriptMgr.Push(L, luaIndex);

            if (m_grid.dataTable != null) {
                m_grid.dataTable.push(L);
            }
            else {
                LuaDLL.lua_pushnil(L);
            }
            if (func.PCall(oldTop, 3)) {
                func.EndPCall(oldTop);
            }
        }
        else {
            throw new LuaException("this table ");
        }
    }
    private void DoClickToggle() {
        if (m_toggle == null) return;

        if (m_toggle.group != 0) {
            m_toggle.Set(m_index == m_grid.selectIndex);
        }
        else {
            m_toggle.Set(m_grid.ContainsMultiSelect(m_index));
        }
    }
    private void DoFindItem() {
        m_toggle = gameObject.GetComponent<UIToggle>();

        if (m_toggle != null) {
            EventDelegate.Add(m_toggle.onChange, ToggleClick);
        }

        if (m_bindTable == null) return;
        LuaFunction func = m_bindTable.getTable<LuaFunction>("BindUICore");

        if (func != null) {
            var L = LuaScriptMgr.Instance.lua.L;
            int oldTop = func.BeginPCall();
            m_bindTable.push(L);
            if (func.PCall(oldTop, 1)) {
                func.EndPCall(oldTop);
            }
        }
    }
    private void BindChild() {
        if (!m_grid.hasChild)
            return;
        if (m_childItems == null) {
            m_childItems = new List<LoopGridLuaItem>();
        }
        else {
            m_childItems.Clear();
        }
        gameObject.GetComponentsInChildren<LoopGridLuaItem>(true, m_childItems);
        m_childItems.Remove(this);

        for (int i = 0, imax = m_childItems.Count; i < imax; i++) {
            LoopGridLuaItem item = m_childItems[i];
            item.grid = m_grid;
            LuaTable childBind = m_grid.CreateChildFillTable();
            item.DoInitUICore(childBind);
            item.DoFindItem();
        }
    }
    private void ToggleClick() {
        if (m_toggle == null) return;
        if (m_toggle.value) {
            if (m_toggle.group == 0) {
                if (m_grid.isStartFill) return;
                if (m_grid.CanAddMultiSelect()) {
                    m_grid.AddMultiSelect(m_index);
                }
                else {
                    m_toggle.Set(false);
                }
            }
            else {
                m_grid.selectIndex = m_index;
            }
        }
        else if (m_toggle.group == 0) {
            m_grid.RemoveMultiSelect(m_index);
        }
        else if(m_toggle.optionCanBeNone) {
            m_grid.selectIndex = -1;
        }
    }
    private bool IsIndexOut() {
        return m_dataCount < m_index;
    }
    #endregion

}