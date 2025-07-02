using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTracker.Application.DTOs;

public class CardDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public Guid ColumnId { get; set; }
    public int RowIndex { get; set; }
}
