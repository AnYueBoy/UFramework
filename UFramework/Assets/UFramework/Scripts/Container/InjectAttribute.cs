using System;
namespace UFramework.Container {

    public class InjectAttribute : Attribute {

        /// <summary>
        /// Gets or sets a value indicating whether the property is required.
        /// </summary>
        public bool Required { get; set; } = true;
    }
}