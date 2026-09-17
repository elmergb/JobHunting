using JobHunting.Domain.Entities;
using JobHunting.Domain.Primatives;
using System;
using System.Collections.Generic;
using System.Text;
using ApplicationId = JobHunting.Domain.Primatives.ApplicationId;

namespace JobHunting.Domain.Repositories
{
    public interface IDocumentRepository : IRepository<Document, DocumentId>
    {
        /// <summary>
        /// Returns all documents belonging to a user, optionally filtered by type.
        /// </summary>
        Task<IReadOnlyList<Document>> GetByUserIdAsync(string userId, CancellationToken ct = default);

        /// <summary>
        /// Returns all documents attached to a specific job application.
        /// </summary>
        Task<IReadOnlyList<Document>> GetByApplicationIdAsync(ApplicationId applicationId, CancellationToken ct = default);

        /// <summary>
        /// Returns the master/base version of a document type for a user (e.g. master resume).
        /// </summary>
        Task<Document?> GetMasterAsync(string userId, DocumentType type, CancellationToken ct = default);
    }
}
