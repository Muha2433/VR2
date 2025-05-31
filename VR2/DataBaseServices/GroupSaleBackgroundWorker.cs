using System.Collections.Concurrent;
using VR2.DTOqMoels;

namespace VR2.DataBaseServices
{

    public class GroupSaleBackgroundWorker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ConcurrentQueue<(string requestId, DtoGroupRequestSale request)> _queue = new();

        public GroupSaleBackgroundWorker(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void EnqueueRequest(string requestId, DtoGroupRequestSale request)
        {
            _queue.Enqueue((requestId, request));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_queue.TryDequeue(out var item))
                {
                    using var scope = _serviceProvider.CreateScope();
                    var crudService = scope.ServiceProvider.GetRequiredService<crudRequestSale>();

                    try
                    {
                        var result = await crudService.AddGroupRequestForSale(item.request);
                        Console.WriteLine($"Processed: {item.requestId} => {result}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error processing request {item.requestId}: {ex.Message}");
                    }

                    await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken); // Delay between jobs
                }
                else
                {
                    await Task.Delay(1000, stoppingToken); // No jobs, wait before checking again
                }
            }
        }

        public async Task ProcessRequestAsync(DtoGroupRequestSale req)
        {
            using var scope = _serviceProvider.CreateScope();
            var crud = scope.ServiceProvider.GetRequiredService<crudRequestSale>();

            var result = await crud.AddGroupRequestForSale(req);

            Console.WriteLine($"[BackgroundWorker] AddGroupRequestForSale result: {result}");
        }
    }
}
