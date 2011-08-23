﻿using System;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using Asynq;
using GitCMS.Definition.Models;

namespace GitCMS.Data.Persists
{
    public sealed class DestroyBlob : DataQuery<int>
    {
        private BlobID _id;

        public DestroyBlob(BlobID id)
        {
            this._id = id;
        }

        public override SqlCommand ConstructCommand(SqlConnection cn)
        {
            string pkName = Tables.TablePKs_Blob.Single();
            var cmdText = String.Format(
                @"DELETE FROM {0} WHERE [{1}] = @{1}",
                Tables.TableName_Blob,
                pkName
            );

            var cmd = new SqlCommand(cmdText, cn);
            cmd.AddInParameter("@" + pkName, new SqlBinary((byte[])_id));
            return cmd;
        }

        public override int Project(SqlDataReader dr)
        {
            throw new NotImplementedException();
        }
    }
}
