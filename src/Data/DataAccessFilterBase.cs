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
using System.Collections.Generic;

namespace Zongsoft.Data
{
	public abstract class DataAccessFilterBase : IDataAccessFilter, Zongsoft.Services.IPredication<DataAccessContextBase>
	{
		#region 成员字段
		private readonly int _flags;
		private readonly string[] _names;
		#endregion

		#region 构造函数
		protected DataAccessFilterBase(params string[] names)
		{
			_names = names;

			if(_names != null && _names.Length > 0)
				Array.Sort(_names);
		}

		protected DataAccessFilterBase(params Type[] types)
		{
			_names = GetNames(types);
		}

		protected DataAccessFilterBase(DataAccessMethod method, params string[] names)
		{
			_flags = GetFlag(method);
			_names = names;

			if(_names != null && _names.Length > 0)
				Array.Sort(_names);
		}

		protected DataAccessFilterBase(DataAccessMethod method, params Type[] types)
		{
			_flags = GetFlag(method);
			_names = GetNames(types);
		}

		protected DataAccessFilterBase(IEnumerable<DataAccessMethod> methods, params string[] names)
		{
			_flags = GetFlags(methods);
			_names = names;

			if(_names != null && _names.Length > 0)
				Array.Sort(_names);
		}

		protected DataAccessFilterBase(IEnumerable<DataAccessMethod> methods, params Type[] types)
		{
			_flags = GetFlags(methods);
			_names = GetNames(types);
		}
		#endregion

		#region 保护属性
		protected virtual Zongsoft.Security.CredentialPrincipal Principal
		{
			get
			{
				return Zongsoft.ComponentModel.ApplicationContextBase.Current.Principal as Zongsoft.Security.CredentialPrincipal;
			}
		}
		#endregion

		#region 过滤方法
		protected virtual void OnFiltered(DataAccessContextBase context)
		{
			switch(context.Method)
			{
				case DataAccessMethod.Count:
					this.OnCounted((DataCountContext)context);
					break;
				case DataAccessMethod.Execute:
					this.OnExecuted((DataExecuteContext)context);
					break;
				case DataAccessMethod.Exists:
					this.OnExisted((DataExistContext)context);
					break;
				case DataAccessMethod.Increment:
					this.OnIncremented((DataIncrementContext)context);
					break;
				case DataAccessMethod.Select:
					this.OnSelected((DataSelectContext)context);
					break;
				case DataAccessMethod.Delete:
					this.OnDeleted((DataDeleteContext)context);
					break;
				case DataAccessMethod.Insert:
					this.OnInserted((DataInsertContext)context);
					break;
				case DataAccessMethod.Update:
					this.OnUpdated((DataUpdateContext)context);
					break;
				case DataAccessMethod.Upsert:
					this.OnUpserted((DataUpsertContext)context);
					break;
			}
		}

		protected virtual void OnFiltering(DataAccessContextBase context)
		{
			switch(context.Method)
			{
				case DataAccessMethod.Count:
					this.OnCounting((DataCountContext)context);
					break;
				case DataAccessMethod.Execute:
					this.OnExecuting((DataExecuteContext)context);
					break;
				case DataAccessMethod.Exists:
					this.OnExisting((DataExistContext)context);
					break;
				case DataAccessMethod.Increment:
					this.OnIncrementing((DataIncrementContext)context);
					break;
				case DataAccessMethod.Select:
					this.OnSelecting((DataSelectContext)context);
					break;
				case DataAccessMethod.Delete:
					this.OnDeleting((DataDeleteContext)context);
					break;
				case DataAccessMethod.Insert:
					this.OnInserting((DataInsertContext)context);
					break;
				case DataAccessMethod.Update:
					this.OnUpdating((DataUpdateContext)context);
					break;
				case DataAccessMethod.Upsert:
					this.OnUpserting((DataUpsertContext)context);
					break;
			}
		}

		void IDataAccessFilter.OnFiltered(DataAccessContextBase context)
		{
			this.OnFiltered(context);
		}

		void IDataAccessFilter.OnFiltering(DataAccessContextBase context)
		{
			this.OnFiltering(context);
		}
		#endregion

		#region 虚拟方法
		protected virtual void OnCounting(DataCountContext context)
		{
		}

		protected virtual void OnCounted(DataCountContext context)
		{
		}

		protected virtual void OnExecuting(DataExecuteContext context)
		{
		}

		protected virtual void OnExecuted(DataExecuteContext context)
		{
		}

		protected virtual void OnExisting(DataExistContext context)
		{
		}

		protected virtual void OnExisted(DataExistContext context)
		{
		}

		protected virtual void OnIncrementing(DataIncrementContext context)
		{
		}

		protected virtual void OnIncremented(DataIncrementContext context)
		{
		}

		protected virtual void OnSelecting(DataSelectContext context)
		{
		}

		protected virtual void OnSelected(DataSelectContext context)
		{
		}

		protected virtual void OnDeleting(DataDeleteContext context)
		{
		}

		protected virtual void OnDeleted(DataDeleteContext context)
		{
		}

		protected virtual void OnInserting(DataInsertContext context)
		{
		}

		protected virtual void OnInserted(DataInsertContext context)
		{
		}

		protected virtual void OnUpdating(DataUpdateContext context)
		{
		}

		protected virtual void OnUpdated(DataUpdateContext context)
		{
		}

		protected virtual void OnUpserting(DataUpsertContext context)
		{
		}

		protected virtual void OnUpserted(DataUpsertContext context)
		{
		}
		#endregion

		#region 断言方法
		public virtual bool Predicate(DataAccessContextBase context)
		{
			if(_flags != 0)
			{
				var flag = GetFlag(context.Method);

				if((_flags & flag) != flag)
					return false;
			}

			if(_names != null && _names.Length > 0)
				return Array.Exists(_names, name => context.Name.EndsWith(name, StringComparison.OrdinalIgnoreCase));

			return true;
		}

		bool Zongsoft.Services.IPredication.Predicate(object parameter)
		{
			var context = parameter as DataAccessContextBase;

			if(context == null)
				return false;
			else
				return this.Predicate(context);
		}
		#endregion

		#region 静态方法
		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
		private static int GetFlag(DataAccessMethod method)
		{
			return (int)Math.Pow(2, (int)method);
		}

		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
		private static int GetFlags(IEnumerable<DataAccessMethod> methods)
		{
			if(methods == null)
				return 0;

			int flags = 0;

			foreach(var method in methods)
			{
				flags |= (int)Math.Pow(2, (int)method);
			}

			return flags;
		}

		private static string[] GetNames(Type[] types)
		{
			if(types == null || types.Length == 0)
				return null;

			var names = new List<string>(types.Length);

			for(int i = 0; i < types.Length; i++)
			{
				var name = DataAccessNaming.GetName(types[i]);

				if(!string.IsNullOrEmpty(name))
					names.Add(name);
			}

			names.Sort();

			return names.ToArray();
		}
		#endregion
	}
}
