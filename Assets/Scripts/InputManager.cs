using UnityEngine;

public class InputManager : MonoBehaviour
{
	private VideoManager videoManager = null;

	public OVRHand leftHand = null;
	private bool isLeftHandPinching = false;
	public OVRHand rightHand = null;
	private bool isRightHandPinching = false;

	// https://github.com/jemmec/metaface-utilities/blob/master/Assets/Metaface/Scripts/Blinking/BlinkHelper.cs
	private OVRFaceExpressions faceExpressions = null;

	private bool hasStartedBlink;
	private float blinkTime;

	private void Start()
	{
		videoManager = GetComponent<VideoManager>();
		faceExpressions = GetComponent<OVRFaceExpressions>();
	}

	private void Update()
	{
		if (!videoManager.IsVideoReady)
			return;

		HandTrackingInput();
		KeyboardInput();
		FaceExpressionsInput();
	}

	private void HandTrackingInput()
	{
		bool _isRightHandPinching = rightHand.GetFingerIsPinching(OVRHand.HandFinger.Index);
		if (_isRightHandPinching != isRightHandPinching)
		{
			isRightHandPinching = _isRightHandPinching;
			if (isRightHandPinching)
			{
				videoManager.PauseToggle();
			}
		}
	}

	private void FaceExpressionsInput()
	{
		if (!faceExpressions) return;

		bool areEyesClosed = AreEyesClosed();


		if (!hasStartedBlink)
		{
			if (areEyesClosed)
			{
				hasStartedBlink = true;
				blinkTime = 0f;
			}
		}
		else
		{
			blinkTime += Time.deltaTime;
			if (!areEyesClosed)
			{
				hasStartedBlink = false;
				videoManager.PauseToggle();
			}
		}
	}

	private bool AreEyesClosed()
	{
		float leftEyeClosedWeight;
		faceExpressions.TryGetFaceExpressionWeight(OVRFaceExpressions.FaceExpression.EyesClosedL, out leftEyeClosedWeight);
		float rightEyeClosedWeight;
		faceExpressions.TryGetFaceExpressionWeight(OVRFaceExpressions.FaceExpression.EyesClosedR, out rightEyeClosedWeight);
		bool areEyesClosed = leftEyeClosedWeight < 0.5 && rightEyeClosedWeight < 0.5;
		return areEyesClosed;
	}

	private void KeyboardInput()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			videoManager.PauseToggle();
		}

		if (Input.GetKeyDown(KeyCode.LeftArrow))
		{
			videoManager.PreviousVideo();
		}

		if (Input.GetKeyDown(KeyCode.RightArrow))
		{
			videoManager.NextVideo();
		}

		if (Input.GetKeyDown(KeyCode.DownArrow))
		{
			videoManager.SeekBack();
		}

		if (Input.GetKeyDown(KeyCode.UpArrow))
		{
			videoManager.SeekForward();
		}
	}
}