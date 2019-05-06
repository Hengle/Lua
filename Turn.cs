using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;

public class Turn : MonoBehaviour,IDragHandler,IBeginDragHandler,IEndDragHandler
{
    private bool IsMove { get; set; }

    private Vector3 startPos;

    private Vector3 startTemp;

    public delegate void CallBackTurn(float val);

    public CallBackTurn OnTurn; //旋转方向回调

    public delegate void CallBackAngle(float angle);

    public CallBackAngle OnAngle; //旋转的角度(0-1之间)

    public delegate void EndDragTurn();

    public EndDragTurn OnEndDragTurn;

    public delegate void CallBackDragBeginObj(GameObject obj);

    public CallBackDragBeginObj OnDragBeginObj; //此刻正在拖动的物体

    public delegate void CallBackDragEndObj(GameObject obj);

    public CallBackDragEndObj OnDragEndObj;//拖动结束的物体

    public delegate void CallBackDragEulerAngel(Vector3 axis,float angle); 

    public CallBackDragEulerAngel OnDragEulerAngel;//旋转的欧拉角的差值


    public bool IsCanMoveAntiClock { get; set; }//是否能逆时针运动

    private void Awake()
    {
        IsMove = false;
        IsCanMoveAntiClock = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!IsMove) return;

        Vector3 pos = GetMousePosToWorldPos();
        float dir = GetDirection();


        if (IsCanMoveAntiClock)
        {
            if(1 == dir) //-1是逆时针 1是顺时针
            {
                startPos = pos;
                return;
            }
        }

        //UnityEngine.Debug.Log("Dir:" + dir + "    IsAntiClock:" + IsCanMoveAntiClock);

        float angle = Vector3.Angle(startPos - this.transform.position, pos - transform.position);
        this.transform.Rotate(Vector3.forward, angle * dir);
        startPos = pos;

        float fonlyfromstart = Vector3.Angle(startTemp - this.transform.position, pos - transform.position);

        if (OnDragEulerAngel != null)
        {
            OnDragEulerAngel(Vector3.forward, angle * dir);
        }


        if (OnTurn != null)
        {
            OnTurn(dir);

        }

        if (OnAngle != null)
        {
            OnAngle(angle);
        }  
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        IsMove = true;
        startPos = GetMousePosToWorldPos();
        startTemp = GetMousePosToWorldPos();


        GameObject objTemp = eventData.pointerPressRaycast.gameObject;
        if (objTemp != null)
        {
            if (OnDragBeginObj!= null)
            {
                OnDragBeginObj(objTemp);
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        IsMove = false;

        if (OnEndDragTurn != null)
        {
            OnEndDragTurn();
        }


        GameObject objTemp = eventData.pointerPressRaycast.gameObject;
        if (objTemp != null)
        {
            if(OnDragEndObj != null)
            {
                OnDragEndObj(objTemp);
            }
        }
    }


    private Vector3 GetMousePosToWorldPos()
    {
        Vector3 mouspos = Input.mousePosition;
        mouspos.z = Camera.main.WorldToScreenPoint(transform.position).z;//得到transform的屏幕坐标
        return Camera.main.ScreenToWorldPoint(mouspos);//屏幕位置转世界坐标
    }

    private float GetDirection()
    {
        Vector3 dir = startPos - this.transform.position;
        Vector3 off = GetMousePosToWorldPos() - startPos;

        //float angle = GetAngle(off, Vector3.up);
        if(dir.y > 0 && dir.x >0)
        {
            return CheckUp(dir, off) ? 1 : -1; 
        }
        else if(dir.y >0 && dir.x <0)
        {
            return CheckUp(dir, off) ? -1: 1;
        }
        else if (dir.y < 0 && dir.x < 0)
        {
            return CheckUp(dir, off) ? -1 : 1;
        }
        else if (dir.y < 0 && dir.x > 0)
        {
            return CheckUp(dir, off) ? 1 : -1;
        }

        return 1;
    }

    private bool CheckUp(Vector3 dir,Vector3 source)
    {
        float k = dir.y / dir.x;
        if(k * source.x < source.y)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private float GetAngle(Vector3 a,Vector3 b)
    {
        if(a.x < b.x)
        {
            return 360 - Vector3.Angle(a, b);
        }
        else
        {
            return Vector3.Angle(a, b);
        }
    }
}
