using MediatR;

namespace Core.Commands.Item;

public record SetItemCategory(int EntityId, int ItemId) : IRequest;