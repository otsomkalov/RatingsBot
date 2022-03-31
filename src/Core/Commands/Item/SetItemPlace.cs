using MediatR;

namespace Core.Commands.Item;

public record SetItemPlace(int? EntityId, int Item) : IRequest;