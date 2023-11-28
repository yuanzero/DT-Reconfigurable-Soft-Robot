using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SoftgripperJointHandler : MonoBehaviour
{
    // The position in the message
    [Tooltip("Latest received position message (=desired state)")]
    public Vector3 positionMsg;
    // Rotation in the message
    [Tooltip("Latest received rotation message (=desired rotation)")]
    public Vector3 rotationMsg;
    // Set true if rotation shall remain untouched
    [Tooltip("Set true if rotation shall remain untouched")]
    public bool isFixedRotation =false;
    // Set true if bone transform shall remain untouched
    [Tooltip("Set true if bone transform shall remain untouched")]
    public bool isSkipBone = false;

    // Set true if using slider control
    [Tooltip("Set true if using slider control")]
    public bool isSliderControl = true;

    // Initial rotation of the transform
    [Tooltip("Initial rotation of the transform (Quaternion)")]
    public Quaternion initState;
    public Vector3 initPosition;

    // Any position offsets 
    [Tooltip("Any position offsets")]
    public float xOffset, yOffset, zOffset;
    // Correctional Vector (e.g. if reference frames in ROS are not identical to the Unity equivalent)
    [Tooltip("Correctional Vector (e.g. if reference frames in ROS are not identical to the Unity equivalent)")]
    public Vector3 CorrectionVector;

    // Define the interpolation speed between current state and desired state for position and oreintation
    [Tooltip("Define the interpolation speed between current state and desired state for position")]
    public float positionLerpSpeed = 0.21f;
    [Tooltip("Define the interpolation speed between current state and desired state for oreintation")]
    public float rotationLerpSpeed = 0.21f;

    // Get the inital orientation of the transform
    private void Start()
    {
        initState = transform.rotation;
        initPosition = transform.position;
        
        xOffset = initPosition.x;
        yOffset = initPosition.y;
        zOffset = initPosition.z;
    }
/*
    /// <summary>
    /// Entry point for mesh deformation. Receives the relevant Vector[2] from the <c>Softgripper4JointSubscriber</c>, 
    /// Reduces the amount of decimals to 3 to reduces unnecessary flickering and stores these in the positionMsg and rotationMsg.
    /// </summary>
    /// <param name="data"></param>
    public void FeedData(Vector3[] data)
    {
        if (!isSkipBone)
        {
            positionMsg = new Vector3((float)Mathf.Round(data[0].x * 1000f) / 1000f + xOffset, (float)Mathf.Round(-data[0].y * 1000f) / 1000f + yOffset, data[0].z);

            if (!isFixedRotation)
            {
                Vector3 vec = new Vector3(CorrectionVector.x, CorrectionVector.y, -data[1].z+180);
                rotationMsg = vec;
            }
        }
    }
*/
    /// <summary>
    /// Updates the position of the bones with each frame, lerps towards the desired states according to the specified lerp speeds.
    /// Lerping is used to obtain visually pleasing results if the received messages are not idientical to the frame rate (e.g. 10 new states per second). 
    /// The resultant delay was not (visually) noticable until less than 15 updates per second were received.
    /// </summary>
    /// 
    private void Update()
    {
        if (isSliderControl)
        {
            if (!isSkipBone)
            {
                transform.position = Vector3.Lerp(transform.position, positionMsg, positionLerpSpeed);
                if (!isFixedRotation)
                {
                   transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(rotationMsg), rotationLerpSpeed);
                }
            }
        }
        
    }
}

