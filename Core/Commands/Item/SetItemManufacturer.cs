using MediatR;

namespace Core.Commands.Item;

public record SetItemManufacturer(int? EntityId, int Item) : IRequest;