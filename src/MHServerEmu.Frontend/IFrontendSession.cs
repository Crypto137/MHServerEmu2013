﻿namespace MHServerEmu.Frontend
{
    /// <summary>
    /// Represents a <see cref="FrontendClient"/> session.
    /// </summary>
    public interface IFrontendSession
    {
        public ulong Id { get; }
        public object Account { get; }
    }
}
