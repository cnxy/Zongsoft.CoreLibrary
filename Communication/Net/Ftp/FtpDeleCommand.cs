﻿#region File Header

// Authors:
//    钟峰(Popeye Zhong) <zongsoft@gmail.com>
//    邓祥云(X.Z. Deng) <627825056@qq.com>
//  
// Copyright (C) 2011-2013 Zongsoft Corporation <http://www.zongsoft.com>
// 
// This file is part of Zongsoft.CoreLibrary.
// 
// Zongsoft.CoreLibrary is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// Zongsoft.CoreLibrary is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with Zongsoft.CoreLibrary; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA

#endregion

#region Using

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

#endregion

namespace Zongsoft.Communication.Net.Ftp
{
    /// <summary>
    /// 删除服务器上指定的文件
    /// </summary>
    internal class FtpDeleCommand : FtpCommand
    {
        public FtpDeleCommand() : base("DELE")
        {
        }

        protected override void Run(FtpCommandContext context)
        {
            context.Channel.CheckLogin();

            if (string.IsNullOrEmpty(context.Statement.Argument))
                throw new SyntaxException();

            var path = context.Statement.Argument;
            var localPath = context.Channel.MapVirtualPathToLocalPath(path);
            context.Statement.Result = localPath;

            if (Directory.Exists(localPath))
                throw new FileNotFoundException(path);

            try
            {
                File.Delete(localPath);
            }
            catch (Exception)
            {
                throw new InternalException("delete file");
            }

            context.Channel.Send("250 Deleted file successfully");
        }
    }
}