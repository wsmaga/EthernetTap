#include "pch.h"
#include "framework.h"

#include "mdio.h"
#include <iostream>
//#include <vector>
//#include <array>
#include <string>
#include <bitset>
#include <io.h>		//for write and read
#include <chrono>	//for time measure and chrono_literals
#include <thread>	//for sleep
#include "open_stream.h" //for open_wstream and open_rstream

__declspec(dllexport) void send_and_receive_mdio(int op_code, int phy_addr, int reg_addr, int data){
	uint32_t raw = 0;

	//Tworzenie ramki
	//Write 01 01 00001 00111 xx ‭00000000 00000000‬
	//Read  01 10 00001 00111 xx ‭00000000 00000000‬
	//start[31:30] op_code[29:28] phy_addr[27:23] reg_addr[22:18] turn_around[17:16] data[15:0]
	if (op_code > 3 || op_code < 0)
		throw "MDIO: Bledny kod OP";
	if (phy_addr > 4 || phy_addr == 0)
		throw "MDIO: Bledny adres modulu PHY.";
	if (reg_addr < 0 || reg_addr > 31)
		throw "MDIO: Bledny adres rejestru";
	if (data > UINT16_MAX || data < 0)
		throw "MDIO: Bledne dane";

	raw = (1u << 30) | (op_code << 28) | (phy_addr << 23) | (reg_addr << 18) | (1u << 17) | data;

	int dev_write = open_wstream("\\\\.\\xillybus_write_32_mdio");
	int dev_read = open_rstream("\\\\.\\xillybus_read_32_mdio");

	std::cout << "Wysylanie po MDIO: 0x" << std::hex << raw << " Data: 0b" << std::bitset<16>(raw) << std::endl;
	int ret1 = _write(dev_write, (char*)&raw, 4);

	uint32_t response = 0;
	int ret2 = _read(dev_read, (char*)&response, 4);
	std::cout << "Odebrano po  MDIO: 0x" << std::hex << response << " Data: 0b" << std::bitset<16>(response) << std::endl;

	if (ret1 != 4 || ret2 != 4)
		throw "MDIO: Blad wysylania/odbierania rejestrow";

	using namespace std::chrono_literals;
	std::this_thread::sleep_for(100ms);

	_close(dev_write);
	_close(dev_read);
}