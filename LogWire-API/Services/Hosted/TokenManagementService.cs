using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using LogWire.API.Data.Model;
using LogWire.API.Data.Repository;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LogWire.API.Services.Hosted
{
    public class TokenManagementService : IHostedService
    {
        private static System.Timers.Timer timer;
        private IServiceScopeFactory _scopeFactory;

        public TokenManagementService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {

            timer = new System.Timers.Timer();
            timer.Interval = 60000;
            timer.Elapsed += OnTimedEvent;
            timer.AutoReset = true;
            timer.Enabled = true;

            OnTimedEvent(this, null);

            return Task.CompletedTask;

        }

        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {

            using var scope = _scopeFactory.CreateScope();
            RefreshTokenRepository repo = scope.ServiceProvider.GetRequiredService<IDataRepository<RefreshTokenEntry>>() as RefreshTokenRepository;

            foreach (var refreshTokenEntry in repo.GetExpiredTokens(60))
            {
                repo.Delete(refreshTokenEntry);
            }

        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            timer.Dispose();
            return Task.CompletedTask;
        }
    }
}
