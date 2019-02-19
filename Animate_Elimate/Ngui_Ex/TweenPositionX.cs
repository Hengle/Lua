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
* Filename: TweenPositionX.cs
* Created:  2018/5/10 11:10:25
* Author:   To Hard The Mind
* Purpose:  
* ==============================================================================
*/
using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("NGUI/Tween/Tween PositionX")]
public class TweenPositionX : UITweener {
    public int fromX = 0;
    public int toX = 0;
    public int value { private set; get; }

    /// <summary>
    /// Tween the value.
    /// </summary>

    protected override void OnUpdate(float factor, bool isFinished) {
        value = (int)Mathf.Lerp(fromX, toX, factor);
        Vector3 pos = transform.localPosition;
        pos.x = value;
        transform.localPosition = pos;
    }

    /// <summary>
    /// Start the tweening operation.
    /// </summary>
    static public TweenNumber Begin(GameObject go, float duration,int from, int to) {
        TweenNumber comp = UITweener.Begin<TweenNumber>(go, duration);
        comp.from = from;
        comp.to = to;

        if (duration <= 0f) {
            comp.Sample(1f, true);
            comp.enabled = false;
        }
        return comp;
    }
    [ContextMenu("Execute")]
    private void Execute() {
        gameObject.SetActive(true);
        UITweener[] uts = GetComponents<UITweener>();
        for (int i = 0, imax = uts.Length; i < imax; i++) {
            uts[i].ResetToBeginning();
        }
        PlayForward();
    }
}