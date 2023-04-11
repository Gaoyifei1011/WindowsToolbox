#pragma once

#include <Windows.h>

/// <summary>
/// DPI（每英寸点数）缩放计算辅助类
/// </summary>
class DPICalcHelper
{
public:
	static int ConvertEpxToPixel(HWND hwnd, int effectivePixels);
	static int ConvertPixelToEpx(HWND hwnd, int pixels);
private:
	static float GetScalingFactor(HWND hwnd);
};
