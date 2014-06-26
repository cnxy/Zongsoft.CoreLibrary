﻿/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2003-2013 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.IO;
using System.ComponentModel;
using System.Collections.Generic;
using System.Security.Principal;

namespace Zongsoft.ComponentModel
{
	public class ApplicationContextBase : MarshalByRefObject, INotifyPropertyChanged
	{
		#region 事件声明
		public event CancelEventHandler Exiting;
		public event EventHandler<ApplicationEventArgs> Starting;
		public event EventHandler<ApplicationEventArgs> Started;
		public event PropertyChangedEventHandler PropertyChanged;
		#endregion

		#region 同步变量
		protected readonly object SyncRoot = new object();
		#endregion

		#region 成员字段
		private string _applicationId;
		private string _title;
		private string _description;
		private IPrincipal _user;
		private Options.ISettingProvider _settings;
		private Options.OptionManager _optionManager;
		private Options.Configuration.OptionConfiguration _configuration;
		#endregion

		#region 构造函数
		protected ApplicationContextBase()
		{
		}

		protected ApplicationContextBase(string applicationId)
		{
			if(string.IsNullOrWhiteSpace(applicationId))
				throw new ArgumentNullException("applicationId");

			//初始化属性变量
			_applicationId = applicationId;
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取或设置当前应用程序唯一代号。
		/// </summary>
		/// <remarks>
		///		<para>注意：本属性一旦被设置则不能被更改。如果已经设置过本属性(即该属性值不为空或全空格字符)，则不能再次设置本属性否则将抛出<see cref="InvalidOperationException"/>异常。</para>
		/// </remarks>
		public string ApplicationId
		{
			get
			{
				return _applicationId;
			}
			set
			{
				if(string.IsNullOrWhiteSpace(value))
					throw new ArgumentNullException();

				if(!string.IsNullOrWhiteSpace(_applicationId))
					throw new InvalidOperationException("The ApplicationId has specified already.");

				if(string.Equals(_applicationId, value.Trim(), StringComparison.Ordinal))
					return;

				//设置对应的成员字段值
				_applicationId = value.Trim();

				//激发“PropertyChanged”事件
				this.OnPropertyChanged("ApplicationId");
			}
		}

		/// <summary>
		/// 获取当前应用程序的根目录。
		/// </summary>
		public virtual string ApplicationDirectory
		{
			get
			{
				return AppDomain.CurrentDomain.BaseDirectory;
			}
		}

		/// <summary>
		/// 获取或设置当前应用程序的标题。
		/// </summary>
		public string Title
		{
			get
			{
				return _title;
			}
			set
			{
				if(string.Equals(_title, value, StringComparison.Ordinal))
					return;

				//设置成员变量
				_title = value ?? string.Empty;

				//激发“PropertyChanged”事件
				this.OnPropertyChanged("Title");
			}
		}

		/// <summary>
		/// 获取或设置当前应用程序的描述文本。
		/// </summary>
		public string Description
		{
			get
			{
				return _description;
			}
			set
			{
				if(string.Equals(_description, value, StringComparison.Ordinal))
					return;

				//设置成员变量
				_description = value ?? string.Empty;

				//激发“PropertyChanged”事件
				this.OnPropertyChanged("Description");
			}
		}

		/// <summary>
		/// 获取当前应用程序的自定义设置提供程序。
		/// </summary>
		public Options.ISettingProvider Settings
		{
			get
			{
				if(_settings == null)
					System.Threading.Interlocked.CompareExchange(ref _settings, this.OptionManager.Settings, null);

				return _settings;
			}
			protected set
			{
				_settings = value;
			}
		}

		/// <summary>
		/// 获取当前应用程序的选项管理。
		/// </summary>
		public Options.OptionManager OptionManager
		{
			get
			{
				if(_optionManager == null)
					System.Threading.Interlocked.CompareExchange(ref _optionManager, new Options.OptionManager(), null);

				return _optionManager;
			}
		}

		/// <summary>
		/// 获取或设置当前应用程序的默认选项配置。
		/// </summary>
		public Options.Configuration.OptionConfiguration Configuration
		{
			get
			{
				return _configuration;
			}
			protected set
			{
				if(value == null)
					throw new ArgumentNullException();

				_configuration = value;
			}
		}

		/// <summary>
		/// 获取当前应用程序上下文的跟踪器对象。
		/// </summary>
		public virtual Zongsoft.Diagnostics.Tracer Tracer
		{
			get
			{
				return Zongsoft.Diagnostics.Tracer.Default;
			}
		}

		/// <summary>
		/// 获取当前应用程序的服务管理对象。
		/// </summary>
		public virtual Zongsoft.Services.IServiceProviderFactory ServiceFactory
		{
			get
			{
				return null;
			}
		}

		/// <summary>
		/// 获取或设置当前应用程序的安全主体。
		/// </summary>
		public virtual IPrincipal User
		{
			get
			{
				return _user;
			}
			set
			{
				if(_user != value)
				{
					_user = value;

					//激发“PropertyChanged”事件
					this.OnPropertyChanged("Principal");
				}
			}
		}
		#endregion

		#region 公共方法
		/// <summary>
		/// 确认指定的当前应用程序的相对目录是否存在，如果不存在则依次创建它们，并返回其对应的完整路径。
		/// </summary>
		/// <param name="relativePath">相对于应用程序根目录的相对路径，可使用'/'或'\'字符作为相对路径的分隔符。</param>
		/// <returns>如果<paramref name="relativePath"/>参数为空或者全空白字符则返回应用程序根目录(即<see cref="ApplicationDirectory"/>属性值。)，否则返回其相对路径的完整路径。</returns>
		public string EnsureDirectory(string relativePath)
		{
			string fullPath = this.ApplicationDirectory;

			if(string.IsNullOrWhiteSpace(relativePath))
				return fullPath;

			var parts = relativePath.Split('/', '\\', Path.DirectorySeparatorChar);

			foreach(var part in parts)
			{
				if(string.IsNullOrWhiteSpace(part))
					continue;

				if(!Directory.Exists(Path.Combine(fullPath, part)))
					Directory.CreateDirectory(Path.Combine(fullPath, part));

				fullPath = Path.Combine(fullPath, part);
			}

			return fullPath;
		}
		#endregion

		#region 激发事件
		protected void OnPropertyChanged(string propertyName)
		{
			if(string.IsNullOrEmpty(propertyName))
				throw new ArgumentNullException("propertyName");

			this.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
		}

		protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
		{
			if(PropertyChanged != null)
				PropertyChanged(this, e);
		}

		protected virtual void OnExiting(CancelEventArgs args)
		{
			if(Exiting != null)
				Exiting(null, args);
		}

		protected virtual void OnStarting(ApplicationEventArgs args)
		{
			if(Starting != null)
				Starting(null, args);
		}

		protected virtual void OnStarted(ApplicationEventArgs args)
		{
			if(Started != null)
				Started(null, args);
		}
		#endregion
	}
}
