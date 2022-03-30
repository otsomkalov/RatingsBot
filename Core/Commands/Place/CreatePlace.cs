using MediatR;

namespace Core.Commands.Place;

public record CreatePlace(string Name) : IRequest;