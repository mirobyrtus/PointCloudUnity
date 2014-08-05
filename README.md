 
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
 
     ** Drivers and libraries needed to install PCL 1.7
     + TODO 
     + Copy the link with dependencies here? 

 For development:
   * Unity3D - free version
   * MS Visual Studio - version compatible with PCL installed
 
 Optional:
   * Kinect
   
   ** If you want to use Kinect in this project, you also will need to install
   + TODO official kinect Drivers so that you computer can recognize Kinect
   
 ------------------------------------------------------------------------------------------------
 
 How to use the plugin? 
 
 1. Create a DLL from the .cpp file 
 2. Make sure that the DLL is located in the root folder of your Unity Project (Copy it in Filesystem, not trough Unity)
 3. Copy the pointcloud file into the project directory, or connect Kinect to your PC
 3.1. If you want to use Kinect, you have to check the "Use Kinect" checkbox in the Plugin Script settings
      If it's not checked, it will load a Pointcloud from file "table_scene_lms400.pcd" (It's available in the UnityPCL directory)
 4. Now you can use the plugin (Check the example script how to use it's functionality)
 
 ------------------------------------------------------------------------------------------------
 
 
