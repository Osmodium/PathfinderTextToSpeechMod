#include "pch.h"
#include "WindowsVoice.h"

namespace WindowsVoice
{
	char* convertWstring(wstring w_str)
	{
		const wchar_t* input = w_str.c_str();
		const size_t size = (wcslen(input) + 1) * sizeof(wchar_t);
		char* buffer = new char[size];

#ifdef __STDC_LIB_EXT1__
		// wcstombs_s is only guaranteed to be available if __STDC_LIB_EXT1__ is defined
		size_t convertedSize;
		wcstombs_s(&convertedSize, buffer, size, input, size);
#else
		wcstombs(buffer, input, size);
#endif
		return buffer;
	}

	void speechThreadFunc(const int rate, const int volume)
	{
		ISpVoice* pVoice = nullptr;

		if (FAILED(::CoInitializeEx(NULL, COINITBASE_MULTITHREADED)))
		{
			theStatusMessage = L"Failed to initialize COM for Voice.";
			return;
		}

		const HRESULT hr = CoCreateInstance(CLSID_SpVoice, nullptr, CLSCTX_ALL, IID_ISpVoice, reinterpret_cast<void**>(&pVoice));
		if (!SUCCEEDED(hr))
		{
			const LPSTR pText = 0;

			::FormatMessage(FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_FROM_SYSTEM | FORMAT_MESSAGE_IGNORE_INSERTS,
				nullptr, hr, MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT), pText, 0, nullptr);
			LocalFree(pText);
			theStatusMessage = L"Failed to create Voice instance.";
			return;
		}

		theStatusMessage = L"Speech ready.";

		pVoice->SetRate(rate);
		pVoice->SetVolume(volume);

		SPVOICESTATUS voiceStatus;
		wchar_t* priorText = nullptr;
		while (!shouldTerminate)
		{
			pVoice->GetStatus(&voiceStatus, 0);
			if (voiceStatus.dwRunningState == SPRS_IS_SPEAKING)
			{
				if (priorText == nullptr)
					theStatusMessage = L"Error: SPRS_IS_SPEAKING but text is NULL";
				else
				{
					theStatusMessage = L"Speaking";
					//theStatusMessage.append(priorText);
					length = voiceStatus.ulInputSentLen;
					position = voiceStatus.ulInputSentPos;
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
				theStatusMessage = L"Waiting";
				if (priorText != nullptr)
				{
					delete[] priorText;
					priorText = nullptr;
				}
				if (!theSpeechQueue.empty())
				{
					theMutex.lock();
					priorText = theSpeechQueue.front();
					theSpeechQueue.pop_front();
					theMutex.unlock();
					pVoice->Speak(priorText, SPF_IS_XML | SPF_ASYNC, nullptr);
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
			const int len = strlen(text) + 1;
			const auto wText = new wchar_t[len];

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
		theSpeechThread = new thread(speechThreadFunc, rate, volume);
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

	char* getStatusMessage()
	{
		if (theStatusMessage.empty())
		{
			theStatusMessage = L"WindowsVoice not yet initialized!";
		}
		return convertWstring(theStatusMessage);
	}

	char* getVoicesAvailable()
	{
		wstring voices;
		HRESULT hr;
		CComPtr<ISpObjectTokenCategory> cpSpCategory = nullptr;
		if (SUCCEEDED(hr = SpGetCategoryFromId(SPCAT_VOICES, &cpSpCategory)))
		{
			CComPtr<IEnumSpObjectTokens> cpSpEnumTokens;
			if (SUCCEEDED(hr = cpSpCategory->EnumTokens(NULL, NULL, &cpSpEnumTokens)))
			{
				ULONG vCount;
				cpSpEnumTokens->GetCount(&vCount);
				CComPtr<ISpObjectToken> pSpTok;
				for (int i = 0; i < vCount; ++i)
				{
					cpSpEnumTokens->Next(1, &pSpTok, nullptr);
					WCHAR* description;
					if (SUCCEEDED(hr = SpGetDescription(pSpTok, &description)))
					{
						voices.append(wstring(description) + L"\n");
					}
					pSpTok.Release();
				}
			}
		}
		return convertWstring(voices);
	}

	UINT32 getLength()
	{
		return length;
	}

	UINT32 getPosition()
	{
		return position;
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