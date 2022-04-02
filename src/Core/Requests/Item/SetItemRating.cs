using MediatR;

namespace Core.Requests.Item;

public record SetItemRating(long UserId, int EntityId, int ItemId) : IRequest<Unit>;