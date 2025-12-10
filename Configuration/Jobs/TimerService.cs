namespace yeni.Configuration;

public class TimerService : IHostedService, IDisposable
{
    private Timer? _timer;
    private readonly IServiceScopeFactory _scopeFactory;

    public TimerService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        ScheduleNextRun();
        return Task.CompletedTask;
    }

    private void ScheduleNextRun()
    {
        var now = DateTime.Now;

        var targetTime = new DateTime(
            now.Year,
            now.Month,
            now.Day,
            14, 50, 0
        );

        if (now > targetTime)
            targetTime = targetTime.AddDays(1);

        var delay = targetTime - now;

        _timer = new Timer(async _ =>
        {
            await ExecuteTask();
            ScheduleNextRun(); 

        }, null, delay, Timeout.InfiniteTimeSpan);
    }

    private async Task ExecuteTask()
    {
        using var scope = _scopeFactory.CreateScope();
        var mailJob = scope.ServiceProvider.GetRequiredService<MailJob>();

        await mailJob.ExecuteAsync(CancellationToken.None);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}