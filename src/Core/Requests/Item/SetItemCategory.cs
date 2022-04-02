using MediatR;

namespace Core.Requests.Item;

public record SetItemCategory(int EntityId, int ItemId) : IRequest;