using System.Reflection;
using Core.Tests.Rules;
using FluentAssertions;
using MediatR;
using NetArchTest.Rules;
using Xunit.Abstractions;

namespace Core.Tests;

public class ArchitectureTests(ITestOutputHelper testOutputHelper)
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
        
        LogIfFailure(result);

        result.IsSuccessful.Should().BeTrue();
    }
    
    [Fact]
    public void Infrastructure_Should_Not_Depend_On_Api()
    {
        var result = Types.InAssembly(InfrastructureAssembly)
            .ShouldNot()
            .HaveDependencyOn("Api")
            .GetResult();
        
        LogIfFailure(result);

        result.IsSuccessful.Should().BeTrue();
    }
    
    [Fact]
    public void Application_Should_Not_Depend_On_Api_Or_Infrastructure()
    {
        var result = Types.InAssembly(ApplicationAssembly)
            .ShouldNot()
            .HaveDependencyOnAny("Api", "Infrastructure")
            .GetResult();
        
        LogIfFailure(result);

        result.IsSuccessful.Should().BeTrue();
    }
    
    [Fact]
    public void Domain_Should_Not_Have_Dependency_On_Any_Of_The_Other_Layers()
    {
        var result = Types.InAssembly(DomainAssembly)
            .ShouldNot()
            .HaveDependencyOnAny("Api", "Infrastructure", "Application")
            .GetResult();
        
        LogIfFailure(result);

        result.IsSuccessful.Should().BeTrue();
    }
    
    [Fact]
    public void IRequests_Should_Return_Result_Or_ResultOfT()
    {
        var requestShouldWrapResultRule = new RequestShouldWrapResultRule();
        
        var result = Types
            .InAssembly(ApplicationAssembly)
            .That()
            .ImplementInterface(typeof(IRequest<>))
            .Should()
            .MeetCustomRule(requestShouldWrapResultRule)
            .GetResult();

        LogIfFailure(result);
        
        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void IRequests_Should_Not_Implement_Plain_Request()
    {
        var result = Types
            .InAssembly(ApplicationAssembly)
            .ShouldNot()
            .ImplementInterface(typeof(IRequest))
            .GetResult();

        LogIfFailure(result);
        result.IsSuccessful.Should().BeTrue();
    }

    private void LogIfFailure(TestResult result)
    {
        if (result.IsSuccessful) return;

        testOutputHelper.WriteLine("Types that failed to meet the rule: " + string.Join(", ", result.FailingTypes));
    }
}