using UnityEngine;
using System.Collections;
public class MouseLook : MonoBehaviour
{
	public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
	public RotationAxes axes = RotationAxes.MouseXAndY;

	public float sensitivityX = 5F;
	public float sensitivityY = 5F;

	public float minimumX = -360F;
	public float maximumX = 360F;

	public float minimumY = -80F;
	public float maximumY = 80F;

	float moveSensitivity = 1.0f;
	public float speed = 6.0f;

	float rotationX = 0F;
	float rotationY = 0F;

	Quaternion originalRotation;

	void Update()
	{
		//限制摄像机高度
		if (transform.position.y < 0.5f) {
			transform.position = new Vector3(transform.position.x,0.5f,transform.position.z);
		}

		if (this.transform.GetChild (0).transform.localPosition != Vector3.zero || this.transform.GetChild (0).transform.localEulerAngles != Vector3.zero) {
			this.transform.GetChild (0).transform.localPosition = Vector3.zero;
			this.transform.GetChild (0).transform.localEulerAngles = Vector3.zero;
		}

		if (Input.GetMouseButton (1) && axes == RotationAxes.MouseXAndY) {
			// Read the mouse input axis
			rotationX += Input.GetAxis ("Mouse X") * sensitivityX;
			rotationY += Input.GetAxis ("Mouse Y") * sensitivityY;

			rotationX = ClampAngle (rotationX, minimumX, maximumX);
			rotationY = ClampAngle (rotationY, minimumY, maximumY);

			Quaternion xQuaternion = Quaternion.AngleAxis (rotationX, Vector3.up);
			Quaternion yQuaternion = Quaternion.AngleAxis (rotationY, Vector3.left);

			transform.localRotation = originalRotation * xQuaternion * yQuaternion;
		} else if (Input.GetMouseButton (1) && axes == RotationAxes.MouseX) {
			rotationX += Input.GetAxis ("Mouse X") * sensitivityX;
			rotationX = ClampAngle (rotationX, minimumX, maximumX);

			Quaternion xQuaternion = Quaternion.AngleAxis (rotationX, Vector3.up);
			transform.localRotation = originalRotation * xQuaternion;
		} else if (Input.GetMouseButton (1) && axes == RotationAxes.MouseY) {
			rotationY += Input.GetAxis ("Mouse Y") * sensitivityY;
			rotationY = ClampAngle (rotationY, minimumY, maximumY);

			Quaternion yQuaternion = Quaternion.AngleAxis (rotationY, Vector3.left);
			transform.localRotation = originalRotation * yQuaternion;
		}

		//鼠标滚轮
		if (Input.GetAxis("Mouse ScrollWheel") != 0)
		{
			transform.position += transform.forward * Input.GetAxis("Mouse ScrollWheel") * speed;
		}
	}
	void Start()
	{
		// Make the rigid body not change rotation
		if (GetComponent<Rigidbody>())
			GetComponent<Rigidbody>().freezeRotation = true;
		originalRotation = transform.localRotation;
	}
	public static float ClampAngle(float angle, float min, float max)
	{
		if (angle < -360F)
			angle += 360F;
		if (angle > 360F)
			angle -= 360F;
		return Mathf.Clamp(angle, min, max);
	}
}