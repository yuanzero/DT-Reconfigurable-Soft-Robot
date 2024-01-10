using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;

public class PIDController : MonoBehaviour
{
    public CubeStretchMover cubeStretchMover; // �û�ָ���Ĵ������ű�
    public Handle_canvas handle_Canvas; // �û�ָ�����������ƽű�

    public float Kp = 1.0f; // ��������
    public float Ki = 0.1f; // ��������
    public float Kd = 0.01f; // ΢������

    private float integral = 0.0f;
    private float lastError = 0.0f;

    // define the pin
    public string stretch_module_actuation;
    public string stretch_module_air;
    public string bending_module_actuation;
    public string bending_module_air;
    public string twist_module_actuation;
    public string twist_module_air;

    // ����������ֵ
    public float T_sensor;

    public float T_goal = 1.0f; // Ŀ��ֵ

    public PinchSlider pinchSlider; // ����user����

    void Update()
    {
        // ��SensorScript��ȡT_sensor��ֵ
        T_sensor = cubeStretchMover.GetSensorValue();
        T_goal = pinchSlider.SliderValue;

        float error = T_goal - T_sensor;

        integral += error * Time.deltaTime;
        float derivative = (error - lastError) / Time.deltaTime;

        float control = Kp * error + Ki * integral + Kd * derivative;

        // Ӧ�ÿ���������������¶Ȼ�λ��
        ApplyControl(control);

        lastError = error;
    }

    void ApplyControl(float control)
    {
        float epsilon = 0.1f; // ����һ����ֵepsilon

        if (control > 0) // ������������0ʱ
        {
            // ����ѹ��PinΪ��ֵ�����Ӹ�ѹ���������
            SetPressure(1.0f); // ��������Ϊ1.0��ʾ��ѹ
        }
        else if (control < 0) // ��������С��0ʱ
        {
            // ����ѹ��PinΪ��ֵ�����ӿ���
            SetPressure(-1.0f); // ��������Ϊ-1.0��ʾ��ѹ
        }
        else if (Mathf.Abs(control) < epsilon) // ���������ľ���ֵС��epsilonʱ
        {
            // ����ѹ��PinΪ�㣬�ر����з����Ա��ֱ���
            SetPressure(0.0f); // ����Ϊ0��ʾ�رշ���
        }
    }

    void SetPressure(float pressure)
    {
        // ������ʵ������ѹ�����߼����������ӵ���ѹ��������ջ�رշ���
        // ����Ҫ���ݾ����ϵͳ��ʵ������ѹ�����߼�
        // ���磺ͨ�����Ʒ��Ż������豸������ѹ��
        if (pressure > 0)
        {
            activation(stretch_module_actuation, stretch_module_air);

        }
        else if (pressure < 0)
        {
            release_air(stretch_module_actuation, stretch_module_air);
        }
        else if (pressure == 0)
        {
            hoding_state(stretch_module_actuation, stretch_module_air);
        }
    }

    void activation(string pin1, string pin2)
    {
        handle_Canvas.send_ESP32_HIGH(pin1);
        handle_Canvas.send_ESP32_LOW(pin2);
    }

    void release_air(string pin1, string pin2)
    {
        handle_Canvas.send_ESP32_LOW(pin1);
        handle_Canvas.send_ESP32_HIGH(pin2);
    }

    void hoding_state(string pin1, string pin2)
    {
        handle_Canvas.send_ESP32_LOW(pin1);
        handle_Canvas.send_ESP32_LOW(pin2);
    }


}