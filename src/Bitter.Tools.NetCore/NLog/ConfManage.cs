﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;

namespace Bitter.Tools.NetCore
{
    internal class ConfManage
    {
        // 定义一个标识确保线程同步
        private static readonly object locker = new object();
        private static ConfManage _configManage;

        private string basejsonFileName = "appsettings.json";

        private IConfigurationRoot configOperat;
        private ConfManage()
        {

        }

        public static ConfManage GetInstance()
        {
            lock (locker)//确保线程安全
            {
                if (_configManage == null)
                {

                    _configManage = new ConfManage();
                    _configManage.configOperat = new ConfigurationBuilder()
                    .AddInMemoryCollection()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile(_configManage.basejsonFileName, optional: true, reloadOnChange: true)
                    .Build();
                }
                return _configManage;
            }
        }

        public T Settings<T>(string key) where T : class, new()
        {

            var appconfig = new ServiceCollection()
            .AddOptions()
            .Configure<T>(_configManage.configOperat.GetSection(key))
            .BuildServiceProvider()
            .GetService<IOptions<T>>()
            .Value;
            return appconfig;
        }
        /// <summary>
        /// AppSettings配置信息
        /// </summary>
        public NameValueCollection AppSettings
        {
            get
            {
                var list = _configManage.configOperat.GetSection("AppSettings").GetChildren().ToList();
                NameValueCollection dc = new NameValueCollection();
                foreach (var item in list)
                {
                    dc.Add(item.Key, item.Value);
                }
                return dc;
            }
        }
       

       
    }
}
