using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Input;
using Domo.SampleModels;

namespace Domo.Sample.Services
{
    public class CommandService 
    { }

    public static class ServiceExtensions
    {
    }

    public class ApplicationEventService : AggregateModelBackedService<ApplicationEvent>
    {
        public ApplicationEventService(IDataStore store, ILogService logService) : base(store)
        {
            Repository.OnModelChanged(status => logService.Log("Application Event", status.Value.EventName));
        }

        public void ApplicationStart()
            => Repository.Create(new("Application Started", DateTimeOffset.Now));

        public void ApplicationEnd()
            => Repository.Create(new("Application Closed", DateTimeOffset.Now));
    }

    public class FileService
    {

    }

    public class ErrorService
    {
    }

    public interface IStatusService : ISingletonModelBackedService<Status>
    {
    }

    public interface ILogService : IAggregateModelBackedService<LogItem>
    {
        void Log(string category, string message);
    }

    public class LogService : AggregateModelBackedService<LogItem>, ILogService
    {
        public LogService(IDataStore store)
            : base(store) 
        { }

        public void Log(string category, string message)
            => Repository.Create(new LogItem(category, message, "", DateTimeOffset.Now));
    }

    public class StatusService : SingletonModelBackedService<Status>, IStatusService
    {
        public StatusService(IDataStore dataStore, ILogService logService)
            : base(dataStore)
        {
            Repository.OnModelChanged(status => logService.Log("Status", status.Value.Message));
        }

        public string Status
        {
            get => Model.Value.Message;
            set => Model.Value = Model.Value with { Message = value };
        }
    }

    public interface IUserService
    {
        public 
    }

    public class UserService : SingletonModelBackedService<User>
    {
        public UserService(IDataStore dataStore)
            : base(dataStore) { }

        public void LogIn(string name)
        {
            if (LoggedIn)
                throw new Exception($"Already logged in as {Model.Value.Name}!");
            if (string.IsNullOrWhiteSpace(name))
                throw new Exception("name cannot be null or empty");
            Model.Value = Model.Value with { Name = name, LogInTime = DateTimeOffset.Now };
        }

        public bool CanLogIn
            => !LoggedIn;

        public bool CanLogOut
            => LoggedIn;

        public void LogOut()
            => Model.Value = Model.Value with { Name = "" };

        public bool LoggedIn
            => !string.IsNullOrWhiteSpace(Model.Value.Name);

        public TimeSpan TimeLoggedIn
            => LoggedIn ? DateTimeOffset.Now - Model.Value.LogInTime : TimeSpan.Zero;
    }

    public class CommandLineService
    {

    }

    public class MouseService
    {

    }

    public class DrawingService
    {
    }

    public interface INamedCommand : ICommand
    {
        public string Name { get; }
    }

    public interface IUndoService : ISingletonModelBackedService<UndoState>
    {
        bool CanUndo { get; }
        bool CanRedo { get; }
        void Undo();
        void Redo();

        INamedCommand UndoCommand { get; }
        INamedCommand RedoCommand { get; }
    }

    public class UndoService : SingletonModelBackedService<UndoState>, IUndoService
    {
        public UndoService(IDataStore store)
            : base(store)
        {
            Store.RepositoryChanged += Store_RepositoryChanged;
            RegisterCommand(Undo, () => CanUndo);
        }

        private void Store_RepositoryChanged(object? sender, RepositoryChangeArgs e)
        {
            // TODO: store the appropriate change. 
            if (CanRedo)
            {
                // Clear all the "undo items" in the Redo stack 
            }
        }

        public bool CanUndo
            => Value.CurrentIndex >= 0;

        public bool CanRedo
            => Value.CurrentIndex < Value.UndoItems.Count - 1;

        public void Redo()
        {
            throw new NotImplementedException();
        }

        public void Undo()
        {
            throw new NotImplementedException();
        }

        public INamedCommand UndoCommand => GetCommand(nameof(Undo));
        public INamedCommand RedoCommand => GetCommand(nameof(Redo));
    }

    public interface IChangeService
    {
    }

    public class ChangeService
    {
        public ChangeService()
    }

    public class KeyboardService
    { }
}