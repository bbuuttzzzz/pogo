using System;

namespace Pogo.Building
{
    public class CommandLineArgsParser
    {
        string[] args;

        public CommandLineArgsParser()
        {
            args = Environment.GetCommandLineArgs();
        }

        public bool TryGetArg(string key, out string value)
        {
            int index = Array.IndexOf(args, key);
            if (index == -1)
            {
                value = default;
                return false;
            }
            if (index + 1 >= args.Length)
            {
                throw new ArgumentException($"Malformed argument {key}. expected format \"{key} <value>\"");
            }

            value = args[index + 1];
            return true;
        }
    }
}