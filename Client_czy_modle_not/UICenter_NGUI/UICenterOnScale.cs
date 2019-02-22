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
* Filename: UICenterOnScale.cs
* Created:  2018/4/3 17:38:19
* Author:   To Hard The Mind
* Purpose:  
* ==============================================================================
*/
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Ever wanted to be able to auto-center on an object within a draggable panel?
/// Attach this script to the container that has the objects to center on as its children.
/// </summary>

[AddComponentMenu("NGUI/Interaction/Center Scroll View on Scale")]
public class UICenterOnScale : MonoBehaviour {
    [Range(0.5f, 2)]
    public float scaleRange = 1.3f;
    [Range(1, 2)]
    public float centerScale = 1.5f;

    private UICenterOnChild mCenterChild;
    private UIScrollView mScrollView;
    private float mDelta;
    private Vector3 mPanelCenter = Vector3.zero;
    private Transform mCenterGo;
    void Start() {
        if (mScrollView == null) {
            mScrollView = NGUITools.FindInParents<UIScrollView>(gameObject);

            if (mScrollView == null) {
                Debug.LogWarning(GetType() + " requires " + typeof(UIScrollView) + " on a parent object in order to work", this);
                enabled = false;
                return;
            }
            // Calculate the panel's center in world coordinates
            Vector3[] corners = mScrollView.panel.worldCorners;
            Vector3 panelCenter = (corners[2] + corners[0]) * 0.5f;
            mPanelCenter = panelCenter;
            mCenterChild = GetComponent<UICenterOnChild>();
            UILoopGrid loop = GetComponent<UILoopGrid>();
            float cellWidth = 0;
            if (loop != null) {
                cellWidth = loop.cellWidth;
            }
            else {
                UIGrid grid = GetComponent<UIGrid>();
                if (grid == null) {
                    enabled = false;
                    return;
                }
                cellWidth = grid.cellWidth;
            }

            float delta = cellWidth * scaleRange;
            mDelta = delta * delta;
        }
    }
    void Update() {
        Transform trans = transform;
        if (trans.childCount == 0) return;

        Vector3 pickingPoint = trans.worldToLocalMatrix.MultiplyPoint(mPanelCenter);

        // Determine the closest child
        for (int i = 0, imax = trans.childCount; i < imax; ++i) {
            Transform t = trans.GetChild(i);
            if (!t.gameObject.activeInHierarchy) continue;
            float sqrDist = Vector3.SqrMagnitude(t.localPosition - pickingPoint);
            float value = Mathf.Clamp01(sqrDist / mDelta);
            float scale = Mathf.Lerp(centerScale, 1.0f, value);

            if (1 - value > 0.99f && mCenterChild) {
                if (t != mCenterGo) {
                    if (mCenterChild.onCenter != null) {
                        mCenterChild.onCenter(t.gameObject);
                    }
                    mCenterGo = t;
                }
            }
            t.localScale = Vector3.one * scale;
            //t.gameObject.SetActive(false);
            //t.gameObject.SetActive(true);
        }
    }
}