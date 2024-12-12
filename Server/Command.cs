using Core;

namespace Server;

public struct Command(CommandName name, int clientId, byte[] body)
{
    public readonly CommandName Name = name;
    public readonly int ClientId = clientId;
    public readonly byte[] Body = body;

    private static readonly byte[] EmptyBody = [];
    
    public static Command Parse(byte[] command)
    {
        switch (command.Length)
        {
            case 4:
                return new Command(
                    (CommandName)BitConverter.ToInt32(command, 0),
                    0,
                    EmptyBody
                );
            case 8:
                return new Command(
                    (CommandName)BitConverter.ToInt32(command, 0),
                    BitConverter.ToInt32(command, 4),
                    EmptyBody
                );
            case > 8:
                var body = new byte[command.Length - 8];
                Array.Copy(command, 8, body, 0, body.Length);
                return new Command(
                    (CommandName)BitConverter.ToInt32(command, 0),
                    BitConverter.ToInt32(command, 4),
                    body
                );
            default:
                throw new ArgumentException("Invalid command length: " + command.Length);
        }
    }
}