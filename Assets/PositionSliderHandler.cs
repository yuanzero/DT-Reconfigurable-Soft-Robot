using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/* This class is used to manipulate the interpolation/lerp speed between current state and the 
 * latest via Mqtt - DTStacks received pose message. 
 * This lerping procedure is an effective way to limit network traffic and overloading of the Unity-environment 
 * if multiple (different) soft-robots shall be visualized.
 * 
 * This is intended to find suitable balancing values while the application remains within the unity editor, 
 * hence, no UWP version is made. The determined values need to be entered as fixed to the joints before building the final application.
 */


public class PositionSliderHandler : MonoBehaviour
{
    //Get reference to the UI elements
    public Slider positionSlider, rotationSlider;
    public Text positionValueText, rotationValueText;


    // Get reference to all individual joint handlers
    public SoftgripperJointHandler[] jointHandler;
    
    void Update()
    {
        //update each joint handler lerp speed with the slider values
        foreach(SoftgripperJointHandler handler in jointHandler)
        {
            handler.positionLerpSpeed = positionSlider.value;
            handler.rotationLerpSpeed = rotationSlider.value;
        }
        //update the textbox next to the sliders with the slider value
        positionValueText.text = positionSlider.value.ToString();
        rotationValueText.text = rotationSlider.value.ToString();
    }
}
