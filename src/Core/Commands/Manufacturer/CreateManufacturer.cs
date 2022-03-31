using MediatR;

namespace Core.Commands.Manufacturer;

public record CreateManufacturer(string Name) : IRequest;