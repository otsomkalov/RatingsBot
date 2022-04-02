using FluentResults;
using MediatR;

namespace Core.Requests.Place;

public record CreatePlace(string Name) : IRequest<Result>;