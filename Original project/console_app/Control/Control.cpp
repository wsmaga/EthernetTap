// Control.cpp : Ten plik zawiera funkcję „main”. W nim rozpoczyna się i kończy wykonywanie programu.
//

#include <iostream>
#include <stdio.h>  //for system('cls')
#include <io.h>		//for low-level open, read, write
#include <fcntl.h>  //for _O_RDONLY

int writeBytes() {
	return 0;
}

uint32_t buildFrame(char a, char b, uint16_t len) {
	return 0;
}

struct filterStruct {
	uint8_t c1;
	uint8_t c2;
	uint16_t counter : 12; 
};

int main()
{
	int dev = _open("\\\\.\\xillybus_write_32_config", _O_WRONLY | _O_BINARY);

	if (dev < 0) {
		std::cout << "Opening Netfpga control stream failed." << std::endl;
		return -1;
	}
	else
		std::cout << "Opening Netfpga control stream succeed and waiting" << std::endl;

	while (1) {
		int x;
		std::cout << "Type: 0 - " << std::endl;
		std::cin >> x;
		int ret = 0;
		if (x == 2) {
			char buf[128 * 4];
			memset(buf, 0xff, 128 * 4);
			ret = _write(dev, buf, 128 * 4);
			std::cout << "Send filter config pass all" << std::endl;
			_write(dev, NULL, 0);
		}
		else if (x == 3) {
			char buf[128 * 4];
			memset(buf, 0x00, 128 * 4);
			ret = _write(dev, buf, 128 * 4);
			std::cout << "Send filter config block all" << std::endl;
			_write(dev, NULL, 0);
		}

		else if (x == 4) {
			char buf[128 * 4];
			char filter[4] = { 0b10,0,0x61,0x61 };
			memset(buf, 0x00, 128*4);
			memset(buf, 0xff, 128);

			memcpy(buf, filter, 4);
			ret = _write(dev, buf, 128*4);
			std::cout << "Pass only frames with 'a' on 9 byte position" << std::endl;
			_write(dev, NULL, 0);
		}
		else if (x == 1){
			char buf[] = { 0xFF, 0xFF, 0xFF, 0xFF };
			ret = _write(dev, buf, 4);
			std::cout << "FF send" << std::endl;
			_write(dev, NULL, 0);
		}
		else if (x == 0) {
			char buf[] = { 0x00, 0x00, 0x00, 0x00 };
			ret = _write(dev, buf, 4);
			std::cout << "00 send" << std::endl;
			_write(dev, NULL, 0);
		}
		std::cout << "Ret: " << ret << std::endl;
	}

	return 0;
}

// Uruchomienie programu: Ctrl + F5 lub menu Debugowanie > Uruchom bez debugowania
// Debugowanie programu: F5 lub menu Debugowanie > Rozpocznij debugowanie

// Porady dotyczące rozpoczynania pracy:
//   1. Użyj okna Eksploratora rozwiązań, aby dodać pliki i zarządzać nimi
//   2. Użyj okna programu Team Explorer, aby nawiązać połączenie z kontrolą źródła
//   3. Użyj okna Dane wyjściowe, aby sprawdzić dane wyjściowe kompilacji i inne komunikaty
//   4. Użyj okna Lista błędów, aby zobaczyć błędy
//   5. Wybierz pozycję Projekt > Dodaj nowy element, aby utworzyć nowe pliki kodu, lub wybierz pozycję Projekt > Dodaj istniejący element, aby dodać istniejące pliku kodu do projektu
//   6. Aby w przyszłości ponownie otworzyć ten projekt, przejdź do pozycji Plik > Otwórz > Projekt i wybierz plik sln
