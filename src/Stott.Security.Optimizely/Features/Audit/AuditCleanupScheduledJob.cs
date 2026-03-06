namespace Stott.Security.Optimizely.Features.Audit;

using System;
using System.Threading.Tasks;

using EPiServer.DataAbstraction;
using EPiServer.Logging;
using EPiServer.Scheduler;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Features.Configuration;

[ScheduledJob(
    DisplayName = "[Stott Security] Audit Record Clean Up",
    Description = "Clears down audit records that are older than the configured retention period (default: 2 years).",
    GUID = "d7f1c8a4-9b2e-4f5a-8c3d-1e6f9a7b4c2d",
    DefaultEnabled = true,
    IntervalType = ScheduledIntervalType.Weeks,
    IntervalLength = 1,
    Restartable = false)]
public sealed class AuditCleanupScheduledJob : ScheduledJobBase
{
    private readonly IAuditRepository _repository;

    private readonly SecurityConfiguration _configuration;

    private readonly ILogger _logger = LogManager.GetLogger(typeof(AuditCleanupScheduledJob));

    public AuditCleanupScheduledJob(IAuditRepository repository, SecurityConfiguration configuration)
    {
        _repository = repository;
        _configuration = configuration;
    }

    public override string Execute()
    {
        try
        {
            var retentionPeriod = _configuration.AuditRetentionPeriod;
            var threshold = DateTime.Today.AddDays(-retentionPeriod.TotalDays);
            var itemsDeleted = Task.Run(() => _repository.DeleteAsync(threshold, CspConstants.AuditDeletionBatchSize)).Result;

            var retentionDays = (int)retentionPeriod.TotalDays;
            var message = $"{itemsDeleted} Audit Record(s) older than {retentionDays} days were deleted.";

            if (itemsDeleted >= CspConstants.AuditDeletionBatchSize)
            {
                message += " Additional old records may remain and will be processed in subsequent runs.";
            }

            return message;
        }
        catch (Exception exception)
        {
            _logger.Error($"{CspConstants.LogPrefix} Failure encountered when clearing down Audit Records.", exception);

            return $"An error was encountered: {exception.Message}";
        }
    }
}
