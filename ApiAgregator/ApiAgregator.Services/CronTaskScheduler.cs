using ApiAgregator.Entities;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;

namespace ApiAgregator.Services;

public class CronTaskScheduler : IHostedService
{
    private Timer? _timer;
    private readonly ILogger<CronTaskScheduler> _logger;
    private readonly IServiceProvider _serviceProvider;

    private readonly ConcurrentDictionary<int, (CronExpression expression, Action<IServiceProvider> action)> _tasks;

    private volatile bool _stopped;
    private Semaphore _semaphore;
    private ManualResetEvent _stopEvent;

    public CronTaskScheduler(IOptions<CronTaskSchedulerOptions> options, ILogger<CronTaskScheduler> logger,
        IServiceProvider serviceProvider)
    {
        ArgumentNullException.ThrowIfNull(options.Value.ThreadCount);

        var threadCount = options.Value.ThreadCount.Value;

        _logger = logger;
        _serviceProvider = serviceProvider;

        _stopEvent = new ManualResetEvent(false);
        _semaphore = new Semaphore(threadCount, threadCount);
        _tasks = new();
    }

    private readonly object _locker = new object();

    public Task StartAsync(CancellationToken cancellationToken)
    {
        lock (_locker)
        {
            _stopEvent.Reset();
            _stopped = false;
            _timer = new Timer(OnTick, null, (60 - DateTime.Now.Second) * 1000 + 100, 60 * 1000);
            _logger.LogInformation("started");
            return Task.CompletedTask;
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        lock (_locker)
        {
            _timer?.Dispose();
            _stopped = true;
            _stopEvent.Set();
            _logger.LogInformation("stopped");
            return Task.CompletedTask;
        }
    }

    private void OnTick(object? _)
    {
        var waitHandles = new WaitHandle[] { _semaphore, _stopEvent };

        var time = DateTime.Now;

        lock (_locker)
        {
            _timer!.Dispose();

            if (_stopped)
            {
                return;
            }

            foreach (var kv in _tasks)
            {
                if (!kv.Value.expression.Match(time))
                    continue;

                bool semaphoreAcquired = WaitHandle.WaitAny(waitHandles) == 0;

                Task.Run(() =>
                {
                    try
                    {
                        kv.Value.action(_serviceProvider);
                    }
                    finally
                    {
                        _semaphore.Release();
                    }
                });
            }

            var nTime = DateTime.Now;

            _timer = new Timer(OnTick, null, (60 - nTime.Second) * 1000 + 100, 60 * 1000);

            int mins = (int)(nTime - time).TotalMinutes;

            if (mins > 0)
            {
                _logger.LogCritical($"Tasks are taking too long. Skipped {mins} calls.");
            }
        }   
    }

    public void Set(int id, CronExpression expression, Action<IServiceProvider> action)
    {
        _tasks[id] = (expression, action);   
    }

    public void Remove(int id)
    {
        _tasks.Remove(id, out var _);
    }
}

public class CronTaskSchedulerOptions
{
    public int? ThreadCount { get; set; }
}

