#ifdef DLL_EXPORTS
#define DLL_API __declspec(dllexport)
#else
#define DLL_API __declspec(dllimport)
#endif

#include <mutex>
#include <list>
#include <thread>

namespace WindowsVoice {
  extern "C" {
    DLL_API void __cdecl initSpeech(int rate, int volume);
    DLL_API void __cdecl addToSpeechQueue(const char* text);
    DLL_API void __cdecl clearSpeechQueue();
    DLL_API void __cdecl destroySpeech();
    DLL_API void __cdecl statusMessage(char* msg, int msgLen);
  }

  std::mutex theMutex;
  std::list<wchar_t*> theSpeechQueue;
  std::thread* theSpeechThread = nullptr;
  bool shouldTerminate = false;

  std::wstring theStatusMessage;
}