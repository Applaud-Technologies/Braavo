using MediatR;
using Braavo.Core.Models;

namespace Braavo.Core.UseCases.Prd;

public record ParsePrdCommand(Guid DocumentId) : IRequest<PrdContent?>;
