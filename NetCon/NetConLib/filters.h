#pragma once
#include <iostream>
#include <vector>
#include <array>
#include <string>


//Filter createFilter(int c1, int c2, int counter);
//
//std::vector<std::string> split(const std::string& text, char sep);
//
//std::vector<Filter> convToFrames(std::vector<std::string> input);
//
//void createBuf(char* buf, int minimalFrameLen, std::vector<Filter> filters);

void sendSettings(int port, int minFrameLen[4], std::vector<std::string> vecStr);

extern "C" __declspec(dllexport) void sendSettingsWrapper(int argc, char**argv );
