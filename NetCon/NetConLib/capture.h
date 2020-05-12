#pragma once
#include <vector>
#include <string>

#include "NetConLib.h"

__declspec(dllexport) void intHandler(int);

__declspec(dllexport) void intHandler2(int);

__declspec(dllexport) void keyboardInput();

__declspec(dllexport) void sendSettings(int port, int minFrameLen[4], std::vector<std::string> vecStr);

extern "C" __declspec(dllexport) int startCapture(int port, const char* fileName = nullptr, const int bufSize = (1024 * 1024 * 16));

extern "C" __declspec(dllexport) int stopCapture();	//symulacja wciœniêcia X