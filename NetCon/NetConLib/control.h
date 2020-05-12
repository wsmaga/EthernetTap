#pragma once

enum {
	CAPTURE_SWITCH,
	BRIDGE_SWITCH
};

void sendRequest(int port, int function, bool state);