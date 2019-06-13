using System;
using System.Linq;

namespace DFDS.CapabilityService.WebApi.Domain.Models
{
    public class Visibility : StringSubstitutable
    {
        private static readonly string[] AcceptableValues = {"private", "public"};

        private Visibility(string value) : base(value)
        {
        }

        public static Visibility CreatePrivate()
        {
            return Create("private");
        }
        
        public static Visibility CreatePublic()
        {
            return Create("public");
        }
        public static Visibility Create(string value)
        {
            value = value.ToLower();

            if (AcceptableValues.Contains(value))
            {
                return new Visibility(value);
            }

            throw new ArgumentException(
                $"Cannot create Visibility object value given: '{value}' is not one of acceptable values {AcceptableValues}");
        }
    }
}