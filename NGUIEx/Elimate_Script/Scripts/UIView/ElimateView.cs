using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElimateView : MonoBehaviour
{
    [SerializeField] GameObject objIn;
    [SerializeField] GameObject objOut;


    private bool m_select;

    public bool IsSelected
    {
        get
        {
            return m_select;
        }

        set
        {
            m_select = value;

            //refresh view
            RefreshSelectView();
        }
    }


    public GameObject ViewIn
    {
        get
        {
            return objIn;
        }
        private set
        {

        }
    }


    public GameObject ViewOut
    {
        get
        {
            return objOut;
        }
        private set
        {

        }
    }



    private void Awake()
    {
        IsSelected = false;
    }


    private void RefreshSelectView()
    {
        objIn.SetActive(!m_select);
        objOut.SetActive(m_select);
    }
}
