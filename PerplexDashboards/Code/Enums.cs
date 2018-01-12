using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerplexDashboards.Code
{    
    public enum MemberAuditAction
    {
        Login,
        [Description("Failed login")]
        FailedLogin,
        Logout,
        [Description("Forgot password requested")]
        ForgotPasswordRequested,
        [Description("Forgot password finished")]
        ForgotPasswordFinished,
        [Description("Changed account details")]
        UpdateAccount,
        [Description("Changed password")]
        UpdatePassword,
        [Description("Locked out")]
        LockedOut,
        Activated,
        [Description("Activated by administrator")]
        ActivatedByAdministrator,
        Register,
    }
}
