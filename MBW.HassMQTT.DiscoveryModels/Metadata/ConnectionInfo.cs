using FluentValidation;

namespace MBW.HassMQTT.DiscoveryModels.Metadata;

public class ConnectionInfo
{
    public static ConnectionInfoValidator Validator { get; } = new ConnectionInfoValidator();

    /// <summary>The connection type, such as <c>mac</c>.</summary>
    public string Type { get; set; }
    /// <summary>The connection identifier associated with <see cref="Type" />.</summary>
    public string Value { get; set; }
        
    public ConnectionInfo()
    {
    }

    public ConnectionInfo(string type, string value)
    {
        Type = type;
        Value = value;
    }

    public static implicit operator ConnectionInfo((string, string) val)
    {
        return new ConnectionInfo
        {
            Type = val.Item1,
            Value = val.Item2
        };
    }

    public class ConnectionInfoValidator : AbstractValidator<ConnectionInfo>
    {
        public ConnectionInfoValidator()
        {
            RuleFor(s => s.Type).NotEmpty().WithMessage("Connections.Type must not be empty or null");
            RuleFor(s => s.Value).NotEmpty().WithMessage("Connections.Value must not be empty or null");
        }
    }
}
