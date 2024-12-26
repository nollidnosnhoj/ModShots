using MediatR;

namespace ModShots.Domain.Common;

public interface IHasDomainEvents
{
    public void AddDomainEvent(BaseDomainEvent domainEvent);
    public List<BaseDomainEvent> GetUncommittedDomainEvents();
}

public abstract class BaseDomainEvent : INotification;