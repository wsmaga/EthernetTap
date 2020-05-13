#pragma once
#include <atomic>
#include <thread>
#include <array>
#include <tuple>


struct rbStruct {
	int len;
	const char* ptr;
};
//UWAGA: Wszystkie urz¹dznia typu Slave powinny byæ zainicjowane przed zapisywaniem do bufora

class ringBuffer
{
private:
	std::atomic<char*> mp_master;				//WskaŸnik dla w¹tku zapisuj¹cego (Master)
	std::array<std::atomic<char*>, 4> mp_slave;	//WskaŸnik dla w¹tku odczytuj¹cego (Slave), jeden slave na jeden w¹tek
	std::atomic<unsigned long long> m_byteCounter;
	std::atomic<int> m_fullBufferCount;
	std::atomic<int> m_minFreeMem;
	int m_slaveCount;
	const char* mp_memory;						//WskaŸnik na czêœæ pamiêci
	const int m_bufferSize;						//Rozmiar bufora
	const char* mp_endBuffer;					//WskaŸnik koñca bufora


	int calcDistanceFromPtrToMaster(char* ptr);
	int calcDistanceFromMasterToPtr(char* ptr); //Oblicz odleg³oœæ do wskaŸnika master lub koñca pamiêci.
	int calcRealFreeMemory();

	ringBuffer();
public:
	ringBuffer(const char* p_memory, int bufferSize, int slaveCount) :
		mp_master((char*)p_memory),
		m_byteCounter(0),
		m_fullBufferCount(0),
		m_minFreeMem(bufferSize),
		mp_memory(p_memory),
		m_slaveCount(slaveCount),
		m_bufferSize(bufferSize)
	{
		mp_endBuffer = mp_memory + bufferSize - 1;

		for (auto& p : mp_slave) {
			p.store((char*)nullptr);
		}
	};

	bool areSlavesInitialized();
	//Zwraca maksymaln¹ d³ugoœæ do zapisu oraz wskaŸnik rozpoczynaj¹cy
	rbStruct getMasterMemory();
	//Aktualizuje wskaŸnik o dan¹ iloœæ elementów
	void updateMaster(int len);

	//Inicjalizuje bufor dla Slave
	void initSlave(int slaveId);
	//Zwraca maksymaln¹ d³ugoœæ do odczytu oraz wskaŸnik rozpoczynaj¹cy
	rbStruct getSlaveMemory(int slaveId);
	//Aktualizuje wskaŸnik o dan¹ iloœæ elementów
	void updateSlave(int slaveId, int len);

	unsigned long long getByteCounter();
	int getFullBufferCount();
	int getBufferSize();
	int getMinFreeMem();
};
