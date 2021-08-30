namespace SaburaIIS.Agent.Transformers
{
    public interface ITransformer
    {
        void Transform(object obj, IDelta delta);
    }
}
