namespace Infrastructure;

public sealed record InfrastructureConfig
{
    public required string ConnectionString { get; set; }
}