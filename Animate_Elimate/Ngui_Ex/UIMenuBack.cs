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
* Filename: UIMenuBack.cs
* Created:  2018/4/19 19:12:51
* Author:   To Hard The Mind
* Purpose:  控制下拉 菜单背景框大小
* ==============================================================================
*/
using UnityEngine;
public class UIMenuBack : MonoBehaviour {
    public int maxNum = 1;
    public float cellHeight;
    public UISprite back;
    public Transform target;
    public int offset = 0;

    [ContextMenu("Execute")]
    void Resize() {
        if (!back) return;
        back.height = (int)(Mathf.Min(maxNum, target.childCount) * cellHeight) + offset;
    }

    void Update() {
        Resize();
    }

#if UNITY_EDITOR
    void OnValidate() {
        if (!Application.isPlaying && NGUITools.GetActive(this)) {
            Resize();
        }
    }
#endif

}