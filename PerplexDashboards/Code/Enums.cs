using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PerplexDashboards.Code
{    
    public static class Enums
    {
        /// <summary>
        /// Levert de waarde op van het Description attribuut van een enum waarde.
        /// Indien dit attribuut niet is gezet wordt de naam van de Enum opgeleverd.
        /// </summary>
        /// <typeparam name="T">Het type van de enum</typeparam>
        /// <param name="enm">De enum waarde waarvan de Description gevonden moet worden</param>
        /// <returns></returns>
        public static string Description<TEnum>(this TEnum enm) where TEnum : struct, IConvertible
        {
            var members = typeof(TEnum).GetMember(enm.ToString());
            if (members.Length > 0)
                return members[0].GetCustomAttribute<DescriptionAttribute>()?.Description ?? enm.ToString();
            else
                return enm.ToString();
        }

        /// <summary>
        /// Levert de waarde op van het Description attribuut van een enum waarde.
        /// Indien dit attribuut niet is gezet wordt de naam van de Enum opgeleverd.
        /// </summary>
        /// <param name="enm">De enum waarvan de description moet worden opgehaald</param>
        /// <returns>Waarde van het Description-attribuut, of de naam van de Enum</returns>
        public static string Description(this Enum enm)
        {
            var members = enm.GetType().GetMember(enm.ToString());
            if (members != null)
                return members[0]?.GetCustomAttribute<DescriptionAttribute>()?.Description ?? enm.ToString();
            else
                return enm.ToString();
        }

        /// <summary>
        /// Levert een IEnuemrable op met alle descriptions van de mogelijke waardes van een bepaald Enum type
        /// </summary>
        /// <typeparam name="T">Het type van de enum</typeparam>
        /// <returns>Lijst met omschrijvingen van alle mogelijk waardes van dit Enum type</returns>
        public static IEnumerable<string> Descriptions<T>() where T : struct, IConvertible
        {
            return Values<T>().Select(v => v.Description());
        }

        /// <summary>
        /// Levert een array op van alle waardes van een bepaald Enum type
        /// </summary>
        /// <typeparam name="T">Type van de enum</typeparam>
        /// <returns>Lijst met alle enum waardes</returns>
        public static T[] Values<T>() where T : struct, IConvertible
        {
            return (T[])Enum.GetValues(typeof(T));
        }
    }

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
