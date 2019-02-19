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
* Filename: TweenNumber.cs
* Created:  2018/5/10 11:10:25
* Author:   To Hard The Mind
* Purpose:  
* ==============================================================================
*/
using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UILabel))]
public class TweenNumber : UITweener {
    public int from = 0;
    public int to = 0;
    public int value { private set; get; }
    UILabel mLabel;

    /// <summary>
    /// Tween the value.
    /// </summary>

    protected override void OnUpdate(float factor, bool isFinished) {
        if (!mLabel) {
            mLabel = GetComponent<UILabel>();
        }
        value = (int)Mathf.Lerp(from, to, factor);
        mLabel.text = value.ToString();
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