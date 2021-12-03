using System.Drawing;
using Domo;

namespace Domo.SampleModels
{
    #region Application and Infrastructure Domain

    public readonly record struct LogItem(string Category, string Message, string Data, DateTimeOffset Time);

    public readonly record struct ApplicationEvent(string EventName, DateTimeOffset Time);

    public readonly record struct Error(string Category, string Message);

    public readonly record struct User(string Name, DateTimeOffset LogInTime);

    public readonly record struct Folders(string ApplicationFolder, string LogFolder, string TempFolder, string DocumentFiles);

    public readonly record struct Files(string ExecutableFile, string LogFile, string SharedStateFile, string CollaboratorsFile, string PreferencesFiles);

    public readonly record struct UserPreferences(Folders Folders, Files Files, IReadOnlyList<RecentFile> RecentFiles, IReadOnlyList<Macro> Macros);

    public readonly record struct CommandLineArg(string Name, string Value);

    public readonly record struct EnvironmentVariable(string Name, string Value);

    public readonly record struct Command(string Name, string Data);

    public readonly record struct Macro(string Name, IReadOnlyList<Command> Commands);

    public readonly record struct RecentFile(string Path, DateTimeOffset DateOpened);

    public enum ChangeType { Added, Removed, Changed }

    public readonly record struct ChangeRecord(string Data, Guid ModelId, ChangeType ChangeType, DateTimeOffset DateChanged);

    public readonly record struct KeyBindings(string Key1, string Key2, string Command);

    public readonly record struct Job(string Name, bool Completed, bool Canceled, float Progress, bool Determinate);

    public readonly record struct LastInput(DateTimeOffset Time);

    public enum StatusCode { Good, Warning, Critical }

    public readonly record struct Status(string Message, StatusCode Code);

    public readonly record struct CurrentFile(string FilePath);

    public readonly record struct UndoItem(Guid RepoId, Guid ModelId, string OldValue, string NewValue);

    public readonly record struct UndoState(int CurrentIndex, IReadOnlyList<UndoItem> UndoItems);

    public readonly record struct Freeze();

    #endregion

    #region Presentation Domain

    public readonly record struct ActiveRepo(Guid RepoId);

    public enum ViewTypeEnum
    {
        Canvas,
        Text,
        List
    }

    public readonly record struct ViewSettings(ViewTypeEnum ViewType);

    public readonly record struct ClickAnimation(Point Position, DateTimeOffset Time);

    #endregion

    #region Business Domain

    public record DrawingShape;

    public record Line(Point Start, Point End) : DrawingShape;

    public record Ellipse(Point Center, Size Size) : DrawingShape;

    public record Rectangle(Point Position, Size Size) : DrawingShape;

    public record DrawingCommand;

    public record SetPen(Color Color, float Width) : DrawingCommand;

    public record SetBrush(Color Color) : DrawingCommand;

    public record WriteText(Point Position, string Text) : DrawingCommand;

    public record Select(IModel<DrawingShape> Shape) : DrawingCommand;

    public record Draw(DrawingShape Shape) : DrawingCommand;

    public record Clear() : DrawingCommand;

    public record CanvasClick(Point Position) : DrawingCommand;

    #endregion

    public static class ModelRegistration
    {

        public static IDataStore RegisterRepos(IDataStore store)
        {
            store.AddAggregateRepository<LogItem>();
            store.AddAggregateRepository<Error>();
            store.AddSingletonRepository<User>();
            store.AddSingletonRepository<UserPreferences>();
            store.AddAggregateRepository<CommandLineArg>();
            store.AddAggregateRepository<EnvironmentVariable>();
            store.AddAggregateRepository<RecentFile>();
            store.AddAggregateRepository<ChangeRecord>();
            store.AddAggregateRepository<Command>();
            store.AddAggregateRepository<Macro>();
            store.AddAggregateRepository<Job>();
            
            store.AddSingletonRepository<ActiveRepo>();
            store.AddSingletonRepository<ViewSettings>();
            store.AddAggregateRepository<ClickAnimation>();

            store.AddAggregateRepository<DrawingShape>();
            store.AddAggregateRepository<DrawingCommand>();
            store.AddSingletonRepository<CurrentFile>();
            store.AddSingletonRepository<LastInput>();
            store.AddAggregateRepository<UndoItem>();
            store.AddSingletonRepository<UndoState>();
            return store;

            /*   
    
    
            public readonly record struct BackupSettings(string BackupFilePath, TimeSpan Frequency);
    
            public readonly record struct Command(string Name, string Data);
    
            public readonly record struct KeyBindings(string Key1, string Key2, string Command);
    
            public readonly record struct Job(string Name, bool Completed, bool Canceled, float Progress, bool Determinate);
    
            public readonly record struct LastInput(DateTimeOffset Time);
    
            public enum StatusCode { Good, Warning, Critical }
    
            public readonly record struct Status(string Message, StatusCode Code);
    
            public readonly record struct CuurentFile(string FilePath);
    
            public readonly record struct UndoItem(Guid RepoId, Guid ModelId, string OldValue, string NewValue);
    
            public readonly record struct Freeze();
            */
        }
    }
}