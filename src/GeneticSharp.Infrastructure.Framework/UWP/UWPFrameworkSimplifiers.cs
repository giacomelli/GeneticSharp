/* 
 * This file provides classes that simplify porting the library to UWP.
 */


using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace System.ComponentModel
{

    /// <devdoc> 
    ///    <para>Specifies the display name for a property or event.  The default is the name of the property or event.</para>
    /// </devdoc> 
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1813:AvoidUnsealedAttributes")]
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Event | AttributeTargets.Class | AttributeTargets.Method)]
    public class DisplayNameAttribute : Attribute
    {
        public DisplayNameAttribute() : this(string.Empty)
        {
        }
        public DisplayNameAttribute(string displayName)
        {
            this.DisplayName = displayName;
        }
        public string DisplayName { get; }
    }
}

namespace HelperSharp
{
    public static class Helper
    {
        public static string With(this string source, params object[] args)
        {
            return string.Format((IFormatProvider)CultureInfo.InvariantCulture, source, args);
        }
    }

    public static class ExceptionHelper
    {
        public static void ThrowIfNull(string argumentName, object argument)
        {
            if (argument == null)
                throw new ArgumentNullException(argumentName);
        }
    }
}

namespace System.Security.Permissions
{
    public class SerializableAttribute : Attribute
    {

    }
}
