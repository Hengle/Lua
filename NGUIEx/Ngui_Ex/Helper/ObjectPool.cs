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
* Filename: ObjectPool
* Created:  2017/4/9 19:03:34
* Author:   HaYaShi ToShiTaKa
* Purpose:  
* ==============================================================================
*/
using System;
using System.Collections.Generic;

public class ObjectPool<T> where T : class {
    public delegate T CreateFunc();

    public ObjectPool() {

    }
    public ObjectPool(int poolSize, CreateFunc createFunc, Action<T> resetAction) {
        Init(poolSize, createFunc, resetAction);
    }
    public T GetObject() {
        if (m_objStack.Count > 0) {
            T t = m_objStack.Pop();
            return t;
        }
        if (m_createFunc == null) {
            throw new NullReferenceException("create function can't be null");
        }
        return m_createFunc();
    }

    public void Init(int poolSize, CreateFunc createFunc, Action<T> resetAction) {
        m_objStack = new Stack<T>();
        m_resetAction = resetAction;
        m_createFunc = createFunc;

        if (m_createFunc == null) {
            throw new NullReferenceException("create function can't be null");
        }
        if (resetAction == null) {
            throw new NullReferenceException("reset function can't be null");
        }

        for (int i = 0; i < poolSize; i++) {
            T item = m_createFunc();
            m_objStack.Push(item);
        }
    }

    public void Store(T obj) {
        if (obj == null)
            return;
        if (m_resetAction != null)
            m_resetAction(obj);
        m_objStack.Push(obj);
    }

    // 少用，调用这个池的作用就没有了
    public void Clear() {
        if (m_objStack != null)
            m_objStack.Clear();
    }

    public int Count {
        get {
            if (m_objStack == null)
                return 0;
            return m_objStack.Count;
        }
    }

    public Stack<T>.Enumerator GetEnumerator() {
        if (m_objStack == null)
            return new Stack<T>.Enumerator();
        return m_objStack.GetEnumerator();
    }

    private Stack<T> m_objStack = null;
    private Action<T> m_resetAction = null;
    private CreateFunc m_createFunc = null;
}