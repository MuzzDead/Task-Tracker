﻿namespace TaskTracker.Client.DTOs.Card;

public class CardDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public Guid ColumnId { get; set; }
    public int RowIndex { get; set; }
}
