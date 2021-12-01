namespace Domo
{
    /// <summary>
    /// Use a ModelReference to make a persistent link between two models. 
    /// </summary>
    public readonly struct ModelReference<T>
    {
        public ModelReference(IModel<T> model)
            => Model = model;

        private IModel<T> Model { get; }
        public T Value => Model.Value;
    }
}