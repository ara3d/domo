using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.ExceptionServices;

namespace Domo.Utils.Wpf
{
    public class SystemInfo
    {
        public string CommandLine { get; init; } = Environment.CommandLine;
        public string CurrentWorkingDirectory { get; init; } = Environment.CurrentDirectory;
        public bool Is64BitOperatingSystem { get; init; } = Environment.Is64BitOperatingSystem;
        public bool Is64BitProcess { get; init; } = Environment.Is64BitProcess;
        public string MachineName { get; init; } = Environment.MachineName;
        public OperatingSystem OSVersion { get; init; } = Environment.OSVersion;
        public string ProcessPath { get; init; } = Environment.ProcessPath ?? "";
        public int ProcessId { get; init; } = Environment.ProcessId;
        public int ProcessorCount { get; init; } = Environment.ProcessorCount;
        public string SystemDirectory { get; init; } = Environment.SystemDirectory;
        public long MSecSinceSystemStarted { get; init; } = Environment.TickCount64;
        public bool UserInteractiveMode { get; init; } = Environment.UserInteractive;
        public Version CommonLanguageRuntime { get; init; } = Environment.Version;
        public long MappedPhysicalRAM { get; init; } = Environment.WorkingSet;

        public string AppDomainBaseDirectory { get; init; } = AppDomain.CurrentDomain.BaseDirectory;
        public string AppDomainDynamicDirectory { get; init; } = AppDomain.CurrentDomain.DynamicDirectory ?? "";
        public long AppDominSurvivedMemorySize { get; init; } = AppDomain.CurrentDomain.MonitoringSurvivedMemorySize;
        public TimeSpan AppDoainTotalProcessTime { get; init; } = AppDomain.CurrentDomain.MonitoringTotalProcessorTime;
        public string AppDomainFriendlyName { get; init; } = AppDomain.CurrentDomain.FriendlyName;

        public bool IsDebuggerAttached { get; init; } = Debugger.IsAttached;

        public ProcessData CurrentProcess { get; init; } = new(Process.GetCurrentProcess());

        public AssemblyData EntryAssembly { get; init; } = Assembly.GetEntryAssembly().ToAssemblyData();
        public AssemblyData ExecutingAssembly { get; init; } = new(Assembly.GetEntryAssembly());
    }

    public class UserInfo
    {
        public string UserDomainName { get; init; } = Environment.UserDomainName;
        public string UserName { get; init; } = Environment.UserName;
    }

    public record ErrorData
    {
        public string Message { get; init; }
        public string HelpLink { get; init; }
        public string Type { get; init; }
        public bool Caught { get; init; }
        public int HResult { get; init; }
        public string StackTrace { get; init; }

        public ErrorData(Exception e, bool caught = true)
        {
            Message = e.Message;
            HelpLink = e.HelpLink ?? "";
            Type = e.GetType().Name;
            Caught = caught;
            HResult = e.HResult;
            StackTrace = e.StackTrace ?? "";
        }
    }

    public static class GitHelper
    {
        // https://stackoverflow.com/questions/48421697/get-name-of-branch-into-code

        public static string CommitHash()
            => GitRunner("rev-parse --short HEAD");

        public static string CommitHashLong()
            => GitRunner("rev-parse HEAD");

        public static string BranchName()
            => GitRunner("rev-parse --abbrev-ref HEAD");

        public static string Remote()
            => GitRunner("config --get remote.origin.url");

        public static string GitRunner(string args)
            => ReadOneLineAndQuit("git", args);

        public static string ReadOneLineAndQuit(string processName, string args)
            => new ProcessStartInfo(processName)
                {
                    UseShellExecute = false,
                    Arguments = args
                }
                .ReadOneLineAndQuit();

        public static string GitLocation()
            => ReadOneLineAndQuit("where", "git.exe");

        public static string ReadOneLineAndQuit(this ProcessStartInfo psi)
        {
            psi.RedirectStandardOutput = true;
            using var p = new Process { StartInfo = psi };
            p.Start();
            return p.StandardOutput.ReadLine() ?? "";
        }
    }

    public record GitStatus
    {
        public string Hash { get; init; } = GitHelper.CommitHash();
        public string Branch { get; init; } = GitHelper.BranchName();
        public string Remote { get; init; } = GitHelper.Remote();
    }

    public record LogEntry
    {
        public LogEntry(EventLogEntry e)
        {
            Index = e.Index;
            Message = e.Message;
            EntryType = (int)e.EntryType;
            CategoryName = e.Category;
        }

        public int Index { get; init; }
        public string Message { get; init; } = "";
        public int EntryType { get; init; }
        public string CategoryName { get; init; } = "";
    }

    public record Log
    {
        public LogEntry[] Logs { get; init; } = Array.Empty<LogEntry>();
    }

