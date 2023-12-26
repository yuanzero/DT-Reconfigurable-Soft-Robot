using UnityEngine;

public class ManipulateAndFixDistance : MonoBehaviour
{
    public Transform parentObject; // 父物体
    public Transform childObject; // 子物体
    private bool isManipulating = false; // 用于标记是否正在进行manipulate
    private Vector3 initialOffset; // 用于保存子物体相对于父物体的初始偏移量
    private Quaternion initialOrientation; // 用于保存子物体相对于父物体的初始旋转角度

    void Start()
    {
        // 在开始时计算子物体相对于父物体的初始偏移量和旋转角度
        initialOffset = childObject.position - parentObject.position;
        initialOrientation = Quaternion.Inverse(parentObject.rotation) * childObject.rotation;
    }

    void Update()
    {
        if (!isManipulating)
        {
            // 检查父物体和子物体之间的距离和旋转角度是否发生变化
            if (Vector3.Distance(parentObject.position, childObject.position) != initialOffset.magnitude)
            {
                // 如果不是在manipulate状态下，且距离发生变化，则重新计算子物体的位置，使其与父物体的距禒保持不变
                Vector3 newPosition = parentObject.position + initialOffset.normalized * initialOffset.magnitude;
                childObject.position = newPosition;
            }
            if (Quaternion.Angle(parentObject.rotation, childObject.rotation) != Quaternion.Angle(parentObject.rotation, initialOrientation))
            {
                // 如果不是在manipulate状态下，且旋转角度发生变化，则重新计算子物体的旋转角度，使其与父物体的旋转角度保持不变
                Quaternion newRotation = parentObject.rotation * initialOrientation;
                childObject.rotation = newRotation;
            }
        }
    }

    // 开始manipulate时调用
    public void StartManipulate()
    {
        isManipulating = true;
        // 在这里可以添加取消约束关系的逻辑
    }

    // 结束manipulate时调用
    public void EndManipulate()
    {
        isManipulating = false;
        // 在结束manipulate时更新initialOffset和initialOrientation为现有的距离和旋转角度
        initialOffset = childObject.position - parentObject.position;
        initialOrientation = Quaternion.Inverse(parentObject.rotation) * childObject.rotation;
    }
}