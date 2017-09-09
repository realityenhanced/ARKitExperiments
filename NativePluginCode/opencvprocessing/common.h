#pragma once
#include <opencv2/opencv.hpp>

// For windows debugging
#ifdef _CONSOLE 
#define DEBUGIMG(mat) DebugDisplayImage(mat);
extern void DebugDisplayImage(cv::Mat& mat);
#else
#define DEBUGIMG(mat)
#endif

extern "C" int SaveDescriptors(uchar* imageBuffer, int numColumns, int numRows);
extern "C" int MatchDescriptors(uchar* imageBuffer, int numColumns, int numRows);
extern "C" int CreateMesh(uchar* imageBuffer, int numColumns, int numRows);