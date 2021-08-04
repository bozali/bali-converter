namespace Bali.Converter.App.Modules.Conversion
{
    using System;
    using System.Linq;

    using Bali.Converter.Common.Enums;

    public class ConversionMetadata
    {
        public string SourcePath { get; set; }

        public DocumentExtension Extension { get; set; }

        public DocumentExtension[] SupportedTargetFormats
        {
            get
            {
                switch (this.GetDocumentDomain(this.Extension))
                {
                    case DocumentDomain.Image:
                        return new[]
                            {
                                DocumentExtension.Jpeg,
                                DocumentExtension.Png,
                                DocumentExtension.Bmp,
                                DocumentExtension.Gif,
                                DocumentExtension.Ico,
                                DocumentExtension.WebP
                            }.Except(new[] { this.Extension })
                             .ToArray();

                    case DocumentDomain.Audio:
                        return new[]
                            {
                                DocumentExtension.Ogg,
                                DocumentExtension.MP3,
                            }.Except(new[] { this.Extension })
                             .ToArray();

                    case DocumentDomain.Video:
                        return new[]
                            {
                                DocumentExtension.MP3,
                                DocumentExtension.MP4,
                                DocumentExtension.Wav,
                                DocumentExtension.WebM,
                                DocumentExtension.Ogg,
                                DocumentExtension.Gif
                            }.Except(new[] { this.Extension })
                             .ToArray();

                    default:
                        throw new ArgumentOutOfRangeException(nameof(this.Extension));
                }
            }
        }

        private DocumentDomain GetDocumentDomain(DocumentExtension from)
        {
            switch (from)
            {
                case DocumentExtension.Jpeg:
                case DocumentExtension.Png:
                case DocumentExtension.Bmp:
                case DocumentExtension.Gif:
                case DocumentExtension.Ico:
                case DocumentExtension.WebP:
                    return DocumentDomain.Image;
            }

            return DocumentDomain.Video;
        }
    }
}
