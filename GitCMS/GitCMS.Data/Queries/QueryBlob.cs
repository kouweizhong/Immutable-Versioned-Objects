﻿using System;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using Asynq;
using GitCMS.Definition.Models;

namespace GitCMS.Data.Queries
{
    public sealed class QueryBlob : ISimpleDataQuery<Blob>
    {
        private BlobID _id;

        public QueryBlob(BlobID id)
        {
            this._id = id;
        }

        public SqlCommand ConstructCommand(SqlConnection cn)
        {
            string cmdText = String.Format(
                @"SELECT {0} FROM {1}{2}{3} WHERE blobid = @blobid",
                Tables.TablePKs_Blob.Concat(Tables.ColumnNames_Blob).NameList(),
                Tables.TableName_Blob,
                "", // no alias
                Tables.TableFromHint_Blob
            );

            SqlCommand cmd = new SqlCommand(cmdText, cn);
            cmd.AddInParameter("@blobid", new SqlBinary((byte[])_id));
            return cmd;
        }

        public Blob Project(SqlDataReader dr)
        {
            BlobID id = (BlobID) dr.GetSqlBinary(0).Value;

            Blob.Builder b = new Blob.Builder();
            b.Contents = dr.GetSqlBinary(1).Value;
            
            var bl = (Blob)b;
#if DEBUG
            if (bl.ID != id) throw new InvalidOperationException();
#endif

            return bl;
        }
    }
}
