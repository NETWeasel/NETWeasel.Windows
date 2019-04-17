using System;
using System.Collections.Generic;

namespace NETWeasel.Windows
{
    internal static class ArgumentParser
    {
        internal static List<Argument> Parse(string[] args)
        {
            var arguments = new List<Argument>();

            for (var i = 0; i < args.Length; i++)
            {
                var command = args[i];

                if (string.IsNullOrWhiteSpace(command) || !command.StartsWith("-"))
                {
                    // Discard this garbage, it's not
                    // a command
                    continue;
                }

                var parameter = args[i + 1];

                if (string.IsNullOrWhiteSpace(parameter) || parameter.StartsWith("-"))
                {
                    // The user has passed in a secondary command after the initial
                    // we'll discard the previous command and get this command on the
                    // next go around
                    continue;
                }

                var sanitizedCommand = SanitizeCommand(command);
                var sanitizedParameter = SanitizeParameter(parameter);

                // Increment index twice, because we've found a valid parameter
                i++;

                // If the command is not accepted by NETWeasel, we'll just discard
                // the command AND its parameter
                if (!Enum.TryParse(typeof(ArgumentCommand), sanitizedCommand, true, out var parsedCommand))
                    continue;

                arguments.Add(new Argument((ArgumentCommand)parsedCommand, sanitizedParameter));
            }

            return arguments;
        }

        private static string SanitizeCommand(string input)
        {
            if (!input.StartsWith("-"))
                return input;

            return input.Remove(0, 1);
        }

        private static string SanitizeParameter(string input)
        {
            if (!input.Contains("\""))
                return input;

            return input.Replace("\"", string.Empty);
        }

        internal struct Argument
        {
            public Argument(ArgumentCommand command, string parameter)
            {
                Command = command;
                Parameter = parameter;
            }

            public readonly ArgumentCommand Command;
            public readonly string Parameter;
        }
    }
}