using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;

public class PIDController : MonoBehaviour
{
    public CubeStretchMover cubeStretchMover; // 用户指定的传感器脚本
    public Handle_canvas handle_Canvas; // 用户指定的气阀控制脚本

    public float Kp = 1.0f; // 比例增益
    public float Ki = 0.1f; // 积分增益
    public float Kd = 0.01f; // 微分增益

    private float integral = 0.0f;
    private float lastError = 0.0f;

    // define the pin
    public string stretch_module_actuation;
    public string stretch_module_air;
    public string bending_module_actuation;
    public string bending_module_air;
    public string twist_module_actuation;
    public string twist_module_air;

    // 传感器测量值
    public float T_sensor;

    public float T_goal = 1.0f; // 目标值

    public PinchSlider pinchSlider; // 用于user输入

    void Update()
    {
        // 从SensorScript获取T_sensor的值
        T_sensor = cubeStretchMover.GetSensorValue();
        T_goal = pinchSlider.SliderValue;

        float error = T_goal - T_sensor;

        integral += error * Time.deltaTime;
        float derivative = (error - lastError) / Time.deltaTime;

        float control = Kp * error + Ki * integral + Kd * derivative;

        // 应用控制量，例如控制温度或位置
        ApplyControl(control);

        lastError = error;
    }

    void ApplyControl(float control)
    {
        float epsilon = 0.1f; // 定义一个阈值epsilon

        if (control > 0) // 当控制量大于0时
        {
            // 设置压力Pin为正值，连接高压空气或真空
            SetPressure(1.0f); // 例如设置为1.0表示高压
        }
        else if (control < 0) // 当控制量小于0时
        {
            // 设置压力Pin为负值，连接空气
            SetPressure(-1.0f); // 例如设置为-1.0表示低压
        }
        else if (Mathf.Abs(control) < epsilon) // 当控制量的绝对值小于epsilon时
        {
            // 设置压力Pin为零，关闭所有阀门以保持变形
            SetPressure(0.0f); // 设置为0表示关闭阀门
        }
    }

    void SetPressure(float pressure)
    {
        // 在这里实现设置压力的逻辑，例如连接到高压空气、真空或关闭阀门
        // 你需要根据具体的系统来实现设置压力的逻辑
        // 例如：通过控制阀门或其他设备来调整压力
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