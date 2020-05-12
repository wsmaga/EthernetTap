// ToFile.cpp : Ten plik zawiera funkcję „main”. W nim rozpoczyna się i kończy wykonywanie programu.
//

#include <iostream>

#include "open_stream.h"
#include "filters.h"
#include "control.h"
#include "capture.h"
#include "mdio.h"


void showHelp() {
	std::cout << "";
}


int main(int argc, char* argv[])
{
	//TODO: Resetowanie karty NetFPGA poprzez wgrywanie bitstreama na nowo
	//TODO: Stworzenie helpa

	//Typowe wywołania programu
	//program.exe capture 1
	//program.exe capture 1 "test.pcap"
	//program.exe set 1 "1 192 193" - ustaw pierwszy filtr na wychwytywanie ramek w którym wartość pierwszego bajta jest >= 192 i <= 193
	//program.exe set 2 0 "9 192 193;10 10;11 10;"
	//program.exe set 2 0 "9 192 193;10 10;11 10;"

	if (argc > 1) {
		if ((strcmp(argv[1], "capture") == 0) || argv[1][0] == 'c') {
			if (argc == 3) {
				startCapture(std::stoi(argv[2]));
			}
			else if (argc == 4) {
				startCapture(std::stoi(argv[2]), argv[3]);
			}
			else if (argc == 5) {
				startCapture(std::stoi(argv[2]), argv[3], (uint64_t)std::stoi(argv[4]) * 1024 * 1024);
			}
		}
		else if ((strcmp(argv[1], "set") == 0) || (argv[1][0] == 's')) {
			if (argc == 2) {
				std::cout << "Podano zbyt malo argumentow." << std::endl;
				std::cout << "Przyklady:" << std::endl;
				std::cout << "NetCon.exe set nr_portu minimalna_dlugosc_ramki nastawy_filtra " << std::endl;
				std::cout << "NetCon.exe set 1 128 ""8 120 121;9 120"" " << std::endl;
			}
			else if (argc > 2) {
				int port = std::stoi(argv[2], 0, 0);
				int minFrameLen[4] = {0x3FFF, 0x3FFF, 0x3FFF, 0x3FFF};

				std::vector<std::string> vecString;
				if (argc == 3)
					std::cout << "Ustawianie filtrow w trybie blokowania." << std::endl;

				if (argc > 3)
					minFrameLen[0] = std::stoi(argv[3], 0, 0);
				if (argc > 4)
					vecString.push_back(std::string(argv[4]));

				if (argc > 5)
					minFrameLen[1] = std::stoi(argv[5], 0, 0);
				if (argc > 6)
					vecString.push_back(std::string(argv[6]));

				if (argc > 7)
					minFrameLen[2] = std::stoi(argv[7], 0, 0);
				if (argc > 8)
					vecString.push_back(std::string(argv[8]));

				if (argc > 9)
					minFrameLen[3] = std::stoi(argv[9], 0, 0);
				if (argc > 10)
					vecString.push_back(std::string(argv[10]));

				try {
					sendSettings(port, minFrameLen, vecString);
					std::cout << "Wysylanie nastaw filtrow powiodlo sie." << std::endl;
				}
				catch (const char e[]) {
					std::cout << e << std::endl;
				}
				
			}
			return 0;
		}
		else if ((strcmp(argv[1], "bridge_all") == 0) || (strcmp(argv[1], "ba") == 0)) {
			sendRequest(1, BRIDGE_SWITCH, true);
			sendRequest(2, BRIDGE_SWITCH, true);
			sendRequest(3, BRIDGE_SWITCH, true);
			sendRequest(4, BRIDGE_SWITCH, true);
			std::cout << "Zalaczono wszystkie polaczenia bridgowane" << std::endl;
			return 0;
		}
		else if ((strcmp(argv[1], "bridge_on") == 0) || argv[1][0] == 'bo') {
			int port = atoi(argv[2]);
			sendRequest(port, BRIDGE_SWITCH, true);
			std::cout << "Zalaczono tunel nr " << port << "." << std::endl;
			return 0;
		}
		else if ((strcmp(argv[1], "bridge_off") == 0) || argv[1][0] == 'bf') {
			int port = atoi(argv[2]);
			sendRequest(port, BRIDGE_SWITCH, false);
			std::cout << "Wylaczono tunel nr " << port << "." << std::endl;
			return 0;
		}
		else if ((strcmp(argv[1], "measure_time") == 0) || (strcmp(argv[1], "mt") == 0)) {
			return 0;
		}
		else if ((strcmp(argv[1], "monitor_only") == 0) || (strcmp(argv[1], "mo") == 0)) {
			return 0;
		}
		else if (strcmp(argv[1], "mdio") == 0) {
			if (argc > 5) {
				try {
					send_and_receive_mdio(std::stoi(argv[2]), std::stoi(argv[3]), std::stoi(argv[4]), std::stoi(argv[5]));
				}
				catch (const char e[]) {
					std::cout << e << std::endl;
				}
			}
			else if (argc == 5) {
				try {
					send_and_receive_mdio(std::stoi(argv[2]), std::stoi(argv[3]), std::stoi(argv[4]), 0);
				}
				catch (const char e[]) {
					std::cout << e << std::endl;
				}
			}
			else {
				std::cout << "Podano za malo argumentow" << std::endl;
				std::cout << "Argumenty:  ""mdio op_code phy_addr reg_addr data""" << std::endl;
				std::cout << "Przyklad zapisu:  ""mdio 1 1 1 10""" << std::endl;
				std::cout << "Przyklad odczytu: ""mdio 2 1 1""" << std::endl;
			}
			return 0;
		}
		else if (strcmp(argv[1], "eee_off") == 0) {
			try {
				std::cout << "Wysylanie rejestrow wylaczajacych Energy-Efficient Ethernet." << std::endl;
				for (int i = 1; i < 5; i++) {
					std::cout << i << std::endl;
					send_and_receive_mdio(1, i, 13, 7); 
					send_and_receive_mdio(1, i, 14, 60);
					send_and_receive_mdio(1, i, 13, 16391);
					send_and_receive_mdio(1, i, 14, 0);
				}
				std::cout << "Odczytywanie rejestrow Energy-Efficient Ethernet." << std::endl;
				for (int i = 1; i < 5; i++) {
					std::cout << i << std::endl;
					send_and_receive_mdio(1, i, 13, 7);
					send_and_receive_mdio(1, i, 14, 60);
					send_and_receive_mdio(1, i, 13, 16391);
					send_and_receive_mdio(2, i, 14, 0);
				}
				std::cout << "Miekki reset ukladow PHY, potrzebny do zastosowania ustawien." << std::endl;
				for (int i = 1; i < 5; i++) {
					std::cout << i << std::endl;
					send_and_receive_mdio(1, i, 0, 37184);
				}

			}
			catch (const char e[]) {
				std::cout << e << std::endl;
			}
			return 0;
		}
	}
	else {
		showHelp();
	}
}

