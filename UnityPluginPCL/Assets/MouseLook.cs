using UnityEngine;
using System.Collections;

[AddComponentMenu("Camera-Control/Mouse Look")]
public class MouseLook : MonoBehaviour {

	// Mouse part

	public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
	public RotationAxes axes = RotationAxes.MouseXAndY;
	public float sensitivityX = 20F;
	public float sensitivityY = 20F;

	public float sensitivityKeyboard = 10f;
	
	public float minimumX = -360F;
	public float maximumX = 360F;
	
	public float minimumY = -60F;
	public float maximumY = 60F;
	
	float rotationX = 0F;
	float rotationY = 0F;

	// Keyboard part

	public float speed = 5.0f;
	
	Quaternion originalRotation;
	
	void Update ()
	{
		if (Input.GetMouseButton (1)) {
			if (axes == RotationAxes.MouseXAndY) {
					// Read the mouse input axis
					rotationX += Input.GetAxis ("Mouse X") * sensitivityX;
					rotationY += Input.GetAxis ("Mouse Y") * sensitivityY;

					rotationX = ClampAngle (rotationX, minimumX, maximumX);
					rotationY = ClampAngle (rotationY, minimumY, maximumY);

					Quaternion xQuaternion = Quaternion.AngleAxis (rotationX, Vector3.up);
					Quaternion yQuaternion = Quaternion.AngleAxis (rotationY, -Vector3.right);

					transform.localRotation = originalRotation * xQuaternion * yQuaternion;
			} else if (axes == RotationAxes.MouseX) {
					rotationX += Input.GetAxis ("Mouse X") * sensitivityX;
					rotationX = ClampAngle (rotationX, minimumX, maximumX);

					Quaternion xQuaternion = Quaternion.AngleAxis (rotationX, Vector3.up);
					transform.localRotation = originalRotation * xQuaternion;
			} else {
					rotationY += Input.GetAxis ("Mouse Y") * sensitivityY;
					rotationY = ClampAngle (rotationY, minimumY, maximumY);

					Quaternion yQuaternion = Quaternion.AngleAxis (-rotationY, Vector3.right);
					transform.localRotation = originalRotation * yQuaternion;
			}
		}

		if(Input.GetKey(KeyCode.RightArrow))
		{
			transform.Translate(new Vector3(1 / sensitivityKeyboard,0,0));
		}
		if(Input.GetKey(KeyCode.LeftArrow))
		{
			transform.Translate(new Vector3(-1 / sensitivityKeyboard,0,0));
		}
		if(Input.GetKey(KeyCode.DownArrow))
		{
			transform.Translate(new Vector3(0,0,-1 / sensitivityKeyboard));
		}
		if(Input.GetKey(KeyCode.UpArrow))
		{
			transform.Translate(new Vector3(0,0,1 / sensitivityKeyboard));
		}
	}	
	
	void Start ()
	{
		// Make the rigid body not change rotation
		if (rigidbody)
			rigidbody.freezeRotation = true;
		originalRotation = transform.localRotation;
	}
	
	public static float ClampAngle (float angle, float min, float max)
	{
		if (angle < -360F)
			angle += 360F;
		if (angle > 360F)
			angle -= 360F;
		return Mathf.Clamp (angle, min, max);
	}
}