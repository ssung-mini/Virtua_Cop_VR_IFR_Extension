using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using ViveSR.anipal.Eye;

public class EyeDataManager : MonoBehaviour
{
	private static EyeData eyeData = new EyeData();
	private bool eye_callback_registered = false;
	private VerboseData verboseData;

	[ReadOnly]
	private int timestamp;
	[ReadOnly]
	public Vector3 gazeDirection;
	[ReadOnly]
	public Vector2 gazePosition;
	[ReadOnly]
	public Vector2 leftPupilPositionInSensorArea;
	[ReadOnly]
	public Vector2 rightPupilPositionInSensorArea;
	[ReadOnly]
	public Vector2 pupilDiameter_mm;
//	[ReadOnly]
	public Camera cam;

	public Vector3 gazePosition3;
	public Vector3 realGazePosition;    // 실제 보는 곳이 어딘지 확인하기 위한 임시 변수.
	public Vector3 tempGaze;



	// Update is called once per frame
	void FixedUpdate()
	{
		//framework의 status 확인
		if (SRanipal_Eye_Framework.Status != SRanipal_Eye_Framework.FrameworkStatus.WORKING &&
			SRanipal_Eye_Framework.Status != SRanipal_Eye_Framework.FrameworkStatus.NOT_SUPPORT) return;

		//callback이 켜져있을 경우 callback 설정
		if (SRanipal_Eye_Framework.Instance.EnableEyeDataCallback == true && eye_callback_registered == false)
		{
			SRanipal_Eye.WrapperRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye.CallbackBasic)EyeCallback));
			eye_callback_registered = true;
		}
		else if (SRanipal_Eye_Framework.Instance.EnableEyeDataCallback == false && eye_callback_registered == true)
		{
			SRanipal_Eye.WrapperUnRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye.CallbackBasic)EyeCallback));
			eye_callback_registered = false;
		}

		//callback이 있을 경우 가져오기만 하고,
		//callback이 없을 경우 update 하면서 가져오기
		if (eye_callback_registered)
		{
			if (SRanipal_Eye.GetVerboseData(out verboseData, eyeData)) { }
			else return;
		}
		else
		{
			if (SRanipal_Eye.GetVerboseData(out verboseData)) { }
			else return;
		}

		GetValidEyeData();
	}

	private void GetValidEyeData()
	{
		if (SRanipal_Eye_Framework.Status == SRanipal_Eye_Framework.FrameworkStatus.WORKING)
		{
			SingleEyeData left = verboseData.left;
			SingleEyeData right = verboseData.right;
			SingleEyeData combined = verboseData.combined.eye_data;

			//pupil diameter
			if (left.GetValidity(SingleEyeDataValidity.SINGLE_EYE_DATA_PUPIL_DIAMETER_VALIDITY))
			{
				pupilDiameter_mm.x = left.pupil_diameter_mm;
			}
			if (right.GetValidity(SingleEyeDataValidity.SINGLE_EYE_DATA_PUPIL_DIAMETER_VALIDITY))
			{
				pupilDiameter_mm.y = right.pupil_diameter_mm;
			}

			//pupil position
			if (left.GetValidity(SingleEyeDataValidity.SINGLE_EYE_DATA_PUPIL_POSITION_IN_SENSOR_AREA_VALIDITY))
			{
				leftPupilPositionInSensorArea = new Vector2(left.pupil_position_in_sensor_area.x * 2 - 1,
																left.pupil_position_in_sensor_area.y * -2 + 1);
			}
			if (right.GetValidity(SingleEyeDataValidity.SINGLE_EYE_DATA_PUPIL_POSITION_IN_SENSOR_AREA_VALIDITY))
			{
				rightPupilPositionInSensorArea = new Vector2(right.pupil_position_in_sensor_area.x * 2 - 1,
																right.pupil_position_in_sensor_area.y * -2 + 1);
			}

			
			//gaze direction
			if (combined.GetValidity(SingleEyeDataValidity.SINGLE_EYE_DATA_GAZE_DIRECTION_VALIDITY))
			{
				gazeDirection = combined.gaze_direction_normalized;
			}
			else if (left.GetValidity(SingleEyeDataValidity.SINGLE_EYE_DATA_GAZE_DIRECTION_VALIDITY))
			{
				gazeDirection = left.gaze_direction_normalized;
			}
			else if (right.GetValidity(SingleEyeDataValidity.SINGLE_EYE_DATA_GAZE_DIRECTION_VALIDITY))
			{
				gazeDirection = right.gaze_direction_normalized;
			}
			else
			{
				return;
			}
			gazeDirection.x *= -1;
			GetGazePosition();
		}
	}

	private void GetGazePosition()
	{
		//Debug.Log("Camera? : " + Camera.main);
		//Vector3 gazePosition3 = Camera.main.WorldToViewportPoint(Camera.main.transform.position + Camera.main.transform.TransformDirection(gazeDirection));
		realGazePosition = cam.transform.position + cam.transform.TransformDirection(gazeDirection) * 8;	// distance to targets = 8
		gazePosition3 = cam.WorldToViewportPoint(cam.transform.position + cam.transform.TransformDirection(gazeDirection));

		//Debug.Log("GazePosition3: " + gazePosition3);
		gazePosition = new Vector2(gazePosition3.x, 1 - gazePosition3.y);
	}

	private void OnApplicationQuit()
	{
		if (eye_callback_registered == true)
		{
			SRanipal_Eye.WrapperUnRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye.CallbackBasic)EyeCallback));
			eye_callback_registered = false;
		}
	}

	private static void EyeCallback(ref EyeData eye_data)
	{
		eyeData = eye_data;
	}
}
