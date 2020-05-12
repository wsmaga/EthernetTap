#pragma once

#include "NetConLib.h"

__declspec(dllexport) int open_wstream(const char* configDeviceName, int tryCount = 10);
__declspec(dllexport) int open_rstream(const char* configDeviceName, int tryCount = 10);