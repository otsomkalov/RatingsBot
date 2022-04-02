using FluentResults;
using MediatR;

namespace Core.Commands.Place;

public record CreatePlace(string Name) : IRequest<Result>;