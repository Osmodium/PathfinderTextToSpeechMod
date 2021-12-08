#include "pch.h"
#include "WindowsVoice.h"

namespace WindowsVoice {

    void speechThreadFunc(int rate, int volume)
    {
        ISpVoice* pVoice = NULL;

        if (FAILED(::CoInitializeEx(NULL, COINITBASE_MULTITHREADED)))
        {
            theStatusMessage = L"Failed to initialize COM for Voice.";
            return;
        }

        HRESULT hr = CoCreateInstance(CLSID_SpVoice, NULL, CLSCTX_ALL, IID_ISpVoice, (void**)&pVoice);
        if (!SUCCEEDED(hr))
        {
            LPSTR pText = 0;

            ::FormatMessage(FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_FROM_SYSTEM | FORMAT_MESSAGE_IGNORE_INSERTS,
                NULL, hr, MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT), pText, 0, NULL);
            LocalFree(pText);
            theStatusMessage = L"Failed to create Voice instance.";
            return;
        }
        theStatusMessage = L"Speech ready.";
        /*
            //std::cout << "Speech ready.\n";
            wchar_t* priorText = nullptr;
            while (!shouldTerminate)
            {
              wchar_t* wText = NULL;
              if (!theSpeechQueue.empty())
              {
                theMutex.lock();
                wText = theSpeechQueue.front();
                theSpeechQueue.pop_front();
                theMutex.unlock();
              }
              if (wText)
              {
                if (priorText == nullptr || lstrcmpW(wText, priorText) != 0)
                {
                  pVoice->Speak(wText, SPF_IS_XML, NULL);
                  Sleep(250);
                  delete[] priorText;
                  priorText = wText;
                }
                else
                  delete[] wText;
              }
              else
              {
                delete[] priorText;
                priorText = nullptr;
                Sleep(50);
              }
            }
            pVoice->Release();
        */
        pVoice->SetRate(rate);
        pVoice->SetVolume(volume);

        SPVOICESTATUS voiceStatus;
        wchar_t* priorText = nullptr;
        while (!shouldTerminate)
        {
            pVoice->GetStatus(&voiceStatus, NULL);
            if (voiceStatus.dwRunningState == SPRS_IS_SPEAKING)
            {
                if (priorText == nullptr)
                    theStatusMessage = L"Error: SPRS_IS_SPEAKING but text is NULL";
                else
                {
                    theStatusMessage = L"Speaking: ";
                    theStatusMessage.append(priorText);
                    if (!theSpeechQueue.empty())
                    {
                        theMutex.lock();
                        if (lstrcmpW(theSpeechQueue.front(), priorText) == 0)
                        {
                            delete[] theSpeechQueue.front();
                            theSpeechQueue.pop_front();
                        }
                        theMutex.unlock();
                    }
                }
            }
            else
            {
                theStatusMessage = L"Waiting.";
                if (priorText != NULL)
                {
                    delete[] priorText;
                    priorText = NULL;
                }
                if (!theSpeechQueue.empty())
                {
                    theMutex.lock();
                    priorText = theSpeechQueue.front();
                    theSpeechQueue.pop_front();
                    theMutex.unlock();
                    pVoice->Speak(priorText, SPF_IS_XML | SPF_ASYNC, NULL);
                }
            }
            Sleep(50);
        }
        pVoice->Pause();
        pVoice->Release();

        theStatusMessage = L"Speech thread terminated.";
    }

    void addToSpeechQueue(const char* text)
    {
        if (text)
        {
            int len = strlen(text) + 1;
            wchar_t* wText = new wchar_t[len];

            memset(wText, 0, len);
            ::MultiByteToWideChar(CP_UTF8, NULL, text, -1, wText, len);

            theMutex.lock();
            theSpeechQueue.push_back(wText);
            theMutex.unlock();
        }
    }

    void clearSpeechQueue()
    {
        theMutex.lock();
        theSpeechQueue.clear();
        theMutex.unlock();
    }

    void initSpeech(int rate, int volume)
    {
        shouldTerminate = false;
        if (theSpeechThread != nullptr)
        {
            theStatusMessage = L"Windows Voice thread already started.";
            return;
        }
        theStatusMessage = L"Starting Windows Voice.";
        theSpeechThread = new thread(WindowsVoice::speechThreadFunc, rate, volume);
    }

    void destroySpeech()
    {
        if (theSpeechThread == nullptr)
        {
            theStatusMessage = L"Speach thread already destroyed or not started.";
            return;
        }
        theStatusMessage = L"Destroying speech.";
        shouldTerminate = true;
        theSpeechThread->join();
        theSpeechQueue.clear();
        delete theSpeechThread;
        theSpeechThread = nullptr;
        CoUninitialize();
        theStatusMessage = L"Speech destroyed.";
    }

    void statusMessage(char* msg, int msgLen)
    {
        size_t count;
        wcstombs_s(&count, msg, msgLen, theStatusMessage.c_str(), msgLen);
    }

    string* getVoicesAvailable()
    {
        vector<string> voices;
        HRESULT hr = S_OK;
        CComPtr<ISpObjectTokenCategory> cpSpCategory = NULL;
        CComPtr<IEnumSpObjectTokens> cpSpEnumTokens = NULL;
        if (SUCCEEDED(hr = SpGetCategoryFromId(SPCAT_VOICES, &cpSpCategory)))
        {
            cpSpCategory->EnumTokens(NULL, NULL, &cpSpEnumTokens);

            CComPtr<ISpObjectToken> cpSpToken;
            unsigned long ulFetched;
            while (SUCCEEDED(hr = cpSpEnumTokens->Next(1, &cpSpToken, &ulFetched)))
            {
                WCHAR* description;
                if (SUCCEEDED(hr = SpGetDescription(cpSpToken, &description)))
                {
                    wstring ws(description);
                    string str(ws.begin(), ws.end());
                    voices.push_back(str);
                }
            }
        }
        return &voices[0];
    }
}

BOOL APIENTRY DllMain(HMODULE, DWORD ul_reason_for_call, LPVOID)
{
    switch (ul_reason_for_call)
    {
    case DLL_PROCESS_ATTACH:
    case DLL_THREAD_ATTACH:
    case DLL_THREAD_DETACH:
    case DLL_PROCESS_DETACH:
        break;
    }

    return TRUE;
}