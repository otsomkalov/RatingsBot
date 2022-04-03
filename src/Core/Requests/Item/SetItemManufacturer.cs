using MediatR;

namespace Core.Requests.Item;

public record SetItemManufacturer(int ItemId, int? ManufacturerId) : IRequest;