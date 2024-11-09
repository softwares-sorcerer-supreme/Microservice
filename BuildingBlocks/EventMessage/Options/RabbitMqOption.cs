namespace EventMessage.Options;

public class RabbitMqOption
{
    public const string OptionName = "RabbitMqSettings";

    public string HostName { get; set; }
    public ushort Port { get; set; }
    public string VirtualHost { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
}