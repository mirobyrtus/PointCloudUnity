using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.InteropServices;

public class PluginPCL : MonoBehaviour {

	class Cloud {
		public Vector3[] points;
		public int length;

		// int cloudTag;

		public Cloud(int newLength) {
			length = newLength;
			points = new Vector3[length];
		}
	}

	class Clouds {
		public Cloud[] clouds;
		public int length;
		
		public Clouds(int newLength) {
			length = newLength;
			clouds = new Cloud[length];
		}
	}

	// [DllImport ("cpp_plugin_pcl")]
	// private static extern void getPointCloud();

	[DllImport ("cpp_plugin_pcl")]
	private static extern IntPtr PrintHello();

	[DllImport ("cpp_plugin_pcl")]
	private static extern IntPtr getIntArray();

	[DllImport("cpp_plugin_pcl")]
	private static extern bool getPseudoPointCloud(ref IntPtr ptrResultVertsX, ref IntPtr ptrResultVertsY, ref IntPtr ptrResultVertsZ, ref int resultVertLength);

	[DllImport("cpp_plugin_pcl")]
	private static extern bool readPointCloud(ref IntPtr ptrResultVertsX, ref IntPtr ptrResultVertsY, ref IntPtr ptrResultVertsZ, ref int resultVertLength);

	[DllImport("cpp_plugin_pcl")]
	private static extern bool extractClusters();

	[DllImport("cpp_plugin_pcl")]
	private static extern bool readCloud();

	[DllImport("cpp_plugin_pcl")]
	private static extern bool removeBiggestPlane();
	
	[DllImport("cpp_plugin_pcl")]
	private static extern bool getClusters();

	[DllImport("cpp_plugin_pcl")]
	private static extern int getClustersCount();

	[DllImport("cpp_plugin_pcl")]
	private static extern bool getCluster(int clusterIndex, ref IntPtr ptrResultVertsX, ref IntPtr ptrResultVertsY, ref IntPtr ptrResultVertsZ, ref int resultVertLength);

	[DllImport("cpp_plugin_pcl")]
	private static extern bool getCloud(ref IntPtr ptrResultVertsX, ref IntPtr ptrResultVertsY, ref IntPtr ptrResultVertsZ, ref int resultVertLength);

	// Struct test begin
	public struct MyCloud
	{
		public Int32 Size;
		public IntPtr pointsX, pointsY, pointsZ;
	}
	
	[DllImport("cpp_plugin_pcl")]
	public static extern bool structureTest(ref IntPtr myCloudStructure);

	public static void CallFunction()
	{
		MyCloud managedObj;
		managedObj.pointsX = IntPtr.Zero;
		managedObj.pointsY = IntPtr.Zero;
		managedObj.pointsZ = IntPtr.Zero;
		managedObj.Size = 0;

		IntPtr unmanagedAddr = Marshal.AllocHGlobal(Marshal.SizeOf(managedObj));
		Marshal.StructureToPtr(managedObj, unmanagedAddr, true);
		
		structureTest(ref unmanagedAddr);
		
		Marshal.PtrToStructure(unmanagedAddr, managedObj);

		Debug.Log ("Structure = " + managedObj);

		Marshal.FreeHGlobal(unmanagedAddr);
		unmanagedAddr = IntPtr.Zero;
	}

	// Struct test end 

	void drawMyCloud () {
		IntPtr ptrResultVertsX = IntPtr.Zero;
		IntPtr ptrResultVertsY = IntPtr.Zero;
		IntPtr ptrResultVertsZ = IntPtr.Zero;
		int resultVertLength = 0;
		
		bool success = getCloud (ref ptrResultVertsX, ref ptrResultVertsY, ref ptrResultVertsZ, ref resultVertLength);
		if (success) {
			// Load the results into a managed array.
			float[] resultVerticesX = new float[resultVertLength];
			float[] resultVerticesY = new float[resultVertLength];
			float[] resultVerticesZ = new float[resultVertLength];
			
			Marshal.Copy (
				ptrResultVertsX, 
				resultVerticesX, 
				0, 
				resultVertLength
				);
			
			Marshal.Copy (
				ptrResultVertsY, 
				resultVerticesY, 
				0, 
				resultVertLength
				);
			
			Marshal.Copy (
				ptrResultVertsZ, 
				resultVerticesZ, 
				0, 
				resultVertLength
				);
			
			for (int i = 0; i < resultVertLength; i++) {
				createCube(resultVerticesX[i], resultVerticesY[i], resultVerticesZ[i]);
			}
			
			return;
		} else { 
			Debug.Log("Ended not sucessfully."); 
			return;
		}
	} 

	Cloud readMyCloud () {
		IntPtr ptrResultVertsX = IntPtr.Zero;
		IntPtr ptrResultVertsY = IntPtr.Zero;
		IntPtr ptrResultVertsZ = IntPtr.Zero;
		int resultVertLength = 0;
		
		bool success = readPointCloud (ref ptrResultVertsX, ref ptrResultVertsY, ref ptrResultVertsZ, ref resultVertLength);
		if (success) {
			// Load the results into a managed array.
			float[] resultVerticesX = new float[resultVertLength];
			float[] resultVerticesY = new float[resultVertLength];
			float[] resultVerticesZ = new float[resultVertLength];
			
			Marshal.Copy (
				ptrResultVertsX, 
				resultVerticesX, 
				0, 
				resultVertLength
				);
			
			Marshal.Copy (
				ptrResultVertsY, 
				resultVerticesY, 
				0, 
				resultVertLength
				);
			
			Marshal.Copy (
				ptrResultVertsZ, 
				resultVerticesZ, 
				0, 
				resultVertLength
				);

			Cloud result = new Cloud(resultVertLength);
		
			for (int i = 0; i < resultVertLength; i++) {
				result.points[i] = new Vector3(resultVerticesX[i], resultVerticesY[i], resultVerticesZ[i]);
			}
			
			return result;
		} else { 
			Debug.Log("Ended not sucessfully."); 
			return null;
		}
	} 

	Clouds readCloudAndExtractClusters(Cloud cloud) {
		return null;
		// TODO
	}

	Vector3[] getPointData() {
		IntPtr ptrResultVertsX = IntPtr.Zero;
		IntPtr ptrResultVertsY = IntPtr.Zero;
		IntPtr ptrResultVertsZ = IntPtr.Zero;
		int resultVertLength = 0;

		// bool success = getPseudoPointCloud (ref ptrResultVertsX, ref ptrResultVertsY, ref ptrResultVertsZ, ref resultVertLength);
		bool success = readPointCloud (ref ptrResultVertsX, ref ptrResultVertsY, ref ptrResultVertsZ, ref resultVertLength);

		if (success) {
			// Load the results into a managed array.
			float[] resultVerticesX = new float[resultVertLength];
			float[] resultVerticesY = new float[resultVertLength];
			float[] resultVerticesZ = new float[resultVertLength];

			Marshal.Copy (
				ptrResultVertsX, 
				resultVerticesX, 
				0, 
				resultVertLength
			);

			Marshal.Copy (
				ptrResultVertsY, 
				resultVerticesY, 
				0, 
				resultVertLength
			);

			Marshal.Copy (
				ptrResultVertsZ, 
				resultVerticesZ, 
				0, 
				resultVertLength
			);

			Vector3[] pseudoCloud = new Vector3[resultVertLength];

			for (int i = 0; i < resultVertLength; i++) {
				pseudoCloud[i] = new Vector3(resultVerticesX[i], resultVerticesY[i], resultVerticesZ[i]);
				createCube(resultVerticesX[i], resultVerticesY[i], resultVerticesZ[i]);
			}

			return pseudoCloud;
		} else { 
			Debug.Log("Ended not sucessfully."); 
			return null;
		}

		/*
		* WARNING!!!! IMPORTANT!!!
		* In this example the plugin created an array allocated
		* in unmanged memory.  The plugin will need to provide a
		* means to free the memory.
		*/

		// Do something with the array results.
	}

	void createCube(float x, float y, float z) {
		createCube (x, y, z, Color.black, 0);
	}

	void createCube(float x, float y, float z, Color color, int tag) {
		GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
		cube.transform.position = new Vector3(x, y, z);

		cube.name = "point_in_cloud_" + tag;
		// cube.AddComponent ("MyClickHandler");
		cube.renderer.material.color = color;
		
		float scale = 0.005F;
		cube.transform.localScale = new Vector3 (scale, scale, scale);
	}

