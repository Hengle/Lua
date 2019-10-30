using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class CustomFly : MonoBehaviour {
    private Vector3 _sourecePos;            // 开始运动位置
    private Vector3 _targetPos;             // 目标位置
    private float _surpFlyTime = 0;         // 剩余飞行时间
    private float _offsetUnit = 0;          // 偏移单位(起点到终点的距离)

    public float FlyTime = 1;               // 总飞行时间
    public AnimationCurve TimeCurve = AnimationCurve.Linear(0, 0, 1, 1);        // 时间曲线 控制动画的快慢
    public AnimationCurve OffsetXCurve = AnimationCurve.Linear(0, 0, 1, 0);     // x轴随时间偏移曲线
    public AnimationCurve OffsetYCurve = AnimationCurve.Linear(0, 0, 1, 0);     // y轴随时间偏移曲线
    public AnimationCurve OffsetZCurve = AnimationCurve.Linear(0, 0, 1, 0);     // z轴随时间偏移曲线

    public Vector3 TargetVec
    {
        set
        {
            _targetPos = value;
        }
    }

    public Vector3 SourecePos
    {
        get
        {
            return _sourecePos;
        }
        set
        {
            _sourecePos = value;
        }
    }

    /// <summary>
    /// X轴随时间偏移
    /// </summary>
    /// <returns></returns>
    private float GetXOffset()
    {
        return OffsetXCurve.Evaluate(GetTimeOffset()) * _offsetUnit;
    }
    /// <summary>
    /// Y轴随时间偏移
    /// </summary>
    /// <returns></returns>
    private float GetYOffset()
    {
        return OffsetYCurve.Evaluate(GetTimeOffset()) * _offsetUnit;
    }
    /// <summary>
    /// Z轴随时间偏移
    /// </summary>
    /// <returns></returns>
    private float GetZOffset()
    {
        return OffsetZCurve.Evaluate(GetTimeOffset()) * _offsetUnit;
    }
    private float GetTimeOffset()
    {
        return TimeCurve.Evaluate(GetTime());
    }

    /// <summary>
    /// 已经运行时间 归一化了
    /// </summary>
    /// <returns></returns>
    private float GetTime()
    {
        float time = 0;
        if (FlyTime == 0) time = 1;
        else time = 1 - _surpFlyTime/ FlyTime;
        return time;
    }

    private Vector3 CaluPos()
    {
        Vector3 caluPos = Vector3.zero;

        // 建立坐标系
        Vector3 curPos = Vector3.Lerp(_sourecePos, _targetPos, GetTimeOffset());
        Vector3 vx = Vector3.Normalize((_targetPos - _sourecePos));
        Vector3 vz = Vector3.up;//Vector3.forward; // (0,0,1)
        Vector3 vy = -Vector3.Normalize(Vector3.Cross(vx, vz)); // 右手坐标转换成左手坐标(unity坐标系)
        Matrix4x4 transMatrix = new Matrix4x4
        {
            m00 = vx.x,m01 = vy.x,m02 = vz.x,m03 = curPos.x,
            m10 = vx.y,m11 = vy.y,m12 = vz.y,m13 = curPos.y,
            m20 = vx.z, m21 = vy.z,m22 = vz.z,m23 = curPos.z,
            m30 = 0,m31 = 0,m32 = 0,m33 = 1f
        };

        // 新坐标系的本地坐标
        float offsetX = GetXOffset();
        float offsetY = GetYOffset();
        float offsetZ = GetZOffset();

        // 坐标系转换
        caluPos = transMatrix * new Vector4(offsetX, offsetY, offsetZ, 1);

        Debug.Log("offx--" + offsetX + "--offy--" + offsetY + "--offz--" + offsetZ + "--x--" + caluPos.x + "--y--" + caluPos.y + "--z--" + caluPos.z);

        return caluPos;
    }

    private void Awake()
    {
        _sourecePos = transform.position;
    }

    public void Fly(Vector3 targetPos)
    {
        _surpFlyTime = FlyTime;
        transform.position = _sourecePos;
        _targetPos = targetPos;

        if (_offsetUnit == 0)
            _offsetUnit = (targetPos - _sourecePos).magnitude;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (_targetPos == null)
        {
            return;
        }

        if (_surpFlyTime <= 0)
            return;
        _surpFlyTime -= Time.deltaTime;

        transform.position = CaluPos();
	}
}
