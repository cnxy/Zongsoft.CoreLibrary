﻿/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2010-2017 Zongsoft Corporation <http://www.zongsoft.com>
 *
 * This file is part of Zongsoft.CoreLibrary.
 *
 * Zongsoft.CoreLibrary is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * Zongsoft.CoreLibrary is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
 * Lesser General Public License for more details.
 *
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with Zongsoft.CoreLibrary; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA
 */

using System;
using System.Collections;
using System.Collections.Generic;

namespace Zongsoft.Data
{
	public class DataCountContext : DataAccessContextBase
	{
		#region 成员字段
		private int _result;
		private ICondition _condition;
		private string _includes;
		#endregion

		#region 构造函数
		public DataCountContext(IDataAccess dataAccess, string name, ICondition condition, string includes, int result = 0) : base(dataAccess, name, DataAccessMethod.Count)
		{
			_condition = condition;
			_includes = includes;
			_result = result;
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取或设置计数操作的结果。
		/// </summary>
		public int Result
		{
			get
			{
				return _result;
			}
			set
			{
				_result = value;
			}
		}

		/// <summary>
		/// 获取或设置计数操作的条件。
		/// </summary>
		public ICondition Condition
		{
			get
			{
				return _condition;
			}
			set
			{
				_condition = value;
			}
		}

		/// <summary>
		/// 获取或设置计数操作的包含成员。
		/// </summary>
		public string Includes
		{
			get
			{
				return _includes;
			}
			set
			{
				_includes = value;
			}
		}
		#endregion
	}

	public class DataExistenceContext : DataAccessContextBase
	{
		#region 成员字段
		private ICondition _condition;
		private bool _result;
		#endregion

		#region 构造函数
		public DataExistenceContext(IDataAccess dataAccess, string name, ICondition condition, bool result = false) : base(dataAccess, name, DataAccessMethod.Exists)
		{
			_condition = condition;
			_result = result;
		}
		#endregion

		#region 公共属性
		public ICondition Condition
		{
			get
			{
				return _condition;
			}
			set
			{
				_condition = value;
			}
		}

		public bool Result
		{
			get
			{
				return _result;
			}
			set
			{
				_result = value;
			}
		}
		#endregion
	}

	public class DataExecutionContext : DataAccessContextBase
	{
		#region 成员字段
		private object _result;
		private Type _resultType;
		private IDictionary<string, object> _inParameters;
		private IDictionary<string, object> _outParameters;
		#endregion

		#region 构造函数
		public DataExecutionContext(IDataAccess dataAccess, string name, Type resultType, IDictionary<string, object> inParameters, IDictionary<string, object> outParameters, object result = null) : base(dataAccess, name, DataAccessMethod.Execute)
		{
			if(resultType == null)
				throw new ArgumentNullException("resultType");

			_result = result;
			_resultType = resultType;
			_inParameters = inParameters;
			_outParameters = outParameters;
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取一个值，指示当前是否为单值执行方法。
		/// </summary>
		public bool IsScalar
		{
			get
			{
				return !(_resultType != null && Zongsoft.Common.TypeExtension.IsAssignableFrom(typeof(IEnumerable<>), _resultType));
			}
		}

		/// <summary>
		/// 获取查询结果集的实体类型，如果是单值执行方法则返回空(null)。
		/// </summary>
		public Type EntityType
		{
			get
			{
				if(_resultType != null && Zongsoft.Common.TypeExtension.IsAssignableFrom(typeof(IEnumerable<>), _resultType))
					return _resultType.GetGenericArguments()[0];

				return null;
			}
		}

		/// <summary>
		/// 获取或设置执行结果的类型。
		/// </summary>
		public Type ResultType
		{
			get
			{
				return _resultType;
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException();

				_resultType = value;
			}
		}

		/// <summary>
		/// 获取或设置执行操作的结果。
		/// </summary>
		public object Result
		{
			get
			{
				return _result;
			}
			set
			{
				_result = value;
			}
		}

		/// <summary>
		/// 获取执行操作的输入参数。
		/// </summary>
		public IDictionary<string, object> InParameters
		{
			get
			{
				return _inParameters;
			}
		}

		/// <summary>
		/// 获取执行操作的输出参数。
		/// </summary>
		public IDictionary<string, object> OutParameters
		{
			get
			{
				return _outParameters;
			}
		}
		#endregion
	}

	public class DataIncrementContext : DataAccessContextBase
	{
		#region 成员字段
		private string _member;
		private ICondition _condition;
		private int _interval;
		private long _result;
		#endregion

		#region 构造函数
		public DataIncrementContext(IDataAccess dataAccess, string name, string member, ICondition condition, int interval = 1, long result = 0) : base(dataAccess, name, DataAccessMethod.Increment)
		{
			if(string.IsNullOrEmpty(member))
				throw new ArgumentNullException(nameof(member));

			if(condition == null)
				throw new ArgumentNullException(nameof(condition));

			_member = member;
			_condition = condition;
			_interval = interval;
			_result = result;
		}
		#endregion

