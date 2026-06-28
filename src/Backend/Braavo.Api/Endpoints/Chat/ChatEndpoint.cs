using System.Security.Claims;
using Braavo.Core.UseCases.Chat;
using FastEndpoints;
using MediatR;

namespace Braavo.Api.Endpoints.Chat;

public record ChatRequest(Guid ProjectId, Guid? DocumentId, string Message);

public class ChatEndpoint : Endpoint<ChatRequest, ChatResponse>
{
    private readonly IMediator _mediator;

    public ChatEndpoint(IMediator mediator) => _mediator = mediator;

    public override void Configure()
    {
        Post("/api/chat/message");
        Policies("Authenticated");
    }

    public override async Task HandleAsync(ChatRequest req, CancellationToken ct)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        var command = new SendChatMessageCommand(
            req.ProjectId,
            req.DocumentId,
            req.Message,
            userId
        );

        var result = await _mediator.Send(command, ct);

        if (result.Success)
            await SendOkAsync(result, ct);
        else
            await SendAsync(result, 500, ct);
    }
}
