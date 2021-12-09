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
#include <vector>
//#include <Windows.h>

using namespace std;

namespace WindowsVoice {
  extern "C" {
    DLL_API void __cdecl initSpeech(int rate, int volume);
    DLL_API void __cdecl addToSpeechQueue(const char* text);
    DLL_API void __cdecl clearSpeechQueue();
    DLL_API void __cdecl destroySpeech();
    DLL_API char* __cdecl getStatusMessage();
    DLL_API char* __cdecl getVoicesAvailable();
  }

  mutex theMutex;
  list<wchar_t*> theSpeechQueue;
  thread* theSpeechThread = nullptr;
  bool shouldTerminate = false;
  wstring theStatusMessage;
}

#define length(array) ((sizeof(array)) / (sizeof(array[0])))