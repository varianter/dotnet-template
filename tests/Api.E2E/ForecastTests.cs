using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Api.Authorization;
using Api.E2E.Shared;
using Api.Routes.Weather.Models;
using Bogus;
using Domain;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Api.E2E;

[Collection(ApiTestCollection.CollectionName)]
public class ForecastTests(ApiFactory apiFactory) : TestsBase(apiFactory)
{
    private static readonly Faker<Forecast> ForecastFaker = new Faker<Forecast>()
        .RuleFor(x => x.Id, f => f.Random.Guid())
        .RuleFor(x => x.Date, f => f.Date.FutureDateOnly())
        .RuleFor(x => x.TemperatureC, f => f.Random.Int(-20, 55))
        .RuleFor(x => x.Summary, f => f.PickRandom(null, f.Lorem.Sentence()))
        .RuleFor(x => x.IsDeleted, f => false);

    [Fact]
    public async Task GetForecast_IfAvailableInDb_ShouldReturnForecast()
    {
        // Arrange
        var forecast = ForecastFaker.Generate();
        DatabaseContext.Forecasts.Add(forecast);
        await DatabaseContext.SaveChangesAsync();
        DatabaseContext.ChangeTracker.Clear();

        var jwt = MockJwtTokensHelper.GenerateJwtToken(new MockJwtTokensHelper.TokenOptions
            { Scopes = [Scopes.Read] });

        var request = new HttpRequestMessage(HttpMethod.Get, $"/weather/{forecast.Date.ToString("yyyy-MM-dd")}")
        {
            Headers = { Authorization = new AuthenticationHeaderValue("Bearer", jwt) }
        };

        var response = await Client.SendAsync(request);

        response.IsSuccessStatusCode.Should().BeTrue();

        var content = await response.Content.ReadFromJsonAsync<GetWeatherResponse>();

        content.Should().NotBeNull();
        content.Should().BeEquivalentTo(forecast, options => options.ExcludingMissingMembers());
    }

    [Fact]
    public async Task GetForecast_WithoutScope_ShouldReturnForbidden()
    {
        var jwt = MockJwtTokensHelper.GenerateJwtToken(new MockJwtTokensHelper.TokenOptions
            { Scopes = [] });

        var request = new HttpRequestMessage(HttpMethod.Get, $"/weather/{DateTime.Now:yyyy-MM-dd}")
        {
            Headers = { Authorization = new AuthenticationHeaderValue("Bearer", jwt) }
        };

        var response = await Client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task AddForecast_WithValidData_ShouldReturnCreated()
    {
        var forecast = ForecastFaker.Generate();
        var jwt = MockJwtTokensHelper.GenerateJwtToken(new MockJwtTokensHelper.TokenOptions
            { Scopes = [Scopes.Read, Scopes.Write] });

        var requestBody = new PostWeatherRequest(new DateTimeOffset(forecast.Date.ToDateTime(TimeOnly.MinValue)),
            forecast.TemperatureC, forecast.Summary);

        var request = new HttpRequestMessage(HttpMethod.Post, "/weather")
        {
            Content = JsonContent.Create(requestBody),
            Headers = { Authorization = new AuthenticationHeaderValue("Bearer", jwt) }
        };

        var response = await Client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var dbForecast = await DatabaseContext.Forecasts.SingleAsync();

        dbForecast.Should().NotBeNull();
        dbForecast.Should().BeEquivalentTo(forecast, options => options.Excluding(f => f.Id));
    }

    [Fact]
    public async Task AddForecast_WithoutScope_ShouldReturnForbidden()
    {
        var forecast = ForecastFaker.Generate();
        var jwt = MockJwtTokensHelper.GenerateJwtToken(new MockJwtTokensHelper.TokenOptions
            { Scopes = [] });

        var requestBody = new PostWeatherRequest(new DateTimeOffset(forecast.Date.ToDateTime(TimeOnly.MinValue)),
            forecast.TemperatureC, forecast.Summary);

        var request = new HttpRequestMessage(HttpMethod.Post, "/weather")
        {
            Content = JsonContent.Create(requestBody),
            Headers = { Authorization = new AuthenticationHeaderValue("Bearer", jwt) }
        };

        var response = await Client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);

        var dbForecast = await DatabaseContext.Forecasts.SingleOrDefaultAsync();
        dbForecast.Should().BeNull();
    }

    [Fact]
    public async Task AddForecast_WithInvalidData_ShouldReturnBadRequest()
    {
        var forecast = ForecastFaker.Generate();
        var jwt = MockJwtTokensHelper.GenerateJwtToken(new MockJwtTokensHelper.TokenOptions
            { Scopes = [Scopes.Read, Scopes.Write] });

        var requestBody = new PostWeatherRequest(new DateTimeOffset(forecast.Date.ToDateTime(TimeOnly.MinValue)), -100,
            forecast.Summary);

        var request = new HttpRequestMessage(HttpMethod.Post, "/weather")
        {
            Content = JsonContent.Create(requestBody),
            Headers = { Authorization = new AuthenticationHeaderValue("Bearer", jwt) }
        };

        var response = await Client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        response.Content.Headers.Should().ContainKey("Content-Type").WhoseValue.Should()
            .Contain("application/problem+json");

        var dbForecast = await DatabaseContext.Forecasts.SingleOrDefaultAsync();
        dbForecast.Should().BeNull();
    }

    [Fact]
    public async Task DeleteForecast_AsAdmin_ShouldReturnNoContent()
    {
        // Arrange
        var forecast = ForecastFaker.Generate();
        DatabaseContext.Forecasts.Add(forecast);
        await DatabaseContext.SaveChangesAsync();
        DatabaseContext.ChangeTracker.Clear();

        var jwt = MockJwtTokensHelper.GenerateJwtToken(new MockJwtTokensHelper.TokenOptions
            { Scopes = [Scopes.Admin] });

        var request = new HttpRequestMessage(HttpMethod.Delete, $"/weather/{forecast.Date.ToString("yyyy-MM-dd")}")
        {
            Headers = { Authorization = new AuthenticationHeaderValue("Bearer", jwt) }
        };

        var response = await Client.SendAsync(request);

        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var dbForecast = await DatabaseContext.Forecasts.SingleAsync();
        dbForecast.Should().NotBeNull();
        dbForecast.IsDeleted.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteForecast_AsUser_ShouldReturnForbidden()
    {
        // Arrange
        var forecast = ForecastFaker.Generate();
        DatabaseContext.Forecasts.Add(forecast);
        await DatabaseContext.SaveChangesAsync();
        DatabaseContext.ChangeTracker.Clear();

        var jwt = MockJwtTokensHelper.GenerateJwtToken(new MockJwtTokensHelper.TokenOptions
            { Scopes = [Scopes.Read] });

        var request = new HttpRequestMessage(HttpMethod.Delete, $"/weather/{forecast.Date.ToString("yyyy-MM-dd")}")
        {
            Headers = { Authorization = new AuthenticationHeaderValue("Bearer", jwt) }
        };

        var response = await Client.SendAsync(request);

        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);

        var dbForecast = await DatabaseContext.Forecasts.SingleAsync();
        dbForecast.Should().NotBeNull();
        dbForecast.IsDeleted.Should().BeFalse();
    }
}