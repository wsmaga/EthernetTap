#pragma once
#include <vector>
#include <string>

#include "NetConLib.h"

void intHandler(int);

void intHandler2(int);

void keyboardInput();

//zmodyfikowano - zmieniono filename na send_callback
extern "C" __declspec(dllexport) int startCapture(int port, int (*send_callback)(const char * blob, int blob_size), const int bufSize = (1024 * 1024 * 16));

extern "C" __declspec(dllexport) int stopCapture();	//symulacja wciœniêcia X