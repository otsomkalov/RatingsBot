using MediatR;

namespace Core.Requests.Item;

public record SetItemPlace(int ItemId, int? PlaceId) : IRequest;