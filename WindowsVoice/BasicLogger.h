#pragma once
#include <fstream>
#include <ios>

enum LogPriority
{
    TraceP, DebugP, InfoP, WarnP, ErrorP, FatalP, CriticalP
};

class BasicLogger
{
	static LogPriority verbosity_;
    static const char* filepath_;

public:
    static void SetVerbosity(const LogPriority new_priority)
	{
        verbosity_ = new_priority;
    }

    static void Log(const LogPriority priority, const char* message)
	{
        if (priority >= verbosity_) {
            std::ofstream FILE(filepath_, std::ios_base::app);
            switch (priority) {
	            case TraceP: FILE << "Trace:\t"; break;
	            case DebugP: FILE << "Debug:\t"; break;
	            case InfoP: FILE << "Info:\t"; break;
	            case WarnP: FILE << "Warn:\t"; break;
	            case ErrorP: FILE << "Error:\t"; break;
                case FatalP: FILE << "Fatal:\t"; break;
	            case CriticalP: FILE << "Critical:\t"; break;
            }
            FILE << message << "\n";
            FILE.close();
        }
    }
};

LogPriority BasicLogger::verbosity_ = TraceP;
const char* BasicLogger::filepath_ = "WV-Log.txt";