using System.Reflection;
using MHServerEmu.Core.Logging;
using MHServerEmu.Games.Network;

namespace MHServerEmu.Games.Commands
{
    /// <summary>
    /// A singleton that handles commands.
    /// </summary>
    public partial class CommandManagerMini : ICommandParser<PlayerConnection>
    {
        private static readonly Logger Logger = LogManager.CreateLogger();

        private readonly Dictionary<CommandAttribute, CommandInvoker> _commandDict = new();

        private delegate string CommandInvoker(string[] @params, PlayerConnection invoker);

        public static CommandManagerMini Instance { get; } = new();

        private CommandManagerMini()
        {
            foreach (MethodInfo methodInfo in typeof(CommandManagerMini).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic))
            {
                var commandAttribute = methodInfo.GetCustomAttribute<CommandAttribute>();
                if (commandAttribute == null)
                    continue;

                _commandDict.Add(commandAttribute, methodInfo.CreateDelegate<CommandInvoker>(this));
            }
        }

        public bool TryParse(string message, PlayerConnection invoker)
        {
            if (ParseInput(message, out string command, out string[] @params) == false)
                return false;

            foreach (var kvp in _commandDict)
            {
                if (kvp.Key.Name != command)
                    continue;

                string output = kvp.Value(@params, invoker);

                if (string.IsNullOrEmpty(output) == false)
                    invoker.SendSystemChatMessage(output);

                return true;
            }

            return false;
        }

        private bool ParseInput(string input, out string command, out string[] @params)
        {
            command = string.Empty;
            @params = Array.Empty<string>();

            input = input.Trim();
            if (input.Length < 2 || input[0] != '!')
                return false;

            string[] tokens = input.Split(' ');
            command = tokens[0].Substring(1).ToLower();
            
            if (tokens.Length > 1)
            {
                @params = new string[tokens.Length - 1];
                Array.Copy(tokens, 1, @params, 0, @params.Length);
            }

            return true;   
        }

        [AttributeUsage(AttributeTargets.Method)]
        private class CommandAttribute : Attribute
        {
            public string Name { get; }
            public string Description { get; }

            public CommandAttribute(string name, string description)
            {
                Name = name;
                Description = description;
            }
        }
    }
}
