#include "common.h"

using namespace cv;
using namespace std;

//TEST : INCOMPLETE IMPLN//
extern "C" int CreateMesh(uchar* imageBuffer, int imageWidth, int imageHeight)
{
    // Create a Mat object that wraps around the passed in memory.
    Mat input(imageHeight, imageWidth, CV_8UC3, static_cast<void*>(imageBuffer));

    // Convert the image to grayscale.
    cvtColor(input, input, COLOR_RGB2GRAY);

    blur(input, input, Size(3, 3));

    Canny(input, input, 0, 50, 3);
    DEBUGIMG(input);

    int morph_size = 1;
    Mat element = getStructuringElement(MORPH_RECT, Size(morph_size, morph_size));
    morphologyEx(input, input, MORPH_DILATE, element);
    DEBUGIMG(input);

    // Find the contours.
    vector<vector<Point>> contours;
    vector<Vec4i> hierarchy;
    findContours(input, contours, hierarchy, CV_RETR_TREE, CHAIN_APPROX_SIMPLE);

    const int NUM_CONTOURS_TO_USE = 5;
    if (contours.size() >= NUM_CONTOURS_TO_USE)
    {
        // Choose the largest contours
        std::sort(contours.begin(), contours.end(), [](const vector<Point>& a, const vector<Point>& b) -> bool { return a.size() > b.size(); });
        contours.erase(contours.begin() + NUM_CONTOURS_TO_USE, contours.end());

        // Visualize contours
        RNG rng(12345);
        Mat drawing = Mat::zeros(input.size(), CV_8UC3);
        for (int i = 0; i < contours.size(); i++)
        {
            Scalar color = Scalar(rng.uniform(0, 255), rng.uniform(0, 255), rng.uniform(0, 255));
            drawContours(drawing, contours, i, color, 5);
        }
        DEBUGIMG(drawing);

        // Create a delaunay triangulation from the contour points
        std::sort(contours[0].begin(), contours[0].end(), [](const Point& a, const Point& b) -> bool { return a.y > b.y; });
        Subdiv2D subdivision(Rect(0, 0, input.cols, input.rows));
        for (int i = 0; i < contours[0].size(); ++i)
        {
            subdivision.insert(Point2f(static_cast<float>(contours[0][i].x), static_cast<float>(contours[0][i].y)));
        }
    }

    return 0;
}