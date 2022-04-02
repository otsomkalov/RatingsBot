using MediatR;

namespace Core.Requests.Item;

public record GetItem(int Id) : IRequest<Models.Item>;