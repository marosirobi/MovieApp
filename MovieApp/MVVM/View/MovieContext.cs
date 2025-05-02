
namespace MovieApp.MVVM.View
{
    internal class MovieContext : IDisposable
    {
        public object Reviews { get; internal set; }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}