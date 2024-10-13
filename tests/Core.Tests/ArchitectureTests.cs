using System.Reflection;
using FluentAssertions;
using NetArchTest.Rules;
using Xunit.Abstractions;

namespace Core.Tests;

public class ArchitectureTests()
{
    private static readonly Assembly ApiAssembly = Assembly.GetAssembly(typeof(Api.Program));
    private static readonly Assembly InfrastructureAssembly = Assembly.GetAssembly(typeof(Infrastructure.DependencyInjection));
    private static readonly Assembly ApplicationAssembly = Assembly.GetAssembly(typeof(Application.DependencyInjection));
    private static readonly Assembly DomainAssembly = Assembly.GetAssembly(typeof(Domain.Forecast));

    [Fact]
    public void Api_Should_Not_Depend_On_Infrastructure_Except_For_Setting_Up_DependencyInjection()
    {
        var result = Types.InAssembly(ApiAssembly)
            .That()
            .DoNotHaveName("Program")
            .ShouldNot()
            .HaveDependencyOn("Infrastructure")
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }
    
    [Fact]
    public void Infrastructure_Should_Not_Depend_On_Api()
    {
        var result = Types.InAssembly(InfrastructureAssembly)
            .ShouldNot()
            .HaveDependencyOn("Api")
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }
    
    [Fact]
    public void Application_Should_Not_Depend_On_Api_Or_Infrastructure()
    {
        var result = Types.InAssembly(ApplicationAssembly)
            .ShouldNot()
            .HaveDependencyOnAny("Api", "Infrastructure")
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }
    
    [Fact]
    public void Domain_Should_Not_Have_Dependency_On_Any_Of_The_Other_Layers()
    {
        var result = Types.InAssembly(DomainAssembly)
            .ShouldNot()
            .HaveDependencyOnAny("Api", "Infrastructure", "Application")
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }
}