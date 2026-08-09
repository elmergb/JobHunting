using JobHunting.Domain.Primatives;
using System;
using System.Collections.Generic;
using System.Text;
using ApplicationId = JobHunting.Domain.Primatives.ApplicationId;

namespace JobHunting.Domain.Entities
{
    public class Document : AggregateRoot<DocumentId>
    {
        public string UserId { get; private set; }
        public ApplicationId? ApplicationId { get; private set; }
        public DocumentType Type { get; private set; }
        public string FileName { get; private set; }
        public string StoragePath { get; private set; }
        public string? ParsedText { get; private set; }
        public bool IsMasterVersion { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private Document() { }

        public static Document Create(string userId, string fileName, string storagePath, DocumentType type, bool isMaster = false)
        {
            return new Document
            {
                Id = DocumentId.New(),
                UserId = userId,
                FileName = fileName,
                StoragePath = storagePath,
                Type = type,
                IsMasterVersion = isMaster,
                CreatedAt = DateTime.UtcNow
            };
        }
    }

    public enum DocumentType
    {
        Resume,
        CoverLetter,
        OfferLetter,
        Other
    }
}
