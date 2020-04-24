#pragma once

int open_wstream(const char* configDeviceName, int tryCount = 10);
int open_rstream(const char* configDeviceName, int tryCount = 10);