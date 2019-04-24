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
* Filename: DisableItem.cs
* Created:  2018/1/23 20:05:43
* Author:   To Hard The Mind
* Purpose:  
* ==============================================================================
*/
using UnityEngine;

public class DisableItem : MonoBehaviour {
    protected bool isActived = false;
    protected void EnableDisable() {
        if (isActived) return;
        if (!gameObject.activeInHierarchy) {
            int index = transform.GetSiblingIndex();
            Transform p = transform.parent;
            transform.SetParent(null, false);
            bool active = gameObject.activeSelf;
            gameObject.SetActive(true);
            gameObject.SetActive(active);
            transform.SetParent(p, false);
            transform.SetSiblingIndex(index);
        }
        isActived = true;
    }
}