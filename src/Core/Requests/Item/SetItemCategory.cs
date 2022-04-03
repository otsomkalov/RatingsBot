using MediatR;

namespace Core.Requests.Item;

public record SetItemCategory(int ItemId, int CategoryId) : IRequest;