using MHServerEmu.Core.Config;

namespace MHServerEmu.Auth
{
    /// <summary>
    /// Contains configuration for the <see cref="AuthServer"/>.
    /// </summary>
    public class AuthConfig : ConfigContainer
    {
        public string AuthAddress { get; private set; } = "localhost";
        public string AuthPort { get; private set; } = "8080";
    }
}