		#region 公共属性
		public string Member
		{
			get
			{
				return _member;
			}
			set
			{
				if(string.IsNullOrWhiteSpace(value))
					throw new ArgumentNullException();

				_member = value;
			}
		}

		public ICondition Condition
		{
			get
			{
				return _condition;
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException();

				_condition = value;
			}
		}

		public int Interval
		{
			get
			{
				return _interval;
			}
			set
			{
				_interval = value;
			}
		}

		public long Result
		{
			get
			{
				return _result;
			}
			set
			{
				_result = value;
			}
		}
		#endregion
	}

	public class DataDecrementContext : DataAccessContextBase
	{
		#region 成员字段
		private string _member;
		private ICondition _condition;
		private int _interval;
		private long _result;
		#endregion

		#region 构造函数
		public DataDecrementContext(IDataAccess dataAccess, string name, string member, ICondition condition, int interval = 1, long result = 0) : base(dataAccess, name, DataAccessMethod.Decrement)
		{
			if(string.IsNullOrEmpty(member))
				throw new ArgumentNullException(nameof(member));

			if(condition == null)
				throw new ArgumentNullException(nameof(condition));

			_member = member;
			_condition = condition;
			_interval = interval;
			_result = result;
		}
		#endregion

		#region 公共属性
		public string Member
		{
			get
			{
				return _member;
			}
			set
			{
				if(string.IsNullOrWhiteSpace(value))
					throw new ArgumentNullException();

				_member = value;
			}
		}

		public ICondition Condition
		{
			get
			{
				return _condition;
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException();

				_condition = value;
			}
		}

		public int Interval
		{
			get
			{
				return _interval;
			}
			set
			{
				_interval = value;
			}
		}

		public long Result
		{
			get
			{
				return _result;
			}
			set
			{
				_result = value;
			}
		}
		#endregion
	}

	public class DataSelectionContext : DataAccessContextBase
	{
		#region 成员字段
		private Type _entityType;
		private IEnumerable _result;
		private ICondition _condition;
		private string _scope;
		private Paging _paging;
		private Grouping _grouping;
		private Sorting[] _sortings;
		#endregion

		#region 构造函数
		public DataSelectionContext(IDataAccess dataAccess, string name, Type entityType, ICondition condition, Grouping grouping, string scope, Paging paging, Sorting[] sortings, IEnumerable result = null) : base(dataAccess, name, DataAccessMethod.Select)
		{
			if(entityType == null)
				throw new ArgumentNullException(nameof(entityType));

			_entityType = entityType;
			_condition = condition;
			_grouping = grouping;
			_scope = scope;
			_paging = paging;
			_sortings = sortings;
			_result = result;
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取或设置查询结果集的实体类型。
		/// </summary>
		public Type EntityType
		{
			get
			{
				return _entityType;
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException();

				_entityType = value;
			}
		}

		/// <summary>
		/// 获取或设置查询操作的结果。
		/// </summary>
		public IEnumerable Result
		{
			get
			{
				return _result;
			}
			set
			{
				_result = value;
			}
		}

		/// <summary>
		/// 获取或设置查询操作的条件。
		/// </summary>
		public ICondition Condition
		{
			get
			{
				return _condition;
			}
			set
			{
				_condition = value;
			}
		}

		/// <summary>
		/// 获取或设置查询操作的分组。
		/// </summary>
		public Grouping Grouping
		{
			get
			{
				return _grouping;
			}
			set
			{
				_grouping = value;
			}
		}

		/// <summary>
		/// 获取或设置查询操作的分页设置。
		/// </summary>
		public Paging Paging
		{
			get
			{
				return _paging;
			}
			set
			{
				_paging = value;
			}
		}

		/// <summary>
		/// 获取或设置查询操作的包含成员。
		/// </summary>
		public string Scope
		{
			get
			{
				return _scope;
			}
			set
			{
				_scope = value;
			}
		}

		/// <summary>
		/// 获取或设置查询操作的排序设置。
		/// </summary>
		public Sorting[] Sortings
		{
			get
			{
				return _sortings;
			}
			set
			{
				_sortings = value;
			}
		}
		#endregion
	}

	public class DataDeletionContext : DataAccessContextBase
	{
		#region 成员字段
		private int _count;
		private ICondition _condition;
		private string[] _cascades;
		#endregion

		#region 构造函数
		public DataDeletionContext(IDataAccess dataAccess, string name, ICondition condition, string[] cascades, int count) : base(dataAccess, name, DataAccessMethod.Delete)
		{
			_condition = condition;
			_cascades = cascades;
			_count = count;
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取或设置删除操作的受影响记录数。
		/// </summary>
		public int Count
		{
			get
			{
				return _count;
			}
			set
			{
				_count = value;
			}
		}

		/// <summary>
		/// 获取或设置删除操作的条件。
		/// </summary>
		public ICondition Condition
		{
			get
			{
				return _condition;
			}
			set
			{
				_condition = value;
			}
		}

		/// <summary>
		/// 获取或设置删除操作的关联成员。
		/// </summary>
		public string[] Cascades
		{
			get
			{
				return _cascades;
			}
			set
			{
				_cascades = value;
			}
		}
		#endregion
	}

	public class DataInsertionContext : DataAccessContextBase
	{
		#region 成员字段
		private int _count;
		private DataDictionary _data;
		private string _scope;
		#endregion

		#region 构造函数
		public DataInsertionContext(IDataAccess dataAccess, string name, DataDictionary data, string scope, int count = 0) : base(dataAccess, name, DataAccessMethod.Insert)
		{
			_data = data;
			_scope = scope;
			_count = count;
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取或设置插入操作的受影响记录数。
		/// </summary>
		public int Count
		{
			get
			{
				return _count;
			}
			set
			{
				_count = value;
			}
		}

		/// <summary>
		/// 获取或设置插入操作的数据。
		/// </summary>
		public DataDictionary Data
		{
			get
			{
				return _data;
			}
			set
			{
				_data = value;
			}
		}

		/// <summary>
		/// 获取或设置插入操作的包含成员。
		/// </summary>
		public string Scope
		{
			get
			{
				return _scope;
			}
			set
			{
				_scope = value;
			}
		}
		#endregion
	}

	public class DataManyInsertionContext : DataAccessContextBase
	{
		#region 成员字段
		private int _count;
		private IEnumerable<DataDictionary> _data;
		private string _scope;
		#endregion

		#region 构造函数
		public DataManyInsertionContext(IDataAccess dataAccess, string name, IEnumerable<DataDictionary> data, string scope, int count) : base(dataAccess, name, DataAccessMethod.InsertMany)
		{
			_data = data;
			_scope = scope;
			_count = count;
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取或设置插入操作的受影响记录数。
		/// </summary>
		public int Count
		{
			get
			{
				return _count;
			}
			set
			{
				_count = value;
			}
		}

		/// <summary>
		/// 获取或设置插入操作的数据。
		/// </summary>
		public IEnumerable<DataDictionary> Data
		{
			get
			{
				return _data;
			}
			set
			{
				_data = value;
			}
		}

		/// <summary>
		/// 获取或设置插入操作的包含成员。
		/// </summary>
		public string Scope
		{
			get
			{
				return _scope;
			}
			set
			{
				_scope = value;
			}
		}
		#endregion
	}

	public class DataUpdationContext : DataAccessContextBase
	{
		#region 成员字段
		private int _count;
		private DataDictionary _data;
		private ICondition _condition;
		private string _scope;
		#endregion

		#region 构造函数
		public DataUpdationContext(IDataAccess dataAccess, string name, DataDictionary data, ICondition condition, string scope, int count) : base(dataAccess, name, DataAccessMethod.Update)
		{
			_data = data;
			_condition = condition;
			_scope = scope;
			_count = count;
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取或设置更新操作的受影响记录数。
		/// </summary>
		public int Count
		{
			get
			{
				return _count;
			}
			set
			{
				_count = value;
			}
		}

		/// <summary>
		/// 获取或设置更新操作的数据。
		/// </summary>
		public DataDictionary Data
		{
			get
			{
				return _data;
			}
			set
			{
				_data = value;
			}
		}

		/// <summary>
		/// 获取或设置更新操作的条件。
		/// </summary>
		public ICondition Condition
		{
			get
			{
				return _condition;
			}
			set
			{
				_condition = value;
			}
		}

		/// <summary>
		/// 获取或设置更新操作的包含成员。
		/// </summary>
		public string Scope
		{
			get
			{
				return _scope;
			}
			set
			{
				_scope = value;
			}
		}
		#endregion
	}

	public class DataManyUpdationContext : DataAccessContextBase
	{
		#region 成员字段
		private int _count;
		private IEnumerable<DataDictionary> _data;
		private ICondition _condition;
		private string _scope;
		#endregion

		#region 构造函数
		public DataManyUpdationContext(IDataAccess dataAccess, string name, IEnumerable<DataDictionary> data, ICondition condition, string scope, int count) : base(dataAccess, name, DataAccessMethod.UpdateMany)
		{
			_data = data;
			_condition = condition;
			_scope = scope;
			_count = count;
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取或设置更新操作的受影响记录数。
		/// </summary>
		public int Count
		{
			get
			{
				return _count;
			}
			set
			{
				_count = value;
			}
		}

		/// <summary>
		/// 获取或设置更新操作的数据。
		/// </summary>
		public IEnumerable<DataDictionary> Data
		{
			get
			{
				return _data;
			}
			set
			{
				_data = value;
			}
		}

		/// <summary>
		/// 获取或设置更新操作的条件。
		/// </summary>
		public ICondition Condition
		{
			get
			{
				return _condition;
			}
			set
			{
				_condition = value;
			}
		}

		/// <summary>
		/// 获取或设置更新操作的包含成员。
		/// </summary>
		public string Scope
		{
			get
			{
				return _scope;
			}
			set
			{
				_scope = value;
			}
		}
		#endregion
	}
}
