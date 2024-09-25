#ifdef DLL_EXPORTS
#define DLL_API __declspec(dllexport)
#else
#define DLL_API __declspec(dllimport)
#endif

#include <mutex>
#include <list>
#include <thread>
#include <sapi.h>
#include <atlbase.h>
#pragma warning(disable:4996)
#include <sphelper.h>
#pragma warning(default: 4996)

using namespace std;

namespace WindowsVoice {
	extern "C" {
		DLL_API void __cdecl initSpeech(int rate, int volume);
		DLL_API void __cdecl addToSpeechQueue(const char* text);
		DLL_API void __cdecl clearSpeechQueue();
		DLL_API void __cdecl destroySpeech();
		DLL_API BSTR __cdecl getStatusMessage();
		DLL_API BSTR __cdecl getVoicesAvailable();
		DLL_API UINT32 __cdecl getWordLength();
		DLL_API UINT32 __cdecl getWordPosition();
		DLL_API UINT32 __cdecl getSpeechState();
	}

	enum class speech_state_enum { uninitialized, ready, speaking, terminated, error };

	mutex theMutex;
	list<wchar_t*> theSpeechQueue;
	thread* theSpeechThread = nullptr;
	bool shouldTerminate = false;
	wstring theStatusMessage;
	ULONG wordLength = 0;
	ULONG wordPosition = 0;
	speech_state_enum speechState = speech_state_enum::uninitialized;
}