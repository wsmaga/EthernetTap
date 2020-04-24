#include "filters.h"
#include <io.h>
#include <fcntl.h>  //for _O_RDONLY

struct Filter {
	uint8_t c1;
	uint8_t c2;
	uint8_t column;
	uint16_t pos;
	uint16_t counter;
	uint32_t raw;
};

Filter createFilter(int c1, int c2, int counter) {
	Filter temp;
	temp.raw = 0;
	temp.column = 0;

	//Ogarniczenie wartosci countera do 14 bitów
	if ((counter > (UINT16_MAX >> 2)) || (counter < 0))
		throw "BLAD: Bledna pozycja filtru, sprawdz argumenty.";
	if (c1 < 0 || c1 > UINT8_MAX)
		throw "BLAD: Bledna wartosc filtru, sprawdz argumenty.";
	if (c2 < 0 || c2 > UINT8_MAX)
		throw "BLAD: Bledna wartosc filtru, sprawdz argumenty.";

	temp.raw = ((uint32_t)c1 << 24) + ((uint32_t)c2 << 16) + (counter >> 2);
	temp.c1 = c1;
	temp.c2 = c2;
	temp.column = (counter & 0b11);
	temp.pos = (counter >> 2);
	temp.counter = counter;
	return temp;
}

std::vector<std::string> split(const std::string& text, char sep) {
	std::vector<std::string> tokens;
	std::size_t start = 0, end = 0;
	while ((end = text.find(sep, start)) != std::string::npos) {
		if (end != start) {
			tokens.push_back(text.substr(start, end - start));
		}
		start = end + 1;
	}
	if (end != start) {
		tokens.push_back(text.substr(start));
	}
	return tokens;
}

std::vector<Filter> convToFrames(std::vector<std::string> input) {
	std::vector<Filter> filters;

	for (auto const& token : input) {
		auto temp = split(token, ' ');
		if (temp.size() == 3) {
			int pos = std::stoi(temp[0], 0, 0);
			int c1 = std::stoi(temp[1], 0, 0);
			int c2 = std::stoi(temp[2], 0, 0);
			filters.push_back(createFilter(c1, c2, pos));
		}
		if (temp.size() == 2) {
			int pos = std::stoi(temp[0], 0, 0);
			int c1 = std::stoi(temp[1], 0, 0);
			int c2 = c1;
			filters.push_back(createFilter(c1, c2, pos));
		}
	}

	struct {
		bool operator()(Filter a, Filter b) {
			return a.counter < b.counter;
		}
	}filterLess;

	struct {
		bool operator()(Filter a, Filter b) {
			return a.counter == b.counter;
		}
	}filterEuals;

	std::sort(filters.begin(), filters.end(), filterLess);

	//Sprawdz czy pozcyje filtrow s¹ unikalne
	auto ret = std::adjacent_find(filters.begin(), filters.end(), filterEuals);
	if (ret != filters.end()) {
		throw "BLAD: Wykryto powtarzajace sie pozycje filtrow.";
	}

	return filters;
}

void createBuf(char* buf, int minimalFrameLen, std::vector<Filter> filters) {
	const uint8_t maxCountPerColumn = 7;
	const uint8_t maxCountOfFilters = maxCountPerColumn * 4; // 4 columns by 7 fields

	std::vector<Filter> tempFilters[4];

	if (minimalFrameLen > 0x3FFF || minimalFrameLen < 0)
		throw "BLAD: Zla minimalna dlugosc ramki";

	//Wpisywanie minimalnej dlugosci ramki
	uint16_t u = minimalFrameLen;
	int offset = 0;
	memcpy(buf + 2, (char*)& u, 2);
	offset += 16;

	for (auto const& filter : filters) {
		tempFilters[filter.column].push_back(filter);
	}

	for (auto const& column : tempFilters) {
		if (column.size() > maxCountPerColumn)
			throw "BLAD: Za duza ilosc filtrow na kolumne.";
	}

	//memset(buf, 0xFF, maxCountOfFilters * 4);

	for (auto const& filter : tempFilters) {
		for (size_t i = 0; i < filter.size(); ++i)
		{
			memcpy(buf + offset + (size_t)4 * filter[i].column + i * 16, (char*)& filter[i].raw, 4);
		}
	}
}

void sendSettings(int port, int minFrameLen[4], std::vector<std::string> vecStr) {
	char* configPath;
	configPath = (char*)"\\\\.\\xillybus_write_32_1";

	if (port == 2)
		configPath = (char*)"\\\\.\\xillybus_write_32_2";
	if (port == 3)
		configPath = (char*)"\\\\.\\xillybus_write_32_3";
	if (port == 4)
		configPath = (char*)"\\\\.\\xillybus_write_32_4";

	int dev = _open(configPath, _O_WRONLY | _O_BINARY);

	if (dev < 0)
		throw "BLAD: Otwarcie strumienia kontrolnego nie powiodlo sie.";

	if (vecStr.size() > 4)
		throw "BLAD: Za duza liczba filtrow.";
	//TODO: sprawdz inne znaki

	const int sizeOfBuf = 4 * 8 * 4 * 4; //16 byte minPacketLen + 448 bytes of filters
	//const int sizeOfBuf = 512;
	char buf[sizeOfBuf];

	memset(buf, 0xFF, sizeOfBuf);

	int offset = 0;
	int i = 0;
	for (int i = 0; i < 4; i++) {
		std::vector<Filter> framedFilters;
		if (vecStr.size() > i) {
			auto splitedString = split(vecStr.at(i), ';');
			framedFilters = convToFrames(splitedString);
		}
		createBuf(buf + offset, minFrameLen[i], framedFilters);
		offset += 128;
		i++;
	}

	int ret = _write(dev, buf, sizeOfBuf);
	_write(dev, NULL, 0);

	if (ret != sizeOfBuf)
		throw "BLAD: Blad w wysylaniu ustawien filtra. Sprobuj jeszcze raz.";

}
