#pragma once
#include "ringBuffer.h"

extern std::atomic<int> exitStatus;

void readDevice(int dev, ringBuffer & rb);
void writeToFile(int file, ringBuffer & rb);
void reportBuffer(ringBuffer & rb);
