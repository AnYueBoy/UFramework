﻿using System;
using System.Reflection;

namespace UFramework
{
    public interface IContainer
    {
        /// <summary>
        /// 通过索引器直接从容器中获取一个服务实例
        /// </summary>
        object this[string service] { get; set; }

        /// <summary>
        /// 注册绑定到容器中
        /// </summary>
        /// <param name="service">服务名称</param>
        /// <param name="concrete">服务类型</param>
        /// <param name="isStatic">是否静态</param>
        IBindData Bind(string service, Type concrete, bool isStatic);

        /// <summary>
        /// 注册绑定到容器中
        /// </summary>
        /// <param name="service">服务名称</param>
        /// <param name="concrete">构建对应实例的闭包</param>
        /// <param name="isStatic">是否静态</param>
        IBindData Bind(string service, Func<IContainer, object[], object> concrete, bool isStatic);

        /// <summary>
        /// 注册方法到容器中
        /// </summary>
        /// <param name="method">方法名</param>
        /// <param name="target">唤起对象</param>
        /// <param name="called">唤起对象调用的方法信息</param>
        IMethodBind BindMethod(string method, object target, MethodInfo called);

        /// <summary>
        /// 从容器中解绑方法
        /// </summary>
        void UnbindMethod(object target);

        /// <summary>
        /// 从容器中解绑服务
        /// </summary>
        void Unbind(string service);

        /// <summary>
        /// 为给定的一组服务指定Tag
        /// </summary>
        void Tag(string tag, params string[] services);

        /// <summary>
        /// 通过Tag解析Tag所对应的一组绑定数据
        /// </summary>
        object[] Tagged(string tag);

        /// <summary>
        /// 将现有实例注册为容器中的共享实例。
        /// </summary>
        /// <returns>装饰器处理后的新实例</returns>
        object Instance(string service, object instance);

        /// <summary>
        /// 释放容器中存在的一个实例
        /// </summary>
        /// <param name="mixed">服务名称或别名或实例</param>
        /// <returns>如果已存在的实例已被释放则返回True</returns>
        bool Release(object mixed);

        /// <summary>
        /// 清空容器中所有绑定和已解析实例。
        /// </summary>
        void Flush();

        /// <summary>
        /// 调用容器中的绑定的方法并向方法中注入依赖项
        /// </summary>
        /// <param name="method">方法名称</param>
        /// <param name="userParams">方法参数</param>
        /// <returns>返回方法值</returns>
        object Invoke(string method, params object[] userParams);

        /// <summary>
        /// 调用给定的方法并注入其依赖项。
        /// </summary>
        /// <param name="target">调用其方法的实例</param>
        /// <param name="methodInfo">方法信息</param>
        /// <param name="userParams">方法参数（依赖项）</param>
        object Call(object target, MethodInfo methodInfo, params object[] userParams);

        /// <summary>
        /// 根据服务名或别名从容器中解析实例
        /// </summary>
        /// <returns>服务实例。 如果服务无法解析则抛出异常</returns>
        object Make(string service, params object[] userParams);

        /// <summary>
        /// 将类型转为服务名
        /// </summary>
        string Type2Service(Type type);
    }
}