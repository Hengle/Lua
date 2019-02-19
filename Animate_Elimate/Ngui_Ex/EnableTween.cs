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
* Filename: EnableTween.cs
* Created:  2018/4/19 21:23:21
* Author:   To Hard The Mind
* Purpose:  
* ==============================================================================
*/
using UnityEngine;
using System.Collections.Generic;

public class EnableTween : MonoBehaviour {
    private Dictionary<UITweener, int> uts = new Dictionary<UITweener, int>();

    void Awake()
    {
        var tempUts = GetComponents<UITweener>();
        if (tempUts != null)
        {
            foreach (UITweener ut in tempUts)
            {
                uts[ut] = -1;
                ut.AddOnFinished(()=> {
                    uts[ut] = 0;
                });
            }
        }
    }

    void LateUpdate()
    {
        List<UITweener> tempTws = new List<UITweener>(uts.Keys);
        foreach (UITweener ut in tempTws)
        {
            if (uts[ut] != -1)
            {
                uts[ut] += 1;
                if (uts[ut] > 3)
                {
                    uts[ut] = -1;
                    ut.transform.localScale = new Vector3(0.9999f, 1, 1);
                }
            }
        }
    }

    void OnEnable() {
        List<UITweener> tempTws = new List<UITweener>(uts.Keys);
        foreach (UITweener ut in tempTws)
        {
            uts[ut] = -1;
            ut.ResetToBeginning();
            ut.PlayForward();
        }
    }
}