using MediatR;
using Microsoft.Extensions.Logging;

namespace Ordering.Application.Behaviours
{
    public class UnhandledExceptionBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ILogger<TRequest> _log;

        public UnhandledExceptionBehaviour(ILogger<TRequest> log)
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            try 
            {
                return await next();
            }
            catch (Exception ex) 
            {
                var requestName = typeof(TRequest).Name;
                _log.LogError(ex,"Application Request: Unhandled Exception for Request {name}, {Request}", requestName, request);
                throw;
            }
        }
    }
}
