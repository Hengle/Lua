using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MonoSingleTon<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T m_instance;

    public static T Instance
    {
        get
        {
            return m_instance;
        }
    }

    protected virtual void Awake()
    {
        m_instance = this as T;
    }
}


