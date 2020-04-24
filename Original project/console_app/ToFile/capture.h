#pragma once
#include <vector>
#include <string>

void intHandler(int);

void intHandler2(int);

void keyboardInput();

void sendSettings(int port, int minFrameLen[4], std::vector<std::string> vecStr);

int startCapture(int port, const char* fileName = nullptr, const int bufSize = (1024 * 1024 * 16));