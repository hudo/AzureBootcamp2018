using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BootShop.Service.EmailSender
{
    public abstract class BackgroundService : IHostedService, IDisposable
    {
        private readonly ILogger<BackgroundService> _logger;
        private readonly CancellationTokenSource _stoppingTokenSource = new CancellationTokenSource(); 

        private Task _executingTask;

        protected BackgroundService(ILogger<BackgroundService> logger)
        {
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Background service '{this.GetType().Name}' starting");

            _executingTask = ExecuteAsync(_stoppingTokenSource.Token);

            return _executingTask.IsCompleted 
                ? _executingTask 
                : Task.CompletedTask;
        }

        protected abstract Task ExecuteAsync(CancellationToken cancellationToken);
        

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_executingTask == null) return;

            try
            {
                _stoppingTokenSource.Cancel();
            }
            finally
            {
                await Task.WhenAny(_executingTask, Task.Delay(TimeSpan.FromSeconds(5), cancellationToken));
            }
        }

        public void Dispose()
        {
            _stoppingTokenSource.Cancel();
        }
    }
}