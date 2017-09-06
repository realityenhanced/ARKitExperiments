#include <opencv2/opencv.hpp>
using namespace cv;
using namespace std;

extern "C"
{
// TMP HELPERS
#ifdef _CONSOLE 
#define DEBUGIMG(mat) DebugDisplayImage(mat);
#else
#define DEBUGIMG(mat)
#endif

#ifdef _CONSOLE
    void DebugDisplayImage(Mat& mat) {
        namedWindow("Display window", WINDOW_KEEPRATIO);
        imshow("Display window", mat);
        waitKey(0);
    }
#endif
// TMP HELPERS

    ////////////// JUST EXERCISING OPENCV ////////////////
    int Test(uchar* imageBuffer, int imageWidth, int imageHeight)
    {
        // Create a Mat object that wraps around the passed in memory.
        Mat input(imageHeight, imageWidth, CV_8UC3, static_cast<void*>(imageBuffer));

        // Convert the image to grayscale.
        cvtColor(input, input, COLOR_RGB2GRAY);

        blur(input, input, Size(3, 3));
        DEBUGIMG(input);

        // Apply a threshold & convert the image to a binary.
        threshold(input, input, 127, 150, THRESH_BINARY);
        DEBUGIMG(input);

        // TODO: Use connectedcomponents to reduce noise.
        // ...

        // Find the contours.
        vector<vector<Point>> contours;
        vector<Vec4i> hierarchy;
        findContours(input, contours, hierarchy, CV_RETR_TREE, CHAIN_APPROX_SIMPLE);

        // Visualize contours
        RNG rng(12345);
        Mat drawing = Mat::zeros(input.size(), CV_8UC3);
        for (int i = 0; i< contours.size(); i++)
        {
            Scalar color = Scalar(rng.uniform(0, 255), rng.uniform(0, 255), rng.uniform(0, 255));
            drawContours(drawing, contours, i, color, 2, 8, hierarchy, 0, Point());
        }
        DEBUGIMG(drawing);

        return 0;
    }

    int ImageMatcher(uchar* imageBuffer, int imageWidth, int imageHeight)
    {
        Mat descriptorsOfImage;
        cv::FileStorage fsRead("input_images\\InputImage.yml", FileStorage::READ);
        fsRead["descriptors"] >> descriptorsOfImage;

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
        cout << goodMatches.size() << endl;
        
        return 0;
    }

    int CalcDescriptors(uchar* imageBuffer, int imageWidth, int imageHeight)
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

        FileStorage fsWrite("input_images\\InputImage.yml", FileStorage::WRITE);
        fsWrite << "descriptors" << descriptors;
        fsWrite.release();

        return 0;
    }

    // ASSUME imageBuffer has 3 Bytes per pixel (RGB) layout
    int PerformOperation(uchar* imageBuffer, int imageWidth, int imageHeight)
    {
        return ImageMatcher(imageBuffer, imageWidth, imageHeight);
    }
}