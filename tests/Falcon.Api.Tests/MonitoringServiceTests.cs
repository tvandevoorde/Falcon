using AutoMapper;
using Falcon.Application.Contracts.Alerts;
using Falcon.Application.Contracts.Logs;
using Falcon.Application.Mappings;
using Falcon.Application.Services;
using Falcon.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;

namespace Falcon.Api.Tests;

public sealed class MonitoringServiceTests
{
    private static readonly Guid SeedServerId = Guid.Parse("f0b7a9a0-4f5b-4a64-8a68-9a6aaf8c8d01");
    private static readonly Guid SeedServiceId = Guid.Parse("56a9d873-18a0-4d04-8f3b-8e20538f01dc");

    private readonly MonitoringService monitoringService;

    public MonitoringServiceTests()
    {
        var mapperConfiguration = new MapperConfiguration(configuration =>
        {
            configuration.AddProfile<MonitoringMappingProfile>();
        });

        var mapper = mapperConfiguration.CreateMapper();
        var repository = new InMemoryMonitoringRepository();
        var logger = NullLogger<MonitoringService>.Instance;

        monitoringService = new MonitoringService(repository, mapper, logger);
    }

    [Fact]
    public async Task GetServersAsync_WithSeedData_ReturnsPagedResult()
    {
        var result = await monitoringService.GetServersAsync(null, null, 1, 10, CancellationToken.None);

        result.Total.Should().BeGreaterThan(0);
        result.Items.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetServerAsync_WithExistingServer_ReturnsDetailProjection()
    {
        var server = await monitoringService.GetServerAsync(SeedServerId, CancellationToken.None);

        server.Should().NotBeNull();
        server!.ServiceSummary?.Total.Should().BeGreaterThan(0);
        server.TasksSummary?.Total.Should().BeGreaterThan(0);
        server.IisSummary?.AppPools.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task SearchLogsAsync_FilteredBySeverity_ReturnsMatchingEntries()
    {
        var request = new LogSearchRequestDto
        {
            Severities = ["info"],
            Page = 1,
            PageSize = 10
        };

        var result = await monitoringService.SearchLogsAsync(request, CancellationToken.None);

        result.Items.Should().NotBeEmpty();
        result.Items.Should().OnlyContain(entry =>
            string.Equals(entry.Severity, "info", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task CreateAlertAsync_WithValidPayload_PersistsAlert()
    {
        var request = new CreateAlertRequestDto
        {
            ServerId = SeedServerId,
            SourceType = "service",
            SourceId = SeedServiceId,
            AlertType = "service_restart",
            Severity = "warning",
            Message = "Service restarted by test"
        };

        var created = await monitoringService.CreateAlertAsync(request, CancellationToken.None);

        created.Id.Should().NotBeEmpty();
        created.Message.Should().Be(request.Message);

        var fetched = await monitoringService.GetAlertAsync(created.Id, CancellationToken.None);
        fetched.Should().NotBeNull();
        fetched!.Status.Should().Be("open");
    }
}