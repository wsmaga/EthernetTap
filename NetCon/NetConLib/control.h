#pragma once

enum {
	CAPTURE_SWITCH,
	BRIDGE_SWITCH
};

extern "C" __declspec(dllexport) void sendRequest(int port, int function, bool state);