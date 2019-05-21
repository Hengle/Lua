using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CustomFly))]
public class FlyTest : MonoBehaviour {
    public Transform TargetTrans;

    [ContextMenu("Fly")]
    public void Fly()
    {
        CustomFly FlyCom = gameObject.GetComponent<CustomFly>();
        FlyCom.Fly(TargetTrans.position);
    }
}
