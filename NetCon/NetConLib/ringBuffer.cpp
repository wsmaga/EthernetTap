#include "pch.h"


#include "ringBuffer.h"

int ringBuffer::calcDistanceFromMasterToPtr(char* ptr) {
	char* p_masterBuf = mp_master;
	if ((ptr - p_masterBuf) == 1) { //FULL
		m_fullBufferCount++;
		return 0;
	}
	else if (p_masterBuf >= ptr) {
		//[   ][   rd][   ][wr  ][   ][  end ] end - wr = 2 so + 1 to get 3 free elements to the end
		return (int)((mp_endBuffer - p_masterBuf) + 1);
	}
	else {
		//[   ][   wr][   ][rd  ][   ][  end ] rd - wr = 2 so -1 because it is almost full and wr == rd means empty
		return  (int)((ptr - p_masterBuf) - 1);
	}
}

int ringBuffer::calcRealFreeMemory()
{
	int minLen = m_bufferSize;

	for (const auto& p : mp_slave) {
		char* p_buf = p;
		if (p_buf != nullptr) {
			int len = 0;
			if (p_buf > mp_master)
				len = p_buf - mp_master - 1;
			else
				len = m_bufferSize - (mp_master - p_buf);

			if (len < minLen)
				minLen = len;
		}
	}
	return minLen;
}

int ringBuffer::calcDistanceFromPtrToMaster(char* ptr) {
	char* p_masterBuf = mp_master;
	if (p_masterBuf > ptr) {
		//[   ][rd x][  x][wr  ][   ][   ] wr - rd = 2 elements in buffer
		return (int)(p_masterBuf - ptr);
	}
	else if (ptr > p_masterBuf) {
		//[  x][wr ][] .. [rd x][  x][end x] rd - end = 2 so + 1 to get 3 last elements from buffer
		return (int)((mp_endBuffer - ptr) + 1);
	}
	return 0;
}

bool ringBuffer::areSlavesInitialized()
{
	int count = 0;
	for (const auto& p : mp_slave) {
		if (p != nullptr)
			count++;
	}
	return count == m_slaveCount;
}

rbStruct ringBuffer::getMasterMemory() {
	int id = -1;
	int minLen = m_bufferSize; //minimalna d³ugoœæ wolnej pamiêci
	char* p_buf = nullptr;

	for (const auto& p : mp_slave) {
		p_buf = p;
		if (p_buf != nullptr) {
			int dist = calcDistanceFromMasterToPtr(p_buf);
			if (dist <= minLen) 
				minLen = dist;
		}
	}

	if (minLen == 0)
		m_fullBufferCount++;

	if (minLen < m_minFreeMem) {
		int temp = calcRealFreeMemory();
		if (temp < m_minFreeMem)
			m_minFreeMem = temp;
	}
		


	rbStruct r = { minLen, mp_master};
	return r;
}

void ringBuffer::updateMaster(int len) {
	m_byteCounter += len;
	char* p_buf = mp_master;
	p_buf += len;
	if (p_buf > mp_endBuffer)
		p_buf = (char*)mp_memory;
	mp_master.store(p_buf);
};


void ringBuffer::initSlave(int slaveId)
{
	mp_slave[slaveId] = (char*)mp_memory;
}


rbStruct ringBuffer::getSlaveMemory(int slaveId) {
	char* p_buf = mp_slave[slaveId];
	rbStruct r = { calcDistanceFromPtrToMaster(p_buf), p_buf };
	return r;
};


void ringBuffer::updateSlave(int slaveId, int len) {
	char* p_buf = mp_slave[slaveId];
	p_buf += len;
	if (p_buf > mp_endBuffer)
		p_buf = (char*)mp_memory;
	mp_slave[slaveId].store(p_buf);
}


unsigned long long ringBuffer::getByteCounter() {
	return m_byteCounter;
}


int ringBuffer::getFullBufferCount() {
	return m_fullBufferCount;
}


int ringBuffer::getBufferSize()
{
	return m_bufferSize;
}


int ringBuffer::getMinFreeMem()
{
	return m_minFreeMem;
}
