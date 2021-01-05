#include "pch.h"

#include "readStatistics.h"
#include <thread>
#include <chrono>
#include <conio.h>
#include <io.h>
#include <fcntl.h>
#include <csignal>
#include <future>
#include <queue>

#include "control.h"

using namespace std::chrono_literals;

int exitStatus = 0;

std::queue<uint64_t> queue;

int openedPort = 0;

uint32_t frames = 0;
uint32_t validFrames = 0;
uint64_t framesTime = 0;

__declspec(dllexport) void startRead(int port, void (*callback)(float framesTime, uint16_t frame_counter, uint16_t valid_frame_counter), int timeInterval) {
	openedPort = port;

	const char* deviceName;

	switch (port) {
	case 1:
		deviceName = "\\\\.\\xillybus_read_32_statistics_1";
		break;
	case 2:
		deviceName = "\\\\.\\xillybus_read_32_statistics_2";
		break;
	case 3:
		deviceName = "\\\\.\\xillybus_read_32_statistics_3";
		break;
	case 4:
		deviceName = "\\\\.\\xillybus_read_32_statistics_4";
		break;
	default:
		std::cout << "Bledny numer portu\n";
		return;
		break;
	}

	int dev = _open(deviceName, _O_RDONLY | _O_BINARY);

	if (dev < 0) {
		std::cout << "Otwieranie strumienia NetFPGA " << deviceName << " nie powiodlo sie." << std::endl;
		exitStatus = -1;
	}
	else {
		std::cout << "Otwarcie strumienia NetFPGA " << deviceName << " powiodlo sie.\n" << std::endl;
	}

	std::thread readThread(readDevice, dev);
	std::thread readQueueThread(readFIFO);
	std::thread sendToListenerThread(sendStatsToListener, callback, timeInterval);

	//sendRequest(port, CAPTURE_SWITCH, true);
	while (exitStatus >= 0) {
		std::this_thread::sleep_for(1ms);
	}
	//sendRequest(port, CAPTURE_SWITCH, false);

	std::cout << "Zamykanie programu\n";
	readQueueThread.join();
	sendToListenerThread.join();

	std::cout << "Zamykanie watku readThread." << std::endl;
	auto future = std::async(std::launch::async, &std::thread::join, &readThread);
	if (future.wait_for(std::chrono::seconds(1)) == std::future_status::timeout) {	}

	_close(dev);
}

void readDevice(int dev) {
	int ret = 0;
	uint64_t data = 0;

	while (exitStatus >= 0) {
		ret = _read(dev, &data, sizeof(data));

		if (ret < 0)
			exitStatus = -1;

		else {
			queue.push(data);
		}
	}
}

void readFIFO() {
	uint64_t data = 0;

	while (exitStatus >= 0) {
		if (!queue.empty()) {
			data = queue.front();
			queue.pop();

			framesTime = (uint32_t)(data);
			validFrames = (uint16_t)(data >> 48);
			frames = (uint16_t)(data >> 32);
		}
	}
}

__declspec(dllexport) void stopRead() {
	exitStatus = -1;
}

void sendStatsToListener(void(*callback)(float framesTime, uint16_t frame_counter, uint16_t valid_frame_counter), int timeInterval) {
	while (exitStatus >= 0) {
		callback(16.0 * framesTime / 1000000000.0, frames, validFrames);

		frames = 0;
		validFrames = 0;

		std::this_thread::sleep_for(100ms);
	}
}