	void drawCluster(int clusterIndex) {
		IntPtr ptrResultVertsX = IntPtr.Zero;
		IntPtr ptrResultVertsY = IntPtr.Zero;
		IntPtr ptrResultVertsZ = IntPtr.Zero;
		int resultVertLength = 0;

		bool success = getCluster (clusterIndex, ref ptrResultVertsX, ref ptrResultVertsY, ref ptrResultVertsZ, ref resultVertLength);
		if (success) {
			// Debug.Log ("ResultVertLenght = " + resultVertLength);

			// Load the results into a managed array.
			float[] resultVerticesX = new float[resultVertLength];
			float[] resultVerticesY = new float[resultVertLength];
			float[] resultVerticesZ = new float[resultVertLength];

			Marshal.Copy (
				ptrResultVertsX, 
				resultVerticesX, 
				0, 
				resultVertLength
				);
			
			Marshal.Copy (
				ptrResultVertsY, 
				resultVerticesY, 
				0, 
				resultVertLength
				);
			
			Marshal.Copy (
				ptrResultVertsZ, 
				resultVerticesZ, 
				0, 
				resultVertLength
				);

			System.Random r = new System.Random();
			Color color = new Color((float)r.Next(0, 255) / 255, (float)r.Next(0, 255) / 255, (float)r.Next(0, 255) / 255, 1f);
			int tag = clusterIndex;

			// TODO create unique tag and add it into tags

			for (int i = 0; i < resultVertLength; i++) {
				createCube(resultVerticesX[i], resultVerticesY[i], resultVerticesZ[i], color, tag);
			}

			resultVerticesX = null;
			resultVerticesY = null;
			resultVerticesZ = null;

			/*
			* WARNING!!!! IMPORTANT!!!
			* In this example the plugin created an array allocated
			* in unmanged memory.  The plugin will need to provide a
			* means to free the memory.
			*/

		} else { 
			Debug.Log("Ended not sucessfully."); 
		} 

		ptrResultVertsX = IntPtr.Zero;
		ptrResultVertsY = IntPtr.Zero;
		ptrResultVertsZ = IntPtr.Zero;

		return;
	}

	void Start () 
	{
		Debug.Log ("readCloud() : " + readCloud ());
		Debug.Log ("removeBiggestPlane() : " + removeBiggestPlane ());
		Debug.Log ("getClusters() : " + getClusters ());
		Debug.Log ("getClustersCount() : " + getClustersCount ());

		for (int i = 0; i < getClustersCount(); i++) {
			drawCluster (i);
		}
	}

	int selectedTag = -1;
	float keyboardMoveSensitivity = 100f;

	void Update ()
	{
		if (Input.GetMouseButtonUp (0)) {

			GameObject clickedGmObj = GetClickedGameObject();
			
			if (clickedGmObj != null) {
				String objectName = clickedGmObj.name;
				int underline_index = objectName.LastIndexOf("_");
				if (underline_index >= 0) {
					String tagName = objectName.Substring(underline_index + 1);

					selectedTag = Convert.ToInt32(tagName);
					Debug.Log("Selecting tag " + selectedTag);

				} else {
					Debug.Log("Name of clicked object has wrong format");
					selectedTag = -1;
				}
			} else {
				selectedTag = -1;
			}
		}

		if (Input.GetKey ("a")) {
			if (selectedTag >= 0) {
				// Move whole cloud to left
				GameObject[] myPoints  = (GameObject[])FindObjectsOfType (typeof(GameObject));
				Debug.Log("myPoints length = " + myPoints.Length);
				for (int i=0; i < myPoints.Length; i++) {

					if (myPoints[i].name.Equals("point_in_cloud_" + selectedTag)) {
						myPoints[i].transform.Translate(new Vector3(1 / keyboardMoveSensitivity , 0, 0));
					}

				}

				Debug.Log("Cloud segment moved!");
			}
		}
	}

	public LayerMask layerMask;

	GameObject GetClickedGameObject()
	{
		// Builds a ray from camera point of view to the mouse position
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		// Casts the ray and get the first game object hit
		if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
			return hit.transform.gameObject;
		else
			return null;
	}
}
