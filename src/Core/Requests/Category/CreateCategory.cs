using FluentResults;
using MediatR;

namespace Core.Requests.Category;

public record CreateCategory(string Name) : IRequest<Result>;