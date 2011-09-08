﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IVO.Definition.Models
{
    /// <summary>
    /// Represents streamed read-only access to a Blob's contents.
    /// </summary>
    public interface IStreamedBlob
    {
        /// <summary>
        /// Gets the ID of this Blob.
        /// </summary>
        BlobID ID { get; }

        /// <summary>
        /// Starts a Task to open the data stream from the persistence store, passes it to the <paramref name="read"/>
        /// function to read from and then closes the underlying Stream when the lambda returns. The Stream must not
        /// leave the lambda's scope. This method returns the value returned from the <paramref name="read"/> function.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="read">Reads content from the underlying Stream until complete.</param>
        /// <returns></returns>
        Task<TResult> ReadStream<TResult>(Func<System.IO.Stream, TResult> read);

        /// <summary>
        /// Starts a Task to open the data stream from the persistence store, passes it to the <paramref name="read"/>
        /// function to read from and then closes the underlying Stream when the lambda returns. The Stream must not
        /// leave the lambda's scope.
        /// </summary>
        /// <param name="read">Reads content from the underlying Stream until complete.</param>
        /// <returns></returns>
        Task ReadStream(Action<System.IO.Stream> read);
    }
}
