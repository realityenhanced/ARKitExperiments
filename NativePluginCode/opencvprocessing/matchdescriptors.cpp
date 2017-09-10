#include <opencv2/opencv.hpp>
#include "common.h"

using namespace cv;
using namespace std;

extern Mat descriptorsOfImage;

extern "C" int MatchDescriptors(uchar* imageBuffer, int imageWidth, int imageHeight, int* boundingRectangle)
{
    int retVal = 0;
    
    //Mat descriptorsOfImage;
    //cv::FileStorage fsRead("InputImage.yml", FileStorage::READ);
    //fsRead["descriptors"] >> descriptorsOfImage;
    cout << "Descriptor size = " << descriptorsOfImage.rows << " x " << descriptorsOfImage.cols <<endl;

    // Create a Mat object that wraps around the passed in memory.
    Mat input(imageHeight, imageWidth, CV_8UC3, static_cast<void*>(imageBuffer));

    // Convert the image to grayscale.
    cvtColor(input, input, COLOR_RGB2GRAY);

    Ptr<FeatureDetector> detector = ORB::create();
    vector<KeyPoint> keypoints;
    detector->detect(input, keypoints);

    Ptr<DescriptorExtractor> extractor = ORB::create();
    Mat descriptors;
    extractor->compute(input, keypoints, descriptors);
    descriptors.convertTo(descriptors, CV_32F);

    FlannBasedMatcher matcher;
    vector<DMatch> matches;
    matcher.match(/*query*/descriptors, /*train*/descriptorsOfImage, matches);
    if (matches.size() > 0)
    {
        double maxDist = 0;
        double minDist = 100;
        for (int i = 0; i < descriptors.rows; i++)
        {
            double dist = matches[i].distance;
            if (dist < minDist)
            {
                minDist = dist;
            }
            if (dist > maxDist)
            {
                maxDist = dist;
            }
        }
        cout << "Max dist : " << maxDist << endl;
        cout << "Min dist : " << minDist << endl;

        std::vector< DMatch > goodMatches;
        std::vector<Point> goodKeypoints;
        for (int i = 0; i < descriptors.rows; i++)
        {
            if (matches[i].distance <= max(2 * minDist, 0.02))
            {
                goodMatches.push_back(matches[i]);
                goodKeypoints.push_back(Point(keypoints[matches[i].queryIdx].pt.x, keypoints[matches[i].queryIdx].pt.y));
            }
        }
        cout << "Num good matches = " << goodMatches.size() << endl;
        
        Rect boundary = boundingRect(goodKeypoints);
        cout << "BOUNDARY = " << boundary.x << " " << boundary.y << " " << boundary.width << " " << boundary.height << endl;
        boundingRectangle[0] = boundary.x;
        boundingRectangle[1] = boundary.y;
        boundingRectangle[2] = boundary.width;
        boundingRectangle[3] = boundary.height;
        
        // TODO: Find a good heuristic to evaluate matches.
        retVal = (minDist <= 100 && goodMatches.size() >= 4) ? 1 : 0;
    }
    else
    {
        cout << "No matches" << endl;
    }
    
    return retVal;
}
