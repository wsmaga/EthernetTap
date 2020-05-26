#include "pch.h"
#include "framework.h"

#include "captureThreads.h"
#include "ringBuffer.h"
#include <iostream>
#include <string>
#include <thread>
#include <io.h>		//for low-level open, read, write
#include <atomic>	
#include <chrono>	//for time measure and chrono_literals
#include <tuple>

using namespace std::chrono_literals;

std::atomic<int> exitStatus = 0;
std::atomic<bool> captureFlagState = false;

void setCaptureState(bool state) {
	captureFlagState = state;
}

void readDevice(int dev, ringBuffer & rb) {

	while(rb.areSlavesInitialized() == false)
		std::this_thread::sleep_for(1ms);

	while (1) {

		if (captureFlagState) {

			int ret = 0;
			auto val = rb.getMasterMemory();

			while (val.len == 0) {
				val = rb.getMasterMemory();
				std::this_thread::sleep_for(1ms);
			}

			ret = _read(dev, (char*)val.ptr, val.len);

			if (ret <= 0) {
				exitStatus = -4;
				return;
			}

			rb.updateMaster(ret);
		}
		
	}
}

void writeToFile(int file, ringBuffer & rb) {
	rb.initSlave(0);

	while (1) {
		auto val = rb.getSlaveMemory(0);

		while (val.len == 0) { //waiting for some data to read from cyclicBuffer
			val = rb.getSlaveMemory(0);
			std::this_thread::sleep_for(1ms); //1ms * maxIncomingDataRate(130MB/s) = 0.13MB
			if (exitStatus != 0 && val.len == 0) {
				std::this_thread::sleep_for(40ms);
				val = rb.getSlaveMemory(0);
				if(val.len == 0)
					return;
			}
		}

		int ret = _write(file, val.ptr, val.len);

	}
}


void sendToListener(int file, int (*send)(const char * blob, int blob_size),ringBuffer& rb) {
	rb.initSlave(0);

	while (1) {
		auto val = rb.getSlaveMemory(0);

		while (val.len == 0) { //waiting for some data to read from cyclicBuffer
			val = rb.getSlaveMemory(0);
			std::this_thread::sleep_for(1ms); //1ms * maxIncomingDataRate(130MB/s) = 0.13MB
			if (exitStatus != 0 && val.len == 0) {
				std::this_thread::sleep_for(40ms);
				val = rb.getSlaveMemory(0);
				if(val.len == 0)
					return;
			}
		}

		int ret = send(val.ptr, val.len);

		_write(file, val.ptr, val.len);
		//zapis do pliku


		if (ret > 0) {
			rb.updateSlave(0, ret);
		}
		else if (ret <= 0) {
			exitStatus = -5;
		}
	}
}

std::string hAlign(std::string temp, int value) {
	temp.resize(value, ' ');
	return temp;
}

void reportBuffer(ringBuffer & rb) {
	unsigned long long byteCounter = 0;
	unsigned long long lastByteCounter = 0;

	std::chrono::steady_clock::time_point timer = std::chrono::steady_clock::now();
	//std::chrono::seconds sec(1);
	double throughput = 0.0;

	const int hSpace = 25;
	std::cout << hAlign("Licznik", hSpace) << hAlign("Przepustowosc", hSpace) << hAlign("Pelny bufor", hSpace) << hAlign("Maks. zajetosc bufora", hSpace) << "\n";


	while (exitStatus == 0) {

		do {
			byteCounter = rb.getByteCounter();

			auto delay = (std::chrono::steady_clock::now() - timer);
			if (delay > 500ms) {
				timer += delay;
				throughput = ((rb.getByteCounter() - lastByteCounter) / 1024.0 / 1024.0) / std::chrono::duration_cast<std::chrono::duration<float>>(delay).count();
				lastByteCounter = byteCounter;
				break;
			}

			std::this_thread::sleep_for(10ms);
			if (exitStatus != 0)
				return;
		} while (1);

		using namespace std;
		cout << "                                                                                       \r";
		cout << hAlign(to_string(byteCounter), hSpace) << hAlign(to_string(throughput) + " MB/s", hSpace) << hAlign(to_string(rb.getFullBufferCount()) + " razy", hSpace);

		cout << hAlign(to_string(rb.getBufferSize() - rb.getMinFreeMem()) + " bajtow", hSpace) << "\r";
	}
}
