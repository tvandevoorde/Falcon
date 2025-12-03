using AutoMapper;
using Falcon.Application.Contracts.Admin;
using Falcon.Application.Contracts.Alerts;
using Falcon.Application.Contracts.Collectors;
using Falcon.Application.Contracts.Iis;
using Falcon.Application.Contracts.Logs;
using Falcon.Application.Contracts.Maintenance;
using Falcon.Application.Contracts.Servers;
using Falcon.Application.Contracts.Services;
using Falcon.Application.Contracts.Tasks;
using Falcon.Domain.Entities;

namespace Falcon.Application.Mappings;

/// <summary>
/// AutoMapper profile providing mapping configuration between domain models and DTOs.
/// </summary>
public sealed class MonitoringMappingProfile : Profile
{
    public MonitoringMappingProfile()
    {
        CreateMap<Server, ServerSummaryDto>()
            .ForMember(dest => dest.Environment, opt => opt.MapFrom(src => src.Environment.ToString().ToLowerInvariant()))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString().ToLowerInvariant()))
            .ForMember(dest => dest.Cpu, opt => opt.MapFrom(src => src.CpuPercent))
            .ForMember(dest => dest.Os, opt => opt.MapFrom(src => src.OperatingSystem));

        CreateMap<Server, ServerDetailDto>()
            .IncludeBase<Server, ServerSummaryDto>();

        CreateMap<MonitoredService, MonitoredServiceDto>()
            .ForMember(dest => dest.DesiredState, opt => opt.MapFrom(src => src.DesiredState.ToString().ToLowerInvariant()))
            .ForMember(dest => dest.CurrentState, opt => opt.MapFrom(src => src.CurrentState.ToString().ToLowerInvariant()));

        CreateMap<ServiceEvent, ServiceEventDto>();

        CreateMap<ScheduledTask, ScheduledTaskDto>()
            .ForMember(dest => dest.ScheduleDesc, opt => opt.MapFrom(src => src.ScheduleDescription));

        CreateMap<TaskRun, TaskRunDto>()
            .ForMember(dest => dest.Result, opt => opt.MapFrom(src => src.Result.ToString().ToLowerInvariant()));

        CreateMap<AppPool, AppPoolDto>();

        CreateMap<IisSite, IisSiteDto>();

        CreateMap<LogEntry, LogEntryDto>();

        CreateMap<LogPattern, LogPatternDto>();

        CreateMap<Alert, AlertDto>()
            .ForMember(dest => dest.Severity, opt => opt.MapFrom(src => src.Severity.ToString().ToLowerInvariant()))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToLowerInvariant()))
            .ForMember(dest => dest.Notifications, opt => opt.Ignore());

        CreateMap<Alert, AlertSummaryDto>()
            .ForMember(dest => dest.Severity, opt => opt.MapFrom(src => src.Severity.ToString().ToLowerInvariant()))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToLowerInvariant()));

        CreateMap<Notification, NotificationDto>()
            .ForMember(
                dest => dest.Channel,
                opt => opt.MapFrom((src, dest) => src.Channel != null ? src.Channel.ToString()!.ToLowerInvariant() : string.Empty)
            )
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString().ToLowerInvariant()));

        CreateMap<Collector, CollectorDto>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString().ToLowerInvariant()));

        CreateMap<User, UserDto>()
            .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.RoleAssignments.Select(r => r.RoleId.ToString())));

        CreateMap<Role, RoleDto>();

        CreateMap<MaintenanceWindow, MaintenanceWindowDto>()
            .ForMember(dest => dest.Servers, opt => opt.MapFrom(src => src.ServerScope));

        CreateMap<MetricPoint, MetricPointDto>();

        CreateMap<NotificationChannel, NotificationChannelDto>();
    }
}