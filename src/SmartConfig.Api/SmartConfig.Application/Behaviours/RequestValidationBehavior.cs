using System.Net;
using System.Text;
using FluentValidation;
using MediatR;
using SmartConfig.Common.Exceptions;

namespace SmartConfig.Application.Behaviours;

public class RequestValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
	where TRequest : IRequest<TResponse>
{
	private readonly IEnumerable<IValidator<TRequest>> _validators;

	public RequestValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
	{
		_validators = validators;
	}

	public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
	{
		if (!_validators.Any())
			return await next();

		var context = new ValidationContext<TRequest>(request);
		var errorsDictionary = _validators
			.Select(x => x.Validate(context))
			.SelectMany(x => x.Errors)
			.Where(x => x != null)
			.GroupBy(
				x => x.PropertyName,
				x => x.ErrorMessage,
				(propertyName, errorMessages) => new
				{
					Key = propertyName,
					Values = errorMessages.Distinct().ToArray()
				})
			.ToDictionary(x => x.Key, x => x.Values);

		if (errorsDictionary.Any())
		{
			StringBuilder message = new StringBuilder();
			foreach (var keyValuePair in errorsDictionary)
				message?.Append($"{keyValuePair.Key}: {string.Join(',', keyValuePair.Value)} ");

			throw new SmartConfigException(HttpStatusCode.BadRequest, message!.ToString());
		}

		return await next();
	}
}