    public static class Folders
    {
        public static string AdminTools => Environment.GetFolderPath(Environment.SpecialFolder.AdminTools);
        public static string ApplicationData => Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        public static string CDBurning => Environment.GetFolderPath(Environment.SpecialFolder.CDBurning);

        public static string Windows => Environment.GetFolderPath(Environment.SpecialFolder.Windows);
        public static string UserProfile => Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        public static string Templates => Environment.GetFolderPath(Environment.SpecialFolder.Templates);
        public static string SystemX86 => Environment.GetFolderPath(Environment.SpecialFolder.SystemX86);

        public static string System => Environment.GetFolderPath(Environment.SpecialFolder.System);
        public static string StartMenu => Environment.GetFolderPath(Environment.SpecialFolder.StartMenu);
        public static string SendTo => Environment.GetFolderPath(Environment.SpecialFolder.SendTo);
        public static string Resources => Environment.GetFolderPath(Environment.SpecialFolder.Resources);
        public static string Recent => Environment.GetFolderPath(Environment.SpecialFolder.Recent);
        public static string Programs => Environment.GetFolderPath(Environment.SpecialFolder.Programs);
        public static string ProgramFilesX86 => Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
        public static string ProgramFiles => Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
        public static string PrinterShortcuts => Environment.GetFolderPath(Environment.SpecialFolder.PrinterShortcuts);
        public static string Personal => Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        public static string NetworkShortcuts => Environment.GetFolderPath(Environment.SpecialFolder.NetworkShortcuts);
        public static string MyVideos => Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);

        public static string MyPictures => Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
        public static string MyDocuments => Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        
        public static string MyComputer => "";

        public static string LocalizedResources =>
            Environment.GetFolderPath(Environment.SpecialFolder.LocalizedResources);

        public static string LocalApplicationData =>
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

