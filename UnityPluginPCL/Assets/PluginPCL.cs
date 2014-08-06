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
	private static extern bool readKinectCloud();

	[DllImport("cpp_plugin_pcl")]
	private static extern bool removeBiggestPlane(int maxIterations, double distanceThreshold);
	
	[DllImport("cpp_plugin_pcl")]
	private static extern bool getClusters(double clusterTolerance, int minClusterSize, int maxClusterSize);
	
	[DllImport("cpp_plugin_pcl")]
	private static extern int getCloudSize();

	[DllImport("cpp_plugin_pcl")]
	private static extern int getClustersCount();

	[DllImport("cpp_plugin_pcl")]
	private static extern bool getCluster(int clusterIndex, ref IntPtr ptrResultVertsX, ref IntPtr ptrResultVertsY, ref IntPtr ptrResultVertsZ, ref int resultVertLength);

	[DllImport("cpp_plugin_pcl")]
	private static extern bool getClusterIndices(int clusterIndex, ref IntPtr indices, ref int indicesLength);

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

	public Transform point;
	public bool usePrefabs = true;

	void createCube(float x, float y, float z, Color color, int tag) {
		if (usePrefabs) {
			Transform t = ((Transform)Instantiate(point, new Vector3(x, y, z), Quaternion.identity));
			t.renderer.material.color = color;
			t.name = "point_in_cloud_" + tag;

		} else {
			GameObject cube = GameObject.CreatePrimitive (PrimitiveType.Cube);
			cube.transform.position = new Vector3 (x, y, z);

			cube.name = "point_in_cloud_" + tag;
			// cube.AddComponent ("MyClickHandler");
			cube.renderer.material.color = color;

			float scale = 0.005F;
			cube.transform.localScale = new Vector3 (scale, scale, scale);
		}
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

	void debugIndices(int clusterIndex) {
		IntPtr ptrIndices = IntPtr.Zero;
		int resultIndicesLength = 0;
		
		bool success = getClusterIndices (clusterIndex, ref ptrIndices, ref resultIndicesLength);
		if (success) {
			// Debug.Log ("ResultVertLenght = " + resultVertLength);
			
			// Load the results into a managed array.
			float[] resultIndices = new float[resultIndicesLength];

			Marshal.Copy (
				ptrIndices, 
				resultIndices, 
				0, 
				resultIndicesLength
			);

			String indicesStr = "Indices Debug : ";

			for (int i = 0; i < resultIndicesLength; i++) {
				indicesStr += " : " + resultIndices[i];
			}

			// Debug.Log(indicesStr + ".");
			
			resultIndices = null;

			/*
			* WARNING!!!! IMPORTANT!!!
			* In this example the plugin created an array allocated
			* in unmanged memory.  The plugin will need to provide a
			* means to free the memory.
			*/
			
		} else { 
			Debug.Log("Ended not sucessfully."); 
		} 
		
		ptrIndices = IntPtr.Zero;
		
		return;
	}

	void refreshKinect() {
		Debug.Log ("Refreshing kinect ..");

		// Clean pointcloud
		readKinectCloud ();

		/*
		// Clean scene (delete old cubes)
		// Move whole cloud to left
		GameObject[] myPoints  = (GameObject[])FindObjectsOfType (typeof(GameObject));
		for (int i=0; i < myPoints.Length; i++) {

			if (myPoints[i].name.StartsWith("point_in_cloud")) {
				Destroy (myPoints[i]);
			}
		}
		
		getCloudSize ();
		removeBiggestPlane ();
		getClusters ();
		getClustersCount ();
		
		for (int i = 0; i < getClustersCount(); i++) {
			drawCluster (i);
		}
		*/
	}
	
	float totaltime; 
	string printTimeDelta() {
		float ms = Time.realtimeSinceStartup - totaltime;
		totaltime = Time.realtimeSinceStartup;
		return " in " + ms + " s.";
	}

	public bool useKinect;
	public bool kinectRealtime;
	public bool debug_indices;

	// removeBiggestPlane parameters
	public int maxIterations = 100;
	public double distanceThreshold = 0.02;

	// getClusters parameters
	public double clusterTolerance = 0.02; 
	public int minClusterSize = 100; 
	public int maxClusterSize = 25000;

	void Start () 
	{
		totaltime = Time.realtimeSinceStartup; 

		if (useKinect) {
			Debug.Log ("readKinectCloud() : " + readKinectCloud () + printTimeDelta());
		} else {
			Debug.Log ("readCloud() : " + readCloud () + printTimeDelta());
		}

		if (true) {
			Debug.Log ("getCloudSize() : " + getCloudSize () + printTimeDelta());
			Debug.Log ("removeBiggestPlane() : " + removeBiggestPlane (maxIterations, distanceThreshold) + printTimeDelta());
			Debug.Log ("getClusters() : " + getClusters (clusterTolerance, minClusterSize, maxClusterSize) + printTimeDelta());
			Debug.Log ("getClustersCount() : " + getClustersCount () + printTimeDelta());

			for (int i = 0; i < getClustersCount(); i++) {
					drawCluster (i);
	
					if (debug_indices) {
							debugIndices (i);
					}
			}

			Debug.Log ("Clusters drawn" + printTimeDelta());

		} else {
				// Just for testing... dont use that
				drawMyCloud();

		}

		totaltime = Time.realtimeSinceStartup;
	}

	int selectedTag = -1;
	float keyboardMoveSensitivity = 100f;


	void Update ()
	{
		// Debug.Log ("Time = " + (Time.realtimeSinceStartup - totaltime));
		if (useKinect && kinectRealtime) {
				if ((Time.realtimeSinceStartup - totaltime) > 5f) {
						Debug.Log ("refresh now?");
						totaltime = Time.realtimeSinceStartup;

						refreshKinect ();
				}
		}

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
