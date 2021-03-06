﻿using System;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using Asynq;
using IVO.Definition.Models;
using IVO.Definition.Errors;

namespace IVO.Implementation.SQL.Persists
{
    public sealed class PersistRef : IDataOperation<Errorable<Ref>>
    {
        private Ref _rf;

        public PersistRef(Ref rf)
        {
            this._rf = rf;
        }

        public SqlCommand ConstructCommand(SqlConnection cn)
        {
            // Insert or update:
            var cmdText = String.Format(
@"IF (NOT EXISTS(SELECT [name] FROM {0} WHERE [name] = @name))
BEGIN
  INSERT INTO {0} ({1}) VALUES ({2});
END ELSE BEGIN
  UPDATE {0} SET [commitid] = @commitid WHERE [name] = @name;
END",
                Tables.TableName_Ref,
                Tables.TablePKs_Ref.Concat(Tables.ColumnNames_Ref).NameCommaList(),
                Tables.TablePKs_Ref.Concat(Tables.ColumnNames_Ref).ParameterCommaList()
            );

            var cmd = new SqlCommand(cmdText, cn);
            cmd.AddInParameter("@name", new SqlString(this._rf.Name.ToString()));
            cmd.AddInParameter("@commitid", new SqlBinary((byte[])this._rf.CommitID));
            return cmd;
        }

        public Errorable<Ref> Return(SqlCommand cmd, int rowsAffected)
        {
            return this._rf;
        }
    }
}
