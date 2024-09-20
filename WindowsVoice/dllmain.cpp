#include "pch.h"
#include "WindowsVoice.h"

namespace WindowsVoice
{
	void speechThreadFunc(const int rate, const int volume)
	{
		if (FAILED(::CoInitializeEx(NULL, COINITBASE_MULTITHREADED)))
		{
			theStatusMessage = L"Error: Failed to initialize COM for Voice.";
			speechState = speech_state_enum::error;
			return;
		}

		ISpVoice* pVoice = nullptr;

		const HRESULT hr = CoCreateInstance(CLSID_SpVoice, nullptr, CLSCTX_ALL, IID_ISpVoice, reinterpret_cast<void**>(&pVoice));
		if (!SUCCEEDED(hr))
		{
			const LPSTR pText = 0;

			::FormatMessage(FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_FROM_SYSTEM | FORMAT_MESSAGE_IGNORE_INSERTS,nullptr, hr, MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT), pText, 0, nullptr);
			LocalFree(pText);
			theStatusMessage = L"Error: Failed to create Voice instance.";
			speechState = speech_state_enum::error;
			return;
		}

		theStatusMessage = L"Speech ready.";
		speechState = speech_state_enum::ready;

		pVoice->SetRate(rate);
		pVoice->SetVolume(volume);

		SPVOICESTATUS voiceStatus;
		const wchar_t* priorText = nullptr;
		while (!shouldTerminate)
		{
			pVoice->GetStatus(&voiceStatus, 0);
			if (voiceStatus.dwRunningState == SPRS_IS_SPEAKING)
			{
				if (priorText == nullptr)
				{
					theStatusMessage = L"Error: SPRS_IS_SPEAKING but text is NULL";
					speechState = speech_state_enum::error;
				}
				else
				{
					theStatusMessage = L"Speaking: ";
					theStatusMessage.append(priorText);
					speechState = speech_state_enum::speaking;
					wordLength = voiceStatus.ulInputWordLen;
					wordPosition = voiceStatus.ulInputWordPos;
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
				speechState = speech_state_enum::ready;
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
		speechState = speech_state_enum::terminated;
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
			theStatusMessage = L"Warning: Speach thread already destroyed or not started.";
			return;
		}
		theStatusMessage = L"Destroying speech.";
		wordLength = 0;
		wordPosition = 0;
		shouldTerminate = true;
		theSpeechThread->join();
		theSpeechQueue.clear();
		delete theSpeechThread;
		theSpeechThread = nullptr;
		CoUninitialize();
		theStatusMessage = L"Speech destroyed.";
		speechState = speech_state_enum::uninitialized;
	}

	BSTR getStatusMessage()
	{
		if (theStatusMessage.empty())
		{
			theStatusMessage = L"WindowsVoice not yet initialized!";
		}
		return SysAllocString(theStatusMessage.c_str());
	}

	UINT32 getSpeechState()
	{
		return static_cast<UINT32>(speechState);
	}

	BSTR getVoicesAvailable()
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
					// try to get the Name attribute first; if failed, get the description
					CSpDynamicString voiceName;
					CComPtr<ISpDataKey> pAttribs;
					hr = pSpTok->OpenKey(SPTOKENKEY_ATTRIBUTES, &pAttribs);
					if (SUCCEEDED(hr))
					{
						hr = pAttribs->GetStringValue(L"Name", &voiceName);
					}
					if (FAILED(hr))
					{
						hr = SpGetDescription(pSpTok, &voiceName);
					}
					LANGID langid;
					if (SUCCEEDED(hr))
					{
						hr = SpGetLanguageFromVoiceToken(pSpTok, &langid);
					}
					if (SUCCEEDED(hr))
					{
						// get the locale name (e.g. "English (United States)")
						int size = GetLocaleInfoW(langid, LOCALE_SLOCALIZEDDISPLAYNAME, NULL, 0);
						if (size != 0)
						{
							wchar_t* localeName = new wchar_t[size];
							GetLocaleInfoW(langid, LOCALE_SLOCALIZEDDISPLAYNAME, localeName, size);
							voices += voiceName;
							voices += L'#';
							voices += localeName;
							voices += L'\n';
							delete[] localeName;
						}
					}
					pSpTok.Release();
				}
			}
		}
		return SysAllocString(voices.c_str());
	}

	UINT32 getWordLength()
	{
		return wordLength;
	}

	UINT32 getWordPosition()
	{
		return wordPosition;
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