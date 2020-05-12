#pragma once
__declspec(dllexport) void send_and_receive_mdio(int op_code, int phy_addr, int reg_addr, int data);