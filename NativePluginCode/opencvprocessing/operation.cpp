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
        namedWindow("Display window", WINDOW_NORMAL);
        imshow("Display window", mat);
        waitKey(0);
    }
#endif
// TMP HELPERS

    ////////////// JUST EXERCISING OPENCV ////////////////
    // ASSUME imageBuffer has 3 Bytes per pixel (RGB) layout
    int PerformOperation(uchar* imageBuffer, int numColumns, int numRows)
    {
        const unsigned int NUM_CHANNELS = 3;
        if (numColumns%NUM_CHANNELS != 0 || numRows%NUM_CHANNELS != 0)
        {
            printf("Invalid image size");
            return -1;
        }

        // Create a Mat object that wraps around the passed in memory.
        Mat input(numRows, numColumns, CV_8UC3, static_cast<void*>(imageBuffer));

        DEBUGIMG(input);

        // Convert the image to grayscale.
        Mat grayscaleImage(numRows/NUM_CHANNELS, numColumns/NUM_CHANNELS, CV_8U);
        cvtColor(input, grayscaleImage, COLOR_RGB2GRAY);

        DEBUGIMG(grayscaleImage);

        return 0;
    }
}
