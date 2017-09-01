#include <opencv2/opencv.hpp>
using namespace cv;

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

        DEBUGIMG(input);

        // Convert the image to grayscale.
        Mat grayscaleImage(imageHeight, imageWidth, CV_8U);
        cvtColor(input, grayscaleImage, COLOR_RGB2GRAY);

        DEBUGIMG(grayscaleImage);

        return 0;
    }
}
