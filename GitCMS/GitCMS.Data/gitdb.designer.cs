﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.235
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GitCMS.Data
{
	using System.Data.Linq;
	using System.Data.Linq.Mapping;
	using System.Data;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Linq;
	using System.Linq.Expressions;
	using System.ComponentModel;
	using System;
	
	
	[global::System.Data.Linq.Mapping.DatabaseAttribute(Name="GitCMS")]
	public sealed partial class GitDataContext : System.Data.Linq.DataContext
	{
		
		private static System.Data.Linq.Mapping.MappingSource mappingSource = new AttributeMappingSource();
		
    #region Extensibility Method Definitions
    partial void OnCreated();
    partial void InsertBlob(Blob instance);
    partial void UpdateBlob(Blob instance);
    partial void DeleteBlob(Blob instance);
    partial void InsertCommit(Commit instance);
    partial void UpdateCommit(Commit instance);
    partial void DeleteCommit(Commit instance);
    partial void InsertTree(Tree instance);
    partial void UpdateTree(Tree instance);
    partial void DeleteTree(Tree instance);
    #endregion
		
		public GitDataContext() : 
				base(global::GitCMS.Data.Properties.Settings.Default.GitCMSConnectionString, mappingSource)
		{
			OnCreated();
		}
		
		public GitDataContext(string connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public GitDataContext(System.Data.IDbConnection connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public GitDataContext(string connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public GitDataContext(System.Data.IDbConnection connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public System.Data.Linq.Table<Blob> Blob
		{
			get
			{
				return this.GetTable<Blob>();
			}
		}
		
		public System.Data.Linq.Table<TreeTree> TreeTree
		{
			get
			{
				return this.GetTable<TreeTree>();
			}
		}
		
		public System.Data.Linq.Table<Commit> Commit
		{
			get
			{
				return this.GetTable<Commit>();
			}
		}
		
		public System.Data.Linq.Table<CommitParent> CommitParent
		{
			get
			{
				return this.GetTable<CommitParent>();
			}
		}
		
		public System.Data.Linq.Table<Tree> Tree
		{
			get
			{
				return this.GetTable<Tree>();
			}
		}
		
		public System.Data.Linq.Table<TreeBlob> TreeBlob
		{
			get
			{
				return this.GetTable<TreeBlob>();
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.Blob")]
	public partial class Blob : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private System.Data.Linq.Binary _blobid;
		
		private System.Data.Linq.Binary _contents;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnblobidChanging(System.Data.Linq.Binary value);
    partial void OnblobidChanged();
    partial void OncontentsChanging(System.Data.Linq.Binary value);
    partial void OncontentsChanged();
    #endregion
		
		public Blob()
		{
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_blobid", DbType="Binary(20) NOT NULL", CanBeNull=false, IsPrimaryKey=true, UpdateCheck=UpdateCheck.Never)]
		public System.Data.Linq.Binary blobid
		{
			get
			{
				return this._blobid;
			}
			set
			{
				if ((this._blobid != value))
				{
					this.OnblobidChanging(value);
					this.SendPropertyChanging();
					this._blobid = value;
					this.SendPropertyChanged("blobid");
					this.OnblobidChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_contents", DbType="VarBinary(MAX) NOT NULL", CanBeNull=false, UpdateCheck=UpdateCheck.Never)]
		public System.Data.Linq.Binary contents
		{
			get
			{
				return this._contents;
			}
			set
			{
				if ((this._contents != value))
				{
					this.OncontentsChanging(value);
					this.SendPropertyChanging();
					this._contents = value;
					this.SendPropertyChanged("contents");
					this.OncontentsChanged();
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.TreeTree")]
	public partial class TreeTree
	{
		
		private System.Data.Linq.Binary _treeid;
		
		private System.Data.Linq.Binary _linked_treeid;
		
		private string _name;
		
		public TreeTree()
		{
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_treeid", DbType="Binary(20) NOT NULL", CanBeNull=false, UpdateCheck=UpdateCheck.Never)]
		public System.Data.Linq.Binary treeid
		{
			get
			{
				return this._treeid;
			}
			set
			{
				if ((this._treeid != value))
				{
					this._treeid = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_linked_treeid", DbType="Binary(20) NOT NULL", CanBeNull=false, UpdateCheck=UpdateCheck.Never)]
		public System.Data.Linq.Binary linked_treeid
		{
			get
			{
				return this._linked_treeid;
			}
			set
			{
				if ((this._linked_treeid != value))
				{
					this._linked_treeid = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_name", DbType="NVarChar(512) NOT NULL", CanBeNull=false)]
		public string name
		{
			get
			{
				return this._name;
			}
			set
			{
				if ((this._name != value))
				{
					this._name = value;
				}
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.[Commit]")]
	public partial class Commit : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private System.Data.Linq.Binary _commitid;
		
		private System.Data.Linq.Binary _treeid;
		
		private string _committer;
		
		private string _author;
		
		private System.DateTime _date_committed;
		
		private string _message;
		
		private EntityRef<Tree> _Tree;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OncommitidChanging(System.Data.Linq.Binary value);
    partial void OncommitidChanged();
    partial void OntreeidChanging(System.Data.Linq.Binary value);
    partial void OntreeidChanged();
    partial void OncommitterChanging(string value);
    partial void OncommitterChanged();
    partial void OnauthorChanging(string value);
    partial void OnauthorChanged();
    partial void Ondate_committedChanging(System.DateTime value);
    partial void Ondate_committedChanged();
    partial void OnmessageChanging(string value);
    partial void OnmessageChanged();
    #endregion
		
		public Commit()
		{
			this._Tree = default(EntityRef<Tree>);
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_commitid", DbType="Binary(20) NOT NULL", CanBeNull=false, IsPrimaryKey=true, UpdateCheck=UpdateCheck.Never)]
		public System.Data.Linq.Binary commitid
		{
			get
			{
				return this._commitid;
			}
			set
			{
				if ((this._commitid != value))
				{
					this.OncommitidChanging(value);
					this.SendPropertyChanging();
					this._commitid = value;
					this.SendPropertyChanged("commitid");
					this.OncommitidChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_treeid", DbType="Binary(20) NOT NULL", CanBeNull=false, UpdateCheck=UpdateCheck.Never)]
		public System.Data.Linq.Binary treeid
		{
			get
			{
				return this._treeid;
			}
			set
			{
				if ((this._treeid != value))
				{
					if (this._Tree.HasLoadedOrAssignedValue)
					{
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
					}
					this.OntreeidChanging(value);
					this.SendPropertyChanging();
					this._treeid = value;
					this.SendPropertyChanged("treeid");
					this.OntreeidChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_committer", DbType="NVarChar(512) NOT NULL", CanBeNull=false)]
		public string committer
		{
			get
			{
				return this._committer;
			}
			set
			{
				if ((this._committer != value))
				{
					this.OncommitterChanging(value);
					this.SendPropertyChanging();
					this._committer = value;
					this.SendPropertyChanged("committer");
					this.OncommitterChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_author", DbType="NVarChar(512)")]
		public string author
		{
			get
			{
				return this._author;
			}
			set
			{
				if ((this._author != value))
				{
					this.OnauthorChanging(value);
					this.SendPropertyChanging();
					this._author = value;
					this.SendPropertyChanged("author");
					this.OnauthorChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_date_committed", DbType="DateTime2 NOT NULL")]
		public System.DateTime date_committed
		{
			get
			{
				return this._date_committed;
			}
			set
			{
				if ((this._date_committed != value))
				{
					this.Ondate_committedChanging(value);
					this.SendPropertyChanging();
					this._date_committed = value;
					this.SendPropertyChanged("date_committed");
					this.Ondate_committedChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_message", DbType="NVarChar(MAX) NOT NULL", CanBeNull=false)]
		public string message
		{
			get
			{
				return this._message;
			}
			set
			{
				if ((this._message != value))
				{
					this.OnmessageChanging(value);
					this.SendPropertyChanging();
					this._message = value;
					this.SendPropertyChanged("message");
					this.OnmessageChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="Tree_Commit", Storage="_Tree", ThisKey="treeid", OtherKey="treeid", IsForeignKey=true)]
		public Tree Tree
		{
			get
			{
				return this._Tree.Entity;
			}
			set
			{
				Tree previousValue = this._Tree.Entity;
				if (((previousValue != value) 
							|| (this._Tree.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if ((previousValue != null))
					{
						this._Tree.Entity = null;
						previousValue.Commits.Remove(this);
					}
					this._Tree.Entity = value;
					if ((value != null))
					{
						value.Commits.Add(this);
						this._treeid = value.treeid;
					}
					else
					{
						this._treeid = default(System.Data.Linq.Binary);
					}
					this.SendPropertyChanged("Tree");
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.CommitParent")]
	public partial class CommitParent
	{
		
		private System.Data.Linq.Binary _commitid;
		
		private System.Data.Linq.Binary _parent_commitid;
		
		private int _ordinal;
		
		public CommitParent()
		{
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_commitid", DbType="Binary(20) NOT NULL", CanBeNull=false, UpdateCheck=UpdateCheck.Never)]
		public System.Data.Linq.Binary commitid
		{
			get
			{
				return this._commitid;
			}
			set
			{
				if ((this._commitid != value))
				{
					this._commitid = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_parent_commitid", DbType="Binary(20) NOT NULL", CanBeNull=false, UpdateCheck=UpdateCheck.Never)]
		public System.Data.Linq.Binary parent_commitid
		{
			get
			{
				return this._parent_commitid;
			}
			set
			{
				if ((this._parent_commitid != value))
				{
					this._parent_commitid = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ordinal", DbType="Int NOT NULL")]
		public int ordinal
		{
			get
			{
				return this._ordinal;
			}
			set
			{
				if ((this._ordinal != value))
				{
					this._ordinal = value;
				}
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.Tree")]
	public partial class Tree : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private System.Data.Linq.Binary _treeid;
		
		private EntitySet<Commit> _Commits;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OntreeidChanging(System.Data.Linq.Binary value);
    partial void OntreeidChanged();
    #endregion
		
		public Tree()
		{
			this._Commits = new EntitySet<Commit>(new Action<Commit>(this.attach_Commits), new Action<Commit>(this.detach_Commits));
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_treeid", DbType="Binary(20) NOT NULL", CanBeNull=false, IsPrimaryKey=true, UpdateCheck=UpdateCheck.Never)]
		public System.Data.Linq.Binary treeid
		{
			get
			{
				return this._treeid;
			}
			set
			{
				if ((this._treeid != value))
				{
					this.OntreeidChanging(value);
					this.SendPropertyChanging();
					this._treeid = value;
					this.SendPropertyChanged("treeid");
					this.OntreeidChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="Tree_Commit", Storage="_Commits", ThisKey="treeid", OtherKey="treeid")]
		public EntitySet<Commit> Commits
		{
			get
			{
				return this._Commits;
			}
			set
			{
				this._Commits.Assign(value);
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
		
		private void attach_Commits(Commit entity)
		{
			this.SendPropertyChanging();
			entity.Tree = this;
		}
		
		private void detach_Commits(Commit entity)
		{
			this.SendPropertyChanging();
			entity.Tree = null;
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.TreeBlob")]
	public partial class TreeBlob
	{
		
		private System.Data.Linq.Binary _treeid;
		
		private System.Data.Linq.Binary _linked_blobid;
		
		private string _name;
		
		public TreeBlob()
		{
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_treeid", DbType="Binary(20) NOT NULL", CanBeNull=false, UpdateCheck=UpdateCheck.Never)]
		public System.Data.Linq.Binary treeid
		{
			get
			{
				return this._treeid;
			}
			set
			{
				if ((this._treeid != value))
				{
					this._treeid = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_linked_blobid", DbType="Binary(20) NOT NULL", CanBeNull=false, UpdateCheck=UpdateCheck.Never)]
		public System.Data.Linq.Binary linked_blobid
		{
			get
			{
				return this._linked_blobid;
			}
			set
			{
				if ((this._linked_blobid != value))
				{
					this._linked_blobid = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_name", DbType="NVarChar(512) NOT NULL", CanBeNull=false)]
		public string name
		{
			get
			{
				return this._name;
			}
			set
			{
				if ((this._name != value))
				{
					this._name = value;
				}
			}
		}
	}
}
#pragma warning restore 1591
