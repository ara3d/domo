#  DoMo 

> <i>The key to controlling complexity is a good domain model, a model that goes beyond a surface vision of a domain by introducing an underlying structure, which gives the software developers the leverage they need. A good domain model can be incredibly valuable, but it’s not something that’s easy to make. Few people can do it well, and it’s very hard to teach.</i> <p> - Martin Fowler, 2003 in the preface to Domain-Driven Design: Tackling Complexity in the Heart of Software by Eric Evans

 DoMo (short for "Domain Modeling") is a .NET Standard 2.0 library that aids in building layered architecture applications inspired by good software engineering principles and best practices. 

 DoMo offers a solution for defining domain models as simple immutable objects that are unaware of the infrastructure and application logic, while providing support for data binding, and offering advantages of a centralized data management system (like Redux).

 DoMo is an unopinionated and cross-platform library with no dependencies and works well with other application frameworks and libraries (e.g., Entity Framework, Prism, MVVM Light, WPF Toolkit, etc.).

# How  DoMo Works 

 DoMo allow you to define domain models as regular C# classes, structs, or records. There is no requirement to decorate the model type with special attributes, inherit from base classes, or implement a particular interface. 

 DoMo wraps models types in an interface called `IDomainModel<T>`. This interface 

1. provides a GUID to facilitate maintaining references when persisting the model
1. implements `INotifyPropertyChanged` to enable data binding to Views or ViewModels
1. references the repository which created and owns the model
1. removes all event handlers in `IDisposable.Dispose()` to avoid memory leaks

```
public interface IDomainModel<T> 
    : INotifyPropertyChanged, IDisposable
{
    Guid Id { get; }
    T Model { get; set; }
    Type ModelType { get; }
    IRepository<T> Repository { get; }
}
```

It is strongly recommended to make the model implementation immutable. The property changed event handler is triggered when the model is changed. 

`IDomainModel` instances are created, retrieved, updated, validated, and deleted by a repository class. This repository class ultimately is responsible for storing the model data is stored. 

```
public interface IRepository<T>
    : IDisposable
{
    Guid RepositoryId { get; }
    Version Version { get; }
    Type ModelType { get; }
    IDomainModel<T> Create(T model);
    bool Delete(Guid modelId);
    T Read(Guid modelId);
    bool Update(Guid modelId, Func<T,T> updateFunc);
    bool Validate(Guid modelId, T state);
    IReadOnlyList<IDomainModel<T>> GetDomainModels();
}
```

There are two types of repositories, aggregate repositories which contain a collection of data models, and singleton repositories which manage precisely one data model.

```
    public interface IAggregateRepository<T> 
        : IRepository<T>, INotifyCollectionChanged
    { }

    public interface ISingletonRepository<T> 
        : IRepository<T>
    { }
```

All of the active repositories in an application are managed through an `IDataStore`

```
public interface IDataStore
    : IDisposable
{
    void AddRepository(IRepository repo);
    IReadOnlyList<IRepository> GetRepositories();
    void DeleteRepository(IRepository repository);
    IRepository GetRepository(Type type);   
    event EventHandler<RepositoryChangedArgs> RepositoryChanged;
}
```

# Advantages of Decoupled and Centralized Domain 

 DoMo manages the model state in a single location (a data store) that is decoupled from the rest of the application. This provides many advantages: 

* Views can bind directly to domain models
* Changing the persistence mechanism is trivial
* The same set of domain models can be reused in different applications 
* Taking snap shots of the application state (e.g. time travel) is trivial
* Improves testability 

# Suggested Best Practices 

The following are some suggested practices for usin  DoMo: 

* Start new applications by creating the domain models first
* Keep the domain models in a separate project
* Make model types immutable
* Use records as model types and leverage `with` expressions
* Create a separate test project just for your domain models
* Layer your domain models, so that higher-level models observe and react to changes in lower level models
* Use DoMo for application and infrastructure domain models as well. 
* Design and test your application so that arbitrary changes in the domain model are reflected throughout the API and the application, without putting it into an invalid state. 
* Don't be afraid to bind UI directly to domain models if/when appropriate

# Software Architectural Principles

1. The presentation logic should be separated from the business logic
2. The domain model should be ignorant of the persistenace mechanism of data
3. Interface segregation principle - interfaces are small
4. Dependency inversion principle - 

# Domain Driven Design Principles

Eric Evans wrote the following insightful guidance in a white-paper called [Domain-Driven Design Reference](https://www.domainlanguage.com/wp-content/uploads/2016/05/DDD_Reference_2015-03.pdf) which are just plain old good ideas, regardless of whether one chooses to follow all of the recommendations of DDD or not. 

> Isolate the expression of the domain model and the business logic, and eliminate any dependency on infrastructure, user interface, or even application logic that is not business logic. 

> Partition a complex program into layers. Develop a design within each layer that is cohesive and that depends only on the layers below. Follow standard architectural patterns to provide loose coupling to the layers above. 

> The domain objects, free of the responsibility of displaying themselves, storing themselves, managing application tasks, and so forth, can be focused on expressing the domain model. This allows a model to evolve to be rich enough and clear enough to capture essential business knowledge and put it to work. 

# Domain Driven Architectural Layers 

Domain driven design principles encourage the separation of applications into four distinct layers:

1. Presentation
2. Application
3. Domain
4. Infrastructure 

# Further Reading

* https://www.pluralsight.com/blog/software-development/domain-driven-design-csharp
* https://www.domainlanguage.com/wp-content/uploads/2016/05/DDD_Reference_2015-03.pdf
* https://docs.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/ddd-oriented-microservice
* https://www.methodsandtools.com/archive/archive.php?id=97
* https://en.wikipedia.org/wiki/Multitier_architecture
* https://en.wikipedia.org/wiki/Domain-driven_design

* https://ayende.com/blog/3137/infrastructure-ignorance
* https://docs.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/net-core-microservice-domain-model
* https://docs.microsoft.com/en-us/archive/msdn-magazine/2013/august/data-points-coding-for-domain-driven-design-tips-for-data-focused-devs
* https://github.com/heynickc/awesome-ddd

https://jeffreypalermo.com/2008/07/the-onion-architecture-part-1/
https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html
https://docs.microsoft.com/en-us/previous-versions/msp-n-p/ff649690(v=pandp.10)

# Acknowledgements 

* The VIM development team
* Martin Fowler
* Bob Martin
* Eric 




