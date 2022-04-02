using MediatR;

namespace Core.Requests.Item;

public record SetItemManufacturer(int? EntityId, int Item) : IRequest;