using System.ComponentModel.DataAnnotations.Schema;
using MediatR;

namespace ModShots.Domain.Common;

public abstract class BaseEntity<TId> : IHasDomainEvents where TId : IComparable<TId>, IEquatable<TId>
{
    public TId Id { get; init; } = default!;
    
    private readonly List<BaseDomainEvent> _domainEvents = [];
    
    public void AddDomainEvent(BaseDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public List<BaseDomainEvent> GetUncommittedDomainEvents()
    {
        var uncommittedDomainEvents = _domainEvents.ToList();
        _domainEvents.Clear();
        return uncommittedDomainEvents;
    }
}