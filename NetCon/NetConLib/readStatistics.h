#pragma once
#include <iostream>

#include "NetConLib.h"

extern "C" __declspec(dllexport) void startRead(int port, void (*callback)(float framesTime, uint16_t frame_counter, uint16_t valid_frame_counter), int timeInterval);

void readDevice(int dev);

void readFIFO();

extern "C" __declspec(dllexport) void stopRead();

void sendStatsToListener(void (*callback)(float framesTime, uint16_t frame_counter, uint16_t valid_frame_counter), int timeInterval);