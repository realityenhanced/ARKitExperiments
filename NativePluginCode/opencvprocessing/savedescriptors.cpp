#include "common.h"

using namespace cv;
using namespace std;

Mat descriptorsOfImage;

extern "C" int SaveDescriptors(uchar* imageBuffer, int imageWidth, int imageHeight)
{
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

    cout << "Descriptor size = " << descriptors.rows << " x " << descriptors.cols <<endl;
    descriptorsOfImage = descriptors;

    //FileStorage fsWrite("InputImage.yml", FileStorage::WRITE);
    //fsWrite << "descriptors" << descriptors;
    //fsWrite.release();

    return 1;
}
