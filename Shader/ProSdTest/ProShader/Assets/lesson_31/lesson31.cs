using UnityEngine;
using System.Collections;

public class lesson31 : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        //旋转
        Matrix4x4 RM = new Matrix4x4();
        RM[0, 0] = Mathf.Cos(Time.realtimeSinceStartup);
        RM[0, 2] = Mathf.Sin(Time.realtimeSinceStartup);
        RM[1, 1] = 1;
        RM[2, 0] = -Mathf.Sin(Time.realtimeSinceStartup);
        RM[2, 2] = Mathf.Cos(Time.realtimeSinceStartup);
        RM[3, 3] = 1;

        //缩放
        Matrix4x4 SM = new Matrix4x4();
        SM[0, 0] = Mathf.Sin(Time.realtimeSinceStartup);
        SM[1, 1] = Mathf.Cos(Time.realtimeSinceStartup);
        SM[2, 2] = Mathf.Sin(Time.realtimeSinceStartup);
        SM[3, 3] = 1;

        //模拟mvp 模型-视图-投影
        Matrix4x4 mvp = Camera.main.projectionMatrix *
            Camera.main.worldToCameraMatrix*
            transform.localToWorldMatrix*
            RM;


        this.GetComponent<Renderer>().material.SetMatrix("mvp", mvp);
        this.GetComponent<Renderer>().material.SetMatrix("rm", RM);
        this.GetComponent<Renderer>().material.SetMatrix("sm", SM);

    }
}
