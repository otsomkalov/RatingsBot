using FluentResults;
using MediatR;

namespace Core.Requests.Item;

public record CreateItem(string Name) : IRequest<Result<Models.Item>>;