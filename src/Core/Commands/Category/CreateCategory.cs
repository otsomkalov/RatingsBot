using FluentResults;
using MediatR;

namespace Core.Commands.Category;

public record CreateCategory(string Name) : IRequest<Result>;