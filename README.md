 
 ************************************************************************************************
 ************************************************************************************************
 
 From Design to Software 2
 
 Miroslav Byrtus
 1328167
 
 ************************************************************************************************
 
 Description 
 
 PCL Integration in Unity 

 Unity: 
 * Load pointclouds (from file / kinect)
 * Segment
   + Plane model segmentation (floor segmentation)
     http://pointclouds.org/documentation/tutorials/planar_segmentation.php#planar-segmentation) 
   + Euclidean Cluster Extraction (object segmentation)
     http://pointclouds.org/documentation/tutorials/cluster_extraction.php#cluster-extraction)
   + Store indices of segmented clusters 
 * Display 
   + enable in scene (arrows + mouse)
 * Make clusters (segmented pointclouds) clickable 
   + after clicking on cluster - mark it and enable movement ( w a s d )
 
 ------------------------------------------------------------------------------------------------
 
 Requirements:
   * PCL 1.7 installed on your PC 
 
 For development:
   * Unity3D - free version
   * MS Visual Studio - version compatible with PCL installed
 
 Optional:
   * Kinect
   
 ------------------------------------------------------------------------------------------------
 
 How to use the plugin? 
 
 1. Create a DLL from the .cpp file 
 2. Make sure that the DLL is located in the root folder of your Unity Project (Copy it in Filesystem, not trough Unity)
 3. Copy the pointcloud file into the project directory, or connect Kinect to your PC
 4. Now you can use the plugin (Check the example script how to use it's functionality)
 
 ------------------------------------------------------------------------------------------------
 
 
