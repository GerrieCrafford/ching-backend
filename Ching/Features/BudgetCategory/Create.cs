namespace Ching.Features.BudgetCategory;

using System.Threading;
using System.Threading.Tasks;
using MediatR;

public class Create
{
    public class Command : IRequest<bool>
    {
        public string Name { get; set; }
    }

    public class Handler : IRequestHandler<Command, bool>
    {
        public Task<bool> Handle(Command request, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Got name: {request.Name}");
            return Task.FromResult(true);
        }
    }
}