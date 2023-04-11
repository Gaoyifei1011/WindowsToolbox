#include "DPICalcHelper.h"

/// <summary>
/// 有效像素值转换为实际的像素值
/// </summary>
int DPICalcHelper::ConvertEpxToPixel(HWND hwnd, int effectivePixels)
{
	float scalingFactor = GetScalingFactor(hwnd);
	return (int)(effectivePixels * scalingFactor);
}

/// <summary>
/// 实际的像素值转换为有效像素值
/// </summary>
int DPICalcHelper::ConvertPixelToEpx(HWND hwnd, int pixels)
{
	float scalingFactor = GetScalingFactor(hwnd);
	return (int)(pixels / scalingFactor);
}

/// <summary>
/// 获取实际的系统缩放比例
/// </summary>
float DPICalcHelper::GetScalingFactor(HWND hwnd)
{
	int dpi = GetDpiForWindow(hwnd);
	float scalingFactor = (float)dpi / 96;
	return scalingFactor;
}