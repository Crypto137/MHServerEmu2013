namespace MHServerEmu.Core.Memory
{
    /// <summary>
    /// Provides access to type agnostic functionality of <see cref="ObjectPool{T}"/>.
    /// </summary>
    public interface IObjectPool
    {
        public int CountTotal { get; }
        public int CountInactive { get; }
        public int CountActive { get; }

        public void ReturnUnsafe(object instance);
    }
}
