#region Copyright (c) 2007 Ryan Williams <drcforbin@gmail.com>

/// <copyright>
/// Copyright (c) 2007 Ryan Williams <drcforbin@gmail.com>
/// 
/// Permission is hereby granted, free of charge, to any person obtaining a copy
/// of this software and associated documentation files (the "Software"), to deal
/// in the Software without restriction, including without limitation the rights
/// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
/// copies of the Software, and to permit persons to whom the Software is
/// furnished to do so, subject to the following conditions:
/// 
/// The above copyright notice and this permission notice shall be included in
/// all copies or substantial portions of the Software.
/// 
/// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
/// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
/// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
/// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
/// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
/// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
/// THE SOFTWARE.
/// </copyright>

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Mono.Cecil;

namespace Obfuscar
{
    class PropertyTester : IPredicate<PropertyKey>
    {
        private readonly string name;
        private readonly Regex nameRx;
        private readonly string type;
        private readonly string attrib;
        private readonly string typeAttrib;

        public PropertyTester(string name, string type, string attrib, string typeAttrib)
        {
            this.name = name;
            this.type = type;
            this.attrib = attrib;
            this.typeAttrib = typeAttrib;
        }

        public PropertyTester(Regex nameRx, string type, string attrib, string typeAttrib)
        {
            this.nameRx = nameRx;
            this.type = type;
            this.attrib = attrib;
            this.typeAttrib = typeAttrib;
        }

        private static MethodAttributes GetPropertyMethodAttributes(PropertyDefinition prop)
        {
            if (prop.GetMethod != null)
                return prop.GetMethod.Attributes;
            if (prop.SetMethod != null)
                return prop.SetMethod.Attributes;
            return 0;
        }

        public bool Test(PropertyKey prop, InheritMap map)
        {
            // method name matches type regex?
            if (!String.IsNullOrEmpty(type) && !Helper.CompareOptionalRegex(prop.TypeKey.Fullname, type))
            {
                return false;
            }

            // method visibility matches
            if (MethodTester.CheckMemberVisibility(attrib, typeAttrib, GetPropertyMethodAttributes(prop.Property), prop.DeclaringType))
            {
                return false;
            }

            // method's name matches
            if (nameRx != null && !nameRx.IsMatch(prop.Name))
            {
                return false;
            }

            // method's name matches
            if (!string.IsNullOrEmpty(name) && !Helper.CompareOptionalRegex(prop.Name, name))
            {
                return false;
            }

            return true;
        }
    }
}
