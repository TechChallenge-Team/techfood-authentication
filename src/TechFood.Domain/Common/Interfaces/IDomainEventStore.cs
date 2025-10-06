using System.Collections.Generic;
using System.Threading.Tasks;

namespace TechFood.Domain.Common.Interfaces;

public interface IDomainEventStore
{
    Task<IEnumerable<IDomainEvent>> GetDomainEventsAsync();
}