        public static string InternetCache => Environment.GetFolderPath(Environment.SpecialFolder.InternetCache);
        public static string History => Environment.GetFolderPath(Environment.SpecialFolder.History);
        public static string Fonts => Environment.GetFolderPath(Environment.SpecialFolder.Fonts);
        public static string Favorites => Environment.GetFolderPath(Environment.SpecialFolder.Favorites);
        public static string DesktopDirectory => Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
        public static string Desktop => Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        public static string Cookies => Environment.GetFolderPath(Environment.SpecialFolder.Cookies);
        public static string CommonVideos => Environment.GetFolderPath(Environment.SpecialFolder.CommonVideos);
        public static string CommonTemplates => Environment.GetFolderPath(Environment.SpecialFolder.CommonTemplates);
        public static string CommonStartup => Environment.GetFolderPath(Environment.SpecialFolder.CommonStartup);
        public static string CommonStartMenu => Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu);
        public static string CommonPrograms => Environment.GetFolderPath(Environment.SpecialFolder.CommonPrograms);

        public static string CommonProgramFilesX86 =>
            Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFilesX86);

        public static string CommonProgramFiles =>
            Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles);

        public static string CommonPictures => Environment.GetFolderPath(Environment.SpecialFolder.CommonPictures);
        public static string CommonOemLinks => Environment.GetFolderPath(Environment.SpecialFolder.CommonOemLinks);
        public static string CommonMusic => Environment.GetFolderPath(Environment.SpecialFolder.CommonMusic);
        public static string CommonDocuments => Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments);

        public static string CommonDesktopDirectory =>
            Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory);

        public static string CommonApplicationData =>
            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);

        public static string CommonAdminTools => Environment.GetFolderPath(Environment.SpecialFolder.CommonAdminTools);
    }

    public static class EnvironmentVariableNames
    {
        public const string AllUsersProfile = "ALLUSERSPROFILE";
        public const string AppData = "APPDATA";
        public const string CommonProgramFiles = "COMMONPROGRAMFILES";
        public const string CommonProgramFilesX86 = "COMMONPROGRAMFILES(x86)";
        public const string CommonProgramW6432 = "CommonProgramW6432";
        public const string ComputerName = "ComputerName";
        public const string ComSpec = "COMSPEC";
        public const string HomeDrive = "HOMEDRIVE";
        public const string HomePath = "HOMEPATH";
        public const string LocalAppData = "LOCALAPPDATA";
        public const string LogonServer = "LOGONSERVER";
        public const string Path = "PATH";
        public const string PathExt = "PATHEXT";
        public const string ProgramData = "ProgramData";
        public const string ProgramW6432 = "ProgramW6432";
        public const string ProgramFiles = "PROGRAMFILES";
        public const string ProgramFilesX86 = "PROGRAMFILES(X86)";
        public const string Prompt = "PROMPT";
        public const string PSModulePath = "PSModulePath";
        public const string Public = "Public";
        public const string SystemDrive = "SystemDrive";
        public const string SystemRoot = "SystemRoot";
        public const string Temp = "TEMP";
        public const string Tmp = "TMP";
        public const string UserName = "USERNAME";
        public const string UserProfile = "USERPROFILE";
        public const string UserDomain = "USERDOMAIN";
        public const string WinDir = "windir";
    }

    public record AssemblyData
    {
        public AssemblyData(Assembly asm)
        {
            EntryPoint = asm.EntryPoint?.Name ?? "";
            FullName = asm.FullName ?? "";
            ImageRuntimeVersion = asm.ImageRuntimeVersion;
            IsFullyTrusted = asm.IsFullyTrusted;
            Location = asm.Location;
            Architecture = asm.GetName().ProcessorArchitecture.ToString();
            Version = asm.GetName().Version ?? new Version();
            ShortName = asm.GetName().Name ?? "";
            CodeBase = asm.GetName().CodeBase ?? "";
        }

        public string EntryPoint { get; init; }
        public string FullName { get; init; }
        public string ImageRuntimeVersion { get; init; }
        public bool IsFullyTrusted { get; init; }
        public string Location { get; init; }
        public string Architecture { get; init; }
        public Version Version { get; init; }
        public string ShortName { get; init; }
        public string CodeBase { get; init; }
    }

    public record ProcessData
    {
        public ProcessData(Process p)
        {
            ExitCode = p.ExitCode;
            HasExited = p.HasExited;
            Responding = p.Responding;
            ExitTime = p.ExitTime;
            MachineName = p.MachineName;
            WindowTitle = p.MainWindowTitle;
            FileName = p.MainModule?.FileName ?? "";
            Id = p.Id;
            PagedMemorySize = p.PagedMemorySize64;
            NonPagedMemorySize = p.NonpagedSystemMemorySize64;
            VirtualMemorySize = p.VirtualMemorySize64;
            PeakVirtualMemorySize = p.PeakVirtualMemorySize64;
            PeakPagedMemorySize = p.PeakPagedMemorySize64;
            WorkingSet = p.WorkingSet64;
            PeakWorkingSet = p.PeakWorkingSet64;
            PrivateMemorySize = p.PrivateMemorySize64;
            FileVersionInfo = p.MainModule?.FileVersionInfo;
            ModuleName = p.MainModule?.ModuleName ?? "";
        }

        public int ExitCode { get; init; }
        public bool HasExited { get; init; }
        public bool Responding { get; init; }
        public DateTimeOffset ExitTime { get; init; } 
        public string MachineName { get; init; }
        public string FileName { get; init; }
        public string WindowTitle { get; init; }
        public string ModuleName { get; init; }
        public int Id { get; init; }
        public long WorkingSet { get; init; }
        public long PeakWorkingSet { get; init; }
        public long PagedMemorySize { get; init; }
        public long NonPagedMemorySize { get; init; }
        public long PeakPagedMemorySize { get; init; }
        public long VirtualMemorySize { get; init; }
        public long PeakVirtualMemorySize { get; init; }
        public long PrivateMemorySize { get; init; }
        public FileVersionInfo? FileVersionInfo { get; init; }
    }

    // https://stackoverflow.com/questions/5762526/how-can-i-make-something-that-catches-all-unhandled-exceptions-in-a-winforms-a

    public static class Helpers
    {
        public static void SetFirstChanceExceptionCallback(Action<object?, FirstChanceExceptionEventArgs> handler)
            => AppDomain.CurrentDomain.FirstChanceException += (sender, args) => handler(sender, args);

        public static void SetProcessExitCallback(Action<object?, EventArgs> handler)
        {
            var p = Process.GetCurrentProcess();
            p.Exited += (sender, args) => handler(sender, args);
            //AppDomain.CurrentDomain.ProcessExit += (sender, args) => handler(sender, args);
        }

        public static void SetUnhandledExceptionCallback(Action<object?, UnhandledExceptionEventArgs> handler)
            => AppDomain.CurrentDomain.UnhandledException += (sender, args) => handler(sender, args);

        public static ProcessData ToProcessData(this Process process)
            => new(process);

        public static AssemblyData ToAssemblyData(this Assembly assembly)
            => new(assembly);

        public static FileVersionInfo ToFileVersion(this string fileName)
            => FileVersionInfo.GetVersionInfo(fileName);

        public static void Test()
        {
            var p = Process.GetCurrentProcess();
            System.Diagnostics.EventLog.GetEventLogs();


            // 
            // TREE
            // VOL
            // XCOPY
            // TELNET
            // PING
            // FINDSTR

            // Notepad
            // Calculator

        }
    }
}
