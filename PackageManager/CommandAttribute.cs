using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Composite.Integration.Nuget.PackageManager
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class CommandAttribute : Attribute
    {
        private string _description;
        private string _usageSummary;
        private string _usageDescription;
        private string _example;

        public string CommandName { get; private set; }
        public Type ResourceType { get; private set; }
        public string DescriptionResourceName { get; private set; }


        public string AltName { get; set; }
        public int MinArgs { get; set; }
        public int MaxArgs { get; set; }
        public string UsageSummaryResourceName { get; set; }
        public string UsageDescriptionResourceName { get; set; }
        public string UsageExampleResourceName { get; set; }

        public string Description
        {
            get
            {

                return _description;
            }
            private set
            {
                _description = value;
            }
        }

        public string UsageSummary
        {
            get
            {

                return _usageSummary;
            }
            set
            {
                _usageSummary = value;
            }
        }

        public string UsageDescription
        {
            get
            {

                return _usageDescription;
            }
            set
            {
                _usageDescription = value;
            }
        }

        public string UsageExample
        {
            get
            {

                return _example;
            }
            set
            {
                _example = value;
            }
        }

        public CommandAttribute(string commandName, string description)
        {
            CommandName = commandName;
            Description = description;
            MinArgs = 0;
            MaxArgs = Int32.MaxValue;
        }

        public CommandAttribute(Type resourceType, string commandName, string descriptionResourceName)
        {
            ResourceType = resourceType;
            CommandName = commandName;
            DescriptionResourceName = descriptionResourceName;
            MinArgs = 0;
            MaxArgs = Int32.MaxValue;
        }
    }
}
