﻿using MediatR;

namespace TaskTracker.Application.Features.Comment.Commands.Delete;

public class DeleteCommentCommand : IRequest
{
    public Guid Id { get; set; }
}
