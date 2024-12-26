using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using ModShots.Domain.Common;

namespace ModShots.Application.Data.Interceptors;

public class DispatchDomainEventsInterceptor : SaveChangesInterceptor
{
    private readonly IPublisher _notificationPublisher;

    public DispatchDomainEventsInterceptor(IPublisher notificationPublisher)
    {
        _notificationPublisher = notificationPublisher;
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        var dbContext = eventData.Context;
        
        if (dbContext is not null)
        {
            DispatchDomainEventsAsync(dbContext).GetAwaiter().GetResult();
        }
        
        return base.SavingChanges(eventData, result);
    }
    
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData, 
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        var dbContext = eventData.Context;
        
        if (dbContext is not null)
        {
            await DispatchDomainEventsAsync(dbContext, cancellationToken);
        }
        
        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private async Task DispatchDomainEventsAsync(DbContext dbContext, CancellationToken cancellationToken = default)
    {
        var domainEvents = dbContext.ChangeTracker
            .Entries<IHasDomainEvents>()
            .SelectMany(entry => entry.Entity.GetUncommittedDomainEvents())
            .ToList();

        foreach (var domainEvent in domainEvents)
        {
            await _notificationPublisher.Publish(domainEvent, cancellationToken);
        }
    }
}