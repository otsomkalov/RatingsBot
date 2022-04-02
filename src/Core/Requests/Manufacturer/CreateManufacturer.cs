using FluentResults;
using MediatR;

namespace Core.Requests.Manufacturer;

public record CreateManufacturer(string Name) : IRequest<Result>;