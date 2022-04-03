using MediatR;

namespace Core.Requests.Item;

public record SetItemRating(int ItemId, long UserId, int RatingValue) : IRequest<Unit>;