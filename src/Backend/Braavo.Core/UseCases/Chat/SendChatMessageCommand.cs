using MediatR;

namespace Braavo.Core.UseCases.Chat;

public record SendChatMessageCommand(
    Guid ProjectId,
    Guid? DocumentId,
    string Message,
    Guid UserId
) : IRequest<ChatResponse>;

public record ChatResponse(
    string Content,
    Guid? DocumentId,
    bool Success,
    string? Error = null
);
