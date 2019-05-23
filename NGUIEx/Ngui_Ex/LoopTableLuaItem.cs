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
* Filename: LoopTableLuaItem.cs
* Created:  2017/11/20 12:21:45
* Author:   To Hard The Mind
* Purpose:  
* ==============================================================================
*/
using System;
using System.Collections.Generic;
using LuaInterface;
using UnityEngine;

public class LoopTableLuaItem : UICore {
    #region member
    protected UILoopTable m_table;
    protected int m_dataCount;
    protected int m_index;
    private bool m_inited = false;

    private UIScrollView.Movement m_moveType;
    private UIScrollView m_scrollView;
    #endregion

    #region property
    public bool inited {
        get {
            return m_inited;
        }
    }
    public UILoopTable table {
        set {
            m_table = value;
            m_scrollView = m_table.transform.parent.GetComponent<UIScrollView>();
            m_moveType = m_scrollView.movement;
        }
        protected get {
            return m_table;
        }
    }
    #endregion

    #region virtual
    public virtual void SetFirstItemData(int dataCount, int index) {
        m_dataCount = dataCount;
        m_index = index;
        m_inited = true;
    }
    public virtual void FindItem() {
        //if (m_bindTable != null) {
        //    Init(m_bindTable);
        //}
    }
    public void FillItem(int index, bool isSetPos) {
        m_index = index;
#if DEBUG
        gameObject.name = index.ToStringNoGC();
#endif
        if (!isSetPos) return;
        switch (m_moveType) { 
            case UIScrollView.Movement.Horizontal:
                SetHorizonPosition();
                break;
            case UIScrollView.Movement.Vertical:
                SetVerticalPosition();
                break;
        }
    }
    private void SetHorizonPosition() {
        float distance = m_table.GetDistanceByIndex(m_index);
        transform.localPosition = new Vector3(distance, 0, 0);
    }
    private void SetVerticalPosition() {
        float distance = m_table.GetDistanceByIndex(m_index);
        transform.localPosition = new Vector3(0, -distance, 0);
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
        FillItem(m_index, true);
    }
    #endregion

    #region private
    private bool IsIndexOut() {
        return m_dataCount < m_index;
    }
    #endregion

}