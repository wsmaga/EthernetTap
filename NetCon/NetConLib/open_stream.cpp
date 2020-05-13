#include "pch.h"
#include "framework.h"

#include "open_stream.h"
#include <io.h>		//for low-level open, read, write
#include <thread>
#include <chrono>
#include <fcntl.h>  //for _O_RDONLY

__declspec(dllexport) int open_wstream(const char* configDeviceName, int tryCount) {
	using namespace std::chrono_literals;

	int dev = 0;

	do {
		dev = _open(configDeviceName, _O_WRONLY | _O_BINARY);
		if (dev > 0)
			break;
		std::this_thread::sleep_for(10ms);
		tryCount--;
		if (tryCount == 0)
			throw "BLAD: Nie udalo sie otworzyc strumienia do zapisu.";
	} while (1);

	return dev;
}

__declspec(dllexport) int open_rstream(const char* configDeviceName, int tryCount) {
	using namespace std::chrono_literals;

	int dev = 0;

	do {
		dev = _open(configDeviceName, _O_RDONLY | _O_BINARY);
		if (dev > 0)
			break;
		std::this_thread::sleep_for(10ms);
		tryCount--;
		if (tryCount == 0)
			throw "BLAD: Nie udalo sie otworzyc strumienia do odczytu.";
	} while (1);

	return dev;
}