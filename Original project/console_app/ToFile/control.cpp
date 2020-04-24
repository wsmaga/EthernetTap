#include "control.h"
#include "open_stream.h" //for open_wstrem
#include <io.h>		//for low-level open, read, write

void sendRequest(int port, int function, bool state) {
	const char configDeviceName[] = "\\\\.\\xillybus_write_32_config";

	//Otwieranie strumienia
	int dev = open_wstream(configDeviceName);

	//Tworzenie ramki
	char buf[] = { 0x00, 0x00, 0x00, 0x00 };

	port -= 1;
	switch (function) {
	case CAPTURE_SWITCH:
		buf[3] = (state << 7);
		buf[0] = (1 << port);
		break;
	case BRIDGE_SWITCH:
		buf[3] = (state << 7);
		buf[0] = (1 << port) << 4;
		break;
	}

	//Wysylanie danych
	int tryCount = 10;
	int ret = 0;
	do {
		ret = _write(dev, buf, 4);
		tryCount--;
		if (tryCount == 0)
			throw "BLAD: Wysylanie danych nie powiodlo sie.";
	} while (ret != 4);

	_close(dev);
}