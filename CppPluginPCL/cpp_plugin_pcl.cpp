#define EXPORT_API __declspec(dllexport) // Visual Studio needs annotating exported functions with this

#include <iostream>
#include <pcl/ModelCoefficients.h>
#include <pcl/io/pcd_io.h>
#include <pcl/io/openni_grabber.h>
#include <pcl/point_types.h>
#include <pcl/filters/extract_indices.h>
#include <pcl/filters/voxel_grid.h>
#include <pcl/features/normal_3d.h>
#include <pcl/kdtree/kdtree.h>
#include <pcl/sample_consensus/method_types.h>
#include <pcl/sample_consensus/model_types.h>
#include <pcl/segmentation/sac_segmentation.h>
#include <pcl/segmentation/extract_clusters.h>
#include <pcl/visualization/cloud_viewer.h> // TODO uncomment
#include <pcl/common/time.h>

// Link following functions C-style (required for plugins)
extern "C"
{
	
	 class KinectCloudGrabber
	 {
		pcl::Grabber* interface;
		bool viewerWasStopped;
		
	   public:
		 pcl::PointCloud<pcl::PointXYZ>::ConstPtr cloudFromKinect;

		 KinectCloudGrabber () : viewerWasStopped (false) /*, cloudFromKinect (new pcl::PointCloud<pcl::PointXYZ>) */ { }

		 void cloud_cb_ (const pcl::PointCloud<pcl::PointXYZ>::ConstPtr &cloud)
		 {
			 if (!viewerWasStopped) {
				cloudFromKinect = cloud;
				viewerWasStopped = true;
			 }
		 }

		 pcl::PointCloud<pcl::PointXYZ>::ConstPtr grabCloud ()
		 {
		   pcl::Grabber* interface = new pcl::OpenNIGrabber();

		   boost::function<void (const pcl::PointCloud<pcl::PointXYZ>::ConstPtr&)> f = boost::bind (&KinectCloudGrabber::cloud_cb_, this, _1);

		   interface->registerCallback (f);
		   interface->start ();

		   while (!viewerWasStopped)
		   {
			 boost::this_thread::sleep (boost::posix_time::seconds (1));
		   }

		   interface->stop ();

		   return cloudFromKinect;
		 }
	 };

	pcl::PointCloud<pcl::PointXYZ>::Ptr maincloud (new pcl::PointCloud<pcl::PointXYZ>);
	
	pcl::PointCloud<pcl::PointXYZ>::Ptr *clusteredclouds;
	std::vector<std::vector<int>> clusteredcloudsindices;

	int clustersCount = 0;

	struct mycloud 
    { 
        int size;
		float *resultVertsX, *resultVertsY, *resultVertsZ;
    };

	EXPORT_API bool structureTest(mycloud* myc) {
		mycloud mc; 
		mc.size = 2; 
		mc.resultVertsX = new float[mc.size];
		mc.resultVertsX[0] = 1.1f;
		mc.resultVertsX[1] = 2.2f;
		mc.resultVertsY = new float[mc.size];
		mc.resultVertsY[0] = 3.3f;
		mc.resultVertsY[1] = 4.4f;
		mc.resultVertsZ = new float[mc.size];
		mc.resultVertsZ[0] = 5.1f;
		mc.resultVertsZ[1] = 5.2f;

		*myc = mc;
		return true;
	}

	/*
	int main() {
		mycloud m; 
		structureTest(&m);

		std::cout << m.size << std::endl;
		getchar();
		return 0;
	}
	*/

	EXPORT_API bool readCloud() {
		pcl::PCDReader reader;
		pcl::PointCloud<pcl::PointXYZ>::Ptr cloud (new pcl::PointCloud<pcl::PointXYZ>);
		reader.read ("table_scene_lms400.pcd", *cloud);

		if (cloud->empty()) {
			return false;
		}
	
		// Create the filtering object: downsample the dataset using a leaf size of 1cm
		pcl::VoxelGrid<pcl::PointXYZ> vg;
		vg.setInputCloud (cloud);
		vg.setLeafSize (0.01f, 0.01f, 0.01f);
		vg.filter (*maincloud);
		
		return true;
	}

	EXPORT_API bool readKinectCloud() {
		KinectCloudGrabber kinectCloudGrabber;
		pcl::PointCloud<pcl::PointXYZ>::ConstPtr cloud = kinectCloudGrabber.grabCloud ();
		// std::cout << "Cloudsize :" << cloud->points.size () <<  std::endl;
		   
		if (cloud->empty()) {
			return false;
		}
	
		// Create the filtering object: downsample the dataset using a leaf size of 1cm
		pcl::VoxelGrid<pcl::PointXYZ> vg;
		vg.setInputCloud (cloud);
		vg.setLeafSize (0.01f, 0.01f, 0.01f);
		vg.filter (*maincloud);
		
		return true;
	}

	EXPORT_API bool removeBiggestPlane(int maxIterations, double distanceThreshold) {
		pcl::PointCloud<pcl::PointXYZ>::Ptr cloud_f (new pcl::PointCloud<pcl::PointXYZ>);

		// Create the segmentation object for the planar model and set all the parameters
		pcl::SACSegmentation<pcl::PointXYZ> seg;
		pcl::PointIndices::Ptr inliers (new pcl::PointIndices);
		pcl::ModelCoefficients::Ptr coefficients (new pcl::ModelCoefficients);
		pcl::PointCloud<pcl::PointXYZ>::Ptr cloud_plane (new pcl::PointCloud<pcl::PointXYZ> ());
		pcl::PCDWriter writer;
		seg.setOptimizeCoefficients (true);
		seg.setModelType (pcl::SACMODEL_PLANE);
		seg.setMethodType (pcl::SAC_RANSAC);
		seg.setMaxIterations (maxIterations); // 100);
		seg.setDistanceThreshold (distanceThreshold); // 0.02);

		int i=0, nr_points = (int) maincloud->points.size ();
		while (maincloud->points.size () > 0.3 * nr_points)
		{
			// Segment the largest planar component from the remaining cloud
			seg.setInputCloud (maincloud);
			seg.segment (*inliers, *coefficients);
			if (inliers->indices.size () == 0)
			{
				// std::cout << "Could not estimate a planar model for the given dataset." << std::endl;
				break;
			}

			// Extract the planar inliers from the input cloud
			pcl::ExtractIndices<pcl::PointXYZ> extract;
			extract.setInputCloud (maincloud);
			extract.setIndices (inliers);
			extract.setNegative (false);

			// Get the points associated with the planar surface
			extract.filter (*cloud_plane);
			// std::cout << "PointCloud representing the planar component: " << cloud_plane->points.size () << " data points." << std::endl;

			// Remove the planar inliers, extract the rest
			extract.setNegative (true);
			extract.filter (*cloud_f);
			*maincloud = *cloud_f;
		}

		return true;
	}

	EXPORT_API bool getClusters(double clusterTolerance, int minClusterSize, int maxClusterSize) {
		// Creating the KdTree object for the search method of the extraction
		pcl::search::KdTree<pcl::PointXYZ>::Ptr tree (new pcl::search::KdTree<pcl::PointXYZ>);
		tree->setInputCloud (maincloud);

		std::vector<pcl::PointIndices> cluster_indices;
		pcl::EuclideanClusterExtraction<pcl::PointXYZ> ec;
		ec.setClusterTolerance (clusterTolerance); // 0.02); // 2cm
		ec.setMinClusterSize (minClusterSize); // 100);
		ec.setMaxClusterSize (maxClusterSize); // 25000);
		ec.setSearchMethod (tree);
		ec.setInputCloud (maincloud);
		ec.extract (cluster_indices);

		int j = 0;
		
		clustersCount = cluster_indices.size();
		clusteredclouds = new pcl::PointCloud<pcl::PointXYZ>::Ptr[clustersCount];
		clusteredcloudsindices.resize(clustersCount);

		for (std::vector<pcl::PointIndices>::const_iterator it = cluster_indices.begin (); it != cluster_indices.end (); ++it)
		{
			pcl::PointCloud<pcl::PointXYZ>::Ptr cloud_cluster (new pcl::PointCloud<pcl::PointXYZ>);

			for (std::vector<int>::const_iterator pit = it->indices.begin (); pit != it->indices.end (); pit++) {
				cloud_cluster->points.push_back (maincloud->points[*pit]); //*
				clusteredcloudsindices[j].push_back(*pit);
			}

			cloud_cluster->width = cloud_cluster->points.size ();
			cloud_cluster->height = 1;
			cloud_cluster->is_dense = true;

			// std::cout << "PointCloud representing the Cluster: " << cloud_cluster->points.size () << " data points." << std::endl;

			clusteredclouds[j] = cloud_cluster;
			
			j++;
		}

		return true;
	}

	EXPORT_API int getClustersCount() {
		return clustersCount;
	}

	EXPORT_API int getCloudSize() {
		return maincloud->points.size();
	}

	EXPORT_API bool getCluster(int clusterIndex, float** resultVertsX, float** resultVertsY, float** resultVertsZ, int* resultVertLength) 
	{
		pcl::PointCloud<pcl::PointXYZ>::Ptr cluster = clusteredclouds[clusterIndex];

		if (cluster->empty()) {
			return false;
		}

		float* rVertsX = new float[cluster->points.size ()];
		float* rVertsY = new float[cluster->points.size ()];
		float* rVertsZ = new float[cluster->points.size ()];

		for (size_t i = 0; i < cluster->points.size (); ++i)
		{
			rVertsX[i] = cluster->points[i].x;
			rVertsY[i] = cluster->points[i].y;
			rVertsZ[i] = cluster->points[i].z;
		}

		*resultVertsX = rVertsX;
		*resultVertsY = rVertsY;
		*resultVertsZ = rVertsZ;

		int rVertLength = cluster->points.size ();
		*resultVertLength = rVertLength;
		
		return true;
	}

	EXPORT_API bool getClusterIndices(int clusterIndex, float** indices, int* indicesLength) 
	{
		std::vector<int> clusteredcloudindices = clusteredcloudsindices[clusterIndex];

		if (clusteredcloudindices.empty()) {
			return false;
		}

		float* inds = new float[clusteredcloudindices.size ()];

		for (size_t i = 0; i < clusteredcloudindices.size (); ++i)
		{
			inds[i] = clusteredcloudindices[i];
		}

		*indices = inds;

		int indLength = clusteredcloudindices.size ();
		*indicesLength = indLength;
		
		return true;
	}

	EXPORT_API bool getCloud(float** resultVertsX, float** resultVertsY, float** resultVertsZ, int* resultVertLength) 
	{
		if (maincloud->empty()) {
			return false;
		}

		float* rVertsX = new float[maincloud->points.size ()];
		float* rVertsY = new float[maincloud->points.size ()];
		float* rVertsZ = new float[maincloud->points.size ()];

		for (size_t i = 0; i < maincloud->points.size (); ++i)
		{
			rVertsX[i] = maincloud->points[i].x;
			rVertsY[i] = maincloud->points[i].y;
			rVertsZ[i] = maincloud->points[i].z;
		}

		*resultVertsX = rVertsX;
		*resultVertsY = rVertsY;
		*resultVertsZ = rVertsZ;

		int rVertLength = maincloud->points.size ();
		*resultVertLength = rVertLength;
		
		return true;
	}

	EXPORT_API bool readPointCloud(float** resultVertsX, float** resultVertsY, float** resultVertsZ, int* resultVertLength)
	{
		// Read in the cloud data
		pcl::PCDReader reader;
		pcl::PointCloud<pcl::PointXYZ>::Ptr cloud (new pcl::PointCloud<pcl::PointXYZ>);
		reader.read ("table_scene_lms400.pcd", *cloud);

		if (cloud->empty()) {
			return false;
		}

		float* rVertsX = new float[cloud->points.size ()];
		float* rVertsY = new float[cloud->points.size ()];
		float* rVertsZ = new float[cloud->points.size ()];

		for (size_t i = 0; i < cloud->points.size (); ++i)
		{
			rVertsX[i] = cloud->points[i].x;
			rVertsY[i] = cloud->points[i].y;
			rVertsZ[i] = cloud->points[i].z;
		}

		*resultVertsX = rVertsX;
		*resultVertsY = rVertsY;
		*resultVertsZ = rVertsZ;

		int rVertLength = cloud->points.size ();
		*resultVertLength = rVertLength;
		
		return true;
	}

	 /*
	 EXPORT_API bool grabPointCloudFromKinect(float** resultVertsX, float** resultVertsY, float** resultVertsZ, int* resultVertLength)
	{

		   KinectCloudGrabber kinectCloudGrabber;
		   pcl::PointCloud<pcl::PointXYZ>::ConstPtr cloud = kinectCloudGrabber.grabCloud ();
		   std::cout << "Cloudsize :" << cloud->points.size () <<  std::endl;
		   
		if (cloud->empty()) {
			return false;
		}

		float* rVertsX = new float[cloud->points.size ()];
		float* rVertsY = new float[cloud->points.size ()];
		float* rVertsZ = new float[cloud->points.size ()];

		for (size_t i = 0; i < cloud->points.size (); ++i)
		{
			rVertsX[i] = cloud->points[i].x;
			rVertsY[i] = cloud->points[i].y;
			rVertsZ[i] = cloud->points[i].z;
		}

		*resultVertsX = rVertsX;
		*resultVertsY = rVertsY;
		*resultVertsZ = rVertsZ;

		int rVertLength = cloud->points.size ();
		*resultVertLength = rVertLength;
		
		return true;
	}
	 */

	int main() {
		
		readCloud();
		removeBiggestPlane (100, 0.02);
		getClusters (0.02, 100, 25000);
		getClustersCount();
		
		for (size_t i = 0; i < clusteredcloudsindices[0].size (); ++i)
		{
			std::cout << " : " << clusteredcloudsindices[0][i];
		}

		std::cout << std::endl;

		std::cout << "0 -> " << clusteredclouds[0]->size() << std::endl;
		std::cout << "1 -> " << clusteredclouds[1]->size() << std::endl;
		std::cout << "2 -> " << clusteredclouds[2]->size() << std::endl;
		std::cout << "3 -> " << clusteredclouds[3]->size() << std::endl;
		std::cout << "4 -> " << clusteredclouds[4]->size() << std::endl;
		
		getchar();
		return 0;
	}

} // end of export C block