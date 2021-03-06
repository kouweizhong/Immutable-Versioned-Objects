﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace IVO.Definition.Models
{
    /// <summary>
    /// Represents an ordered response to a query.
    /// </summary>
    /// <typeparam name="Telement">The type of elements in the collection</typeparam>
    /// <typeparam name="TorderBy">The enum type that defines what columns may be ordered on</typeparam>
    public sealed class OrderedFullQueryResponse<Tquery, Telement, TorderBy>
        where Tquery : class
        where Telement : class
        where TorderBy : struct
    {
        public OrderedFullQueryResponse(Tquery query, ReadOnlyCollection<Telement> collection, ReadOnlyCollection<OrderByApplication<TorderBy>> orderedBy)
        {
            // Why would you pass nulls? What the hell is wrong with you?
            if (query == null) throw new ArgumentNullException("query");
            if (collection == null) throw new ArgumentNullException("collection");
            if (orderedBy == null) throw new ArgumentNullException("orderedBy");

            this.Query = query;
            this.Collection = collection;
            this.OrderedBy = orderedBy;
        }

        public Tquery Query { get; private set; }
        public ReadOnlyCollection<Telement> Collection { get; private set; }
        public ReadOnlyCollection<OrderByApplication<TorderBy>> OrderedBy { get; private set; }
    }
}
