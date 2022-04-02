using FluentResults;
using MediatR;

namespace Core.Commands.Item;

public record CreateItem(string Name) : IRequest<Result<Models.Item>>;