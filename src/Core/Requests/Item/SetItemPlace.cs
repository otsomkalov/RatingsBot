using MediatR;

namespace Core.Requests.Item;

public record SetItemPlace(int? EntityId, int Item) : IRequest;