﻿namespace Bali.Converter.Common.Conversion
{
    using System;
    using System.Threading.Tasks;
    using Bali.Converter.Common.Enums;

    public abstract class ConversionBase<T> : IConversion where T : IConversion
    {
        protected ConversionBase()
        {
            this.SupportedTargets = ConversionAttributeExtractor.ExtractSupportedTypes(typeof(T));
            this.Extension = ConversionAttributeExtractor.ExtractExtension(typeof(T));
        }

        public abstract ConversionTopology Topology { get; }

        public string Extension { get; }

        public Type[] SupportedTargets { get; }

        public abstract Task Convert(string source, string destination);
    }
}
