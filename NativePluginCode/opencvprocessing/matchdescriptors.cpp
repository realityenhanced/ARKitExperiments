#include <opencv2/opencv.hpp>
#include "common.h"

using namespace cv;
using namespace std;

extern Mat descriptorsOfImage;

extern "C" int MatchDescriptors(uchar* imageBuffer, int imageWidth, int imageHeight)
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
    matcher.match(descriptorsOfImage, descriptors, matches);

    if (matches.size() > 0)
    {
        double maxDist = 0;
        double minDist = 100;
        for (int i = 0; i < descriptors.rows; i++)
        {
            double dist = matches[i].distance;
            if (dist < minDist) minDist = dist;
            if (dist > maxDist) maxDist = dist;
        }
        cout << "Max dist : " << maxDist << endl;
        cout << "Min dist : " << minDist << endl;

        std::vector< DMatch > goodMatches;
        for (int i = 0; i < descriptors.rows; i++)
        {
            if (matches[i].distance <= max(2 * minDist, 0.02))
            {
                goodMatches.push_back(matches[i]);
            }
        }
        cout << "Num good matches = " << goodMatches.size() << endl;
        retVal = (goodMatches.size() > 0) ? 1 : 0;
    }
    else
    {
        cout << "No matches" << endl;
    }
    
    return retVal;
}
