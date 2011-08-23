﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asynq
{
    /// <summary>
    /// Represents the context used for data queries.
    /// </summary>
    public sealed class DataContext
    {
        private string connectionString;

        private DataContext(SqlConnectionStringBuilder connStringBuilder)
        {
            // TODO: is it necessary to force this here?
            connStringBuilder.AsynchronousProcessing = true;

            this.connectionString = connStringBuilder.ConnectionString;
        }

        /// <summary>
        /// Constructs a new context using the given connection string.
        /// </summary>
        /// <param name="connectionString"></param>
        public DataContext(string connectionString)
            : this(new SqlConnectionStringBuilder(connectionString))
        {
        }

        /// <summary>
        /// Asynchronously execute the given query expected to return 0 or more rows.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="expectedCapacity"></param>
        /// <param name="factory"></param>
        /// <returns></returns>
        public Task<List<T>> AsynqMulti<T>(DataQuery<T> query, int expectedCapacity = 10, TaskFactory<List<T>> factory = null)
        {
            if (expectedCapacity < 0) expectedCapacity = 0;
            if (factory == null) factory = new TaskFactory<List<T>>();

            var cn = new SqlConnection(this.connectionString);
            var cmd = query.ConstructCommand(cn);
            cn.Open();

            return factory.FromAsync(
                cmd.BeginExecuteReader(CommandBehavior.CloseConnection),
                ar =>
                {
                    try
                    {
                        var dr = cmd.EndExecuteReader(ar);

                        // Build up the result list:
                        List<T> results = new List<T>(expectedCapacity);
                        while (dr.Read())
                        {
                            var row = query.Project(dr);
                            results.Add(row);
                        }

                        return results;
                    }
                    finally
                    {
                        cn.Close();
                    }
                }
            );
        }

        /// <summary>
        /// Asynchronously execute the given query expected to return 0 or 1 items.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="factory"></param>
        /// <returns></returns>
        public Task<T> AsynqSingle<T>(DataQuery<T> query, TaskFactory<T> factory = null)
        {
            if (factory == null) factory = new TaskFactory<T>();

            var cn = new SqlConnection(this.connectionString);
            var cmd = query.ConstructCommand(cn);
            cn.Open();

            return factory.FromAsync(
                cmd.BeginExecuteReader(CommandBehavior.CloseConnection | CommandBehavior.SingleRow),
                ar =>
                {
                    try
                    {
                        var dr = cmd.EndExecuteReader(ar);

                        // If no row read, return the default:
                        if (!dr.Read()) return default(T);

                        var row = query.Project(dr);

                        return row;
                    }
                    finally
                    {
                        cn.Close();
                    }
                }
            );
        }

        /// <summary>
        /// Asynchronously execute the given query expected to not return any values.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="factory"></param>
        /// <returns></returns>
        public Task<int> AsynqNonQuery(DataQuery<int> query, TaskFactory<int> factory = null)
        {
            if (factory == null) factory = new TaskFactory<int>();

            var cn = new SqlConnection(this.connectionString);
            var cmd = query.ConstructCommand(cn);
            cn.Open();

            return factory.FromAsync(
                cmd.BeginExecuteNonQuery(),
                ar =>
                {
                    try
                    {
                        int rc = cmd.EndExecuteNonQuery(ar);

                        return rc;
                    }
                    finally
                    {
                        cn.Close();
                    }
                }
            );
        }
    }
}