﻿using System;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using Asynq;
using IVO.Definition.Models;

namespace IVO.Implementation.SQL.Persists
{
    public sealed class DestroyTag : IDataOperation<TagID?>
    {
        private TagID _id;

        public DestroyTag(TagID id)
        {
            this._id = id;
        }

        public SqlCommand ConstructCommand(SqlConnection cn)
        {
            string pkName = Tables.TablePKs_Tag.Single();
            var cmdText = String.Format(
                @"DELETE FROM {0} WHERE [{1}] = @{1}",
                Tables.TableName_Tag,
                pkName
            );

            var cmd = new SqlCommand(cmdText, cn);
            cmd.AddInParameter("@" + pkName, new SqlBinary((byte[])_id));
            return cmd;
        }

        public TagID? Return(SqlCommand cmd, int rowsAffected)
        {
            if (rowsAffected == 0) return (TagID?)null;
            return (TagID?)this._id;
        }
    }
}
