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
    // ASSUME imageBuffer has 3 Bytes per pixel (RGB) layout
    int PerformOperation(uchar* imageBuffer, int imageWidth, int imageHeight)
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
}
