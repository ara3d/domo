using System.Drawing;

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

    public record Select(Guid ShapeId);


    #endregion

    #region Business Domain

    public interface IShape {}

    public interface IDrawingCommand {}

    public interface IInteraction {}

    public readonly record struct Line(Point Start, Point End) : IDrawingCommand;

    public readonly record struct Ellipse(Point Center, Size Size) : IShape;

    public readonly record struct Rectangle(Point Position, Size Size) : IShape;

    public readonly record struct SetPen(Color Color, float Width) : IDrawingCommand;

    public readonly record struct SetBrush(Color Color) : IDrawingCommand;

    public readonly record struct WriteText(Point Position, string Text) : IDrawingCommand;

    public readonly record struct Draw(IShape Shape) : IDrawingCommand;

    public readonly record struct Clear() : IDrawingCommand;

    #endregion

    public static class ModelRegistration
    {

        public static IRepositoryManager RegisterRepos(IRepositoryManager store)
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

            //store.AddAggregateRepository<IShape>();
            //store.AddAggregateRepository<IDrawingCommand>();
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