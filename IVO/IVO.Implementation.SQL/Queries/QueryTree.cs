﻿using System;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using Asynq;
using IVO.Definition.Models;
using System.Collections.Generic;
using System.Data;
using IVO.Definition.Errors;
using System.Threading.Tasks;

namespace IVO.Implementation.SQL.Queries
{
    public sealed class QueryTree : IComplexDataQuery<Errorable<TreeNode>>
    {
        private TreeID _id;

        public QueryTree(TreeID id)
        {
            this._id = id;
        }

        public SqlCommand ConstructCommand(SqlConnection cn)
        {
            string pkName = Tables.TablePKs_Tree.Single();
            string cmdText = String.Format(
@"SELECT tr.name, tr.linked_treeid FROM [dbo].[TreeTree] tr WHERE [{0}] = @treeid;
SELECT bl.name, bl.linked_blobid FROM [dbo].[TreeBlob] bl WHERE [{0}] = @treeid;",
                pkName,
                Tables.TableName_Tree,
                Tables.TableFromHint_Tree
            );

            SqlCommand cmd = new SqlCommand(cmdText, cn);
            cmd.AddInParameter("@treeid", new SqlBinary((byte[])this._id));
            return cmd;
        }

        public CommandBehavior GetCustomCommandBehaviors(SqlConnection cn, SqlCommand cmd)
        {
            return CommandBehavior.Default;
        }

        public Task<Errorable<TreeNode>> RetrieveAsync(SqlCommand cmd, SqlDataReader dr, int expectedCapacity = 10)
        {
            return Task.FromResult(retrieve(cmd, dr));
        }

        public Errorable<TreeNode> Retrieve(SqlCommand cmd, SqlDataReader dr, int expectedCapacity = 10)
        {
            return retrieve(cmd, dr);
        }

        public Errorable<TreeNode> retrieve(SqlCommand cmd, SqlDataReader dr)
        {
            TreeNode.Builder tb = new TreeNode.Builder(new List<TreeTreeReference>(), new List<TreeBlobReference>());

            // Read the TreeTreeReferences:
            while (dr.Read())
            {
                var name = dr.GetSqlString(0).Value;
                var linked_treeid = (TreeID)dr.GetSqlBinary(1).Value;

                tb.Trees.Add(new TreeTreeReference.Builder(name, linked_treeid));
            }

            if (!dr.NextResult()) return new TreeIDRecordDoesNotExistError(_id);

            // Read the TreeBlobReferences:
            while (dr.Read())
            {
                var name = dr.GetSqlString(0).Value;
                var linked_blobid = (BlobID)dr.GetSqlBinary(1).Value.ToArray(20);

                tb.Blobs.Add(new TreeBlobReference.Builder(name, linked_blobid));
            }

            TreeNode tr = tb;
            if (tr.ID != _id) return new ComputedTreeIDMismatchError(tr.ID, _id);

            return tr;
        }
    }
}
