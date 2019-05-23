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
* Filename: NGUIMenuEx
* Created:  2017/4/9 19:11:24
* Author:   HaYaShi ToShiTaKa
* Purpose:  
* ==============================================================================
*/
using UnityEditor;

public static class NGUIMenuEx {
    [MenuItem("NGUI/Create/LoopGrid &#g", false, 6)]
    static void AddLoopGrid() {
        NGUIMenu.Add<UILoopGrid>();
    }

    [MenuItem("NGUI/Create/PolygonBar", false, 7)]
    static void AddPolygonBar() {
        UIPolygonBar bar = NGUIMenu.Add<UIPolygonBar>();
        bar.ReframeMeshs();
    }

    [MenuItem("NGUI/Create/Dynamic Sprite", false, 8)]
    static void AddMikuSprite() {
        NGUIMenu.Add<Miku.UI.MikuSprite>();
    }
}