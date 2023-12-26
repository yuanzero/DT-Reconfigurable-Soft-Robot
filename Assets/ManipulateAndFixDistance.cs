using UnityEngine;

public class ManipulateAndFixDistance : MonoBehaviour
{
    public Transform parentObject; // ������
    public Transform childObject; // ������
    private bool isManipulating = false; // ���ڱ���Ƿ����ڽ���manipulate
    private Vector3 initialOffset; // ���ڱ�������������ڸ�����ĳ�ʼƫ����
    private Quaternion initialOrientation; // ���ڱ�������������ڸ�����ĳ�ʼ��ת�Ƕ�

    void Start()
    {
        // �ڿ�ʼʱ��������������ڸ�����ĳ�ʼƫ��������ת�Ƕ�
        initialOffset = childObject.position - parentObject.position;
        initialOrientation = Quaternion.Inverse(parentObject.rotation) * childObject.rotation;
    }

    void Update()
    {
        if (!isManipulating)
        {
            // ��鸸�����������֮��ľ������ת�Ƕ��Ƿ����仯
            if (Vector3.Distance(parentObject.position, childObject.position) != initialOffset.magnitude)
            {
                // ���������manipulate״̬�£��Ҿ��뷢���仯�������¼����������λ�ã�ʹ���븸����ľൠ���ֲ���
                Vector3 newPosition = parentObject.position + initialOffset.normalized * initialOffset.magnitude;
                childObject.position = newPosition;
            }
            if (Quaternion.Angle(parentObject.rotation, childObject.rotation) != Quaternion.Angle(parentObject.rotation, initialOrientation))
            {
                // ���������manipulate״̬�£�����ת�Ƕȷ����仯�������¼������������ת�Ƕȣ�ʹ���븸�������ת�Ƕȱ��ֲ���
                Quaternion newRotation = parentObject.rotation * initialOrientation;
                childObject.rotation = newRotation;
            }
        }
    }

    // ��ʼmanipulateʱ����
    public void StartManipulate()
    {
        isManipulating = true;
        // ������������ȡ��Լ����ϵ���߼�
    }

    // ����manipulateʱ����
    public void EndManipulate()
    {
        isManipulating = false;
        // �ڽ���manipulateʱ����initialOffset��initialOrientationΪ���еľ������ת�Ƕ�
        initialOffset = childObject.position - parentObject.position;
        initialOrientation = Quaternion.Inverse(parentObject.rotation) * childObject.rotation;
    }
}