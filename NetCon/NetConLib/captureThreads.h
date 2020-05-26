#pragma once
#include "ringBuffer.h"

extern std::atomic<int> exitStatus;

extern std::atomic<bool> captureFlagState;

void readDevice(int dev, ringBuffer & rb);
void writeToFile(int file, ringBuffer & rb);
void reportBuffer(ringBuffer & rb);

void sendToListener(int file, int (*send)(const char * blob, int blob_size), ringBuffer& rb);

extern "C" __declspec(dllexport) void setCaptureState(bool state);
