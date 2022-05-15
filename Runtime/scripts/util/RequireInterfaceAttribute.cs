using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UDrive
{
    /// <summary>
    /// Attribute that require implementation of the provided interface.
    /// </summary>
    public class RequireInterfaceAttribute : PropertyAttribute
    {
        // Interface type.
        public System.Type requiredType { get; private set; }
        /// <summary>
        /// Requiring implementation of the <see cref="T:RequireInterfaceAttribute"/> interface.
        /// </summary>
        /// <param name="type">Interface type.</param>
        public RequireInterfaceAttribute(System.Type type)
        {
            this.requiredType = type;
        }
    }
}