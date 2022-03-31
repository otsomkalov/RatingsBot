using MediatR;

namespace Core.Commands.Item;

public record SetItemRating(long UserId, int? EntityId, int ItemId) : IRequest<Unit>;