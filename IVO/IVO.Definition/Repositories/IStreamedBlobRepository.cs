﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IVO.Definition.Models;
using IVO.Definition.Containers;

namespace IVO.Definition.Repositories
{
    public interface IStreamedBlobRepository
    {
        /// <summary>
        /// Persists multiple Blobs asynchronously.
        /// </summary>
        /// <remarks>
        /// Input array length and order must be retained in the returned array.
        /// </remarks>
        /// <param name="blobs"></param>
        /// <returns></returns>
        Task<IStreamedBlob[]> PersistBlobs(params PersistingBlob[] blobs);

        /// <summary>
        /// Deletes multiple Blobs by BlobIDs asynchronously.
        /// </summary>
        /// <remarks>
        /// Input array length and order must be retained in the returned array.
        /// </remarks>
        /// <param name="ids"></param>
        /// <returns></returns>
        Task<BlobID[]> DeleteBlobs(params BlobID[] ids);

        /// <summary>
        /// Retrieves multiple Blobs by BlobIDs asynchronously.
        /// </summary>
        /// <remarks>
        /// Input array length and order must be retained in the returned array.
        /// </remarks>
        /// <param name="ids"></param>
        /// <returns></returns>
        Task<IStreamedBlob[]> GetBlobs(params BlobID[] ids);

    }
}