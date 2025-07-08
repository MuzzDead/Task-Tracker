using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTracker.Application.Common.Interfaces.Services;

public interface ICurrentUserService
{
    Guid GetCurrentUserId();
    string GetCurrentUserEmail();
    string GetCurrentUsername();
    bool IsAuthenticated();
}
