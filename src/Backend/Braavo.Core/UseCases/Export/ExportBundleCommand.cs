using MediatR;

namespace Braavo.Core.UseCases.Export;

public record ExportBundleCommand(Guid DocumentId, bool IncludeDiagrams, Guid UserId) : IRequest<ExportResult>;

public record ExportResult(
    byte[] ZipContent,
    string FileName,
    bool Success,
    string? Error = null
);
