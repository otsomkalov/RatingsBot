using MediatR;

namespace Core.Requests.Rating;

public record GetRating(int ItemId, long UserId) : IRequest<Models.Rating>;