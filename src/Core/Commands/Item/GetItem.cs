using MediatR;

namespace Core.Commands.Item;

public record GetItem(int Id) : IRequest<Models.Item>;