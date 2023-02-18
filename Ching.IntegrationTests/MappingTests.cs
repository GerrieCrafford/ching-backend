using AutoMapper;
using Xunit;

public class MappingConfigurationsValid
{
    [Fact]
    public void CreateValidMappingConfiguration()
    {
        var mapper = new MapperConfiguration(cfg => cfg.AddMaps(new[] { typeof(Program) }));
        mapper.AssertConfigurationIsValid();
    }
}
