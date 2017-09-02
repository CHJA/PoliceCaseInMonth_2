using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HighResolutionApps.Common;
using HighResolutionApps.Interfaces;
using HighResolutionApps.Interfaces.Plugins;

namespace HighResolutionApps.VisualControls.PoliceCaseInMonth_2
{
    /// <summary>
    /// 组件组类
    /// </summary>
    public class Plugin : IVisualControlsPlug
    {
        #region IVisualControlsPlug Members
        /// <summary>
        /// 当前插件内包含的所有组件
        /// </summary>
        List<IVisualControlDescriptor> controls = new List<IVisualControlDescriptor>();

        /// <summary>
        /// 插件名称，理解为分类名称 需要修改
        /// </summary>
        public string Name
        {
            get { return "株洲公安"; }
        }

        /// <summary>
        /// 接口实现
        /// </summary>
        public IVisualControlDescriptor[] Controls
        {
            get { return controls.ToArray(); }
        }

        /// <summary>
        /// 插件ID，必须为唯一而且命名要有一定意义，便于脚本编写
        /// 需要修改
        /// </summary>
        public string PluginId
        {
            get { return "demo_controls_plug"; }
        }

        /// <summary>
        /// 插件初始化 
        /// 需要修改
        /// </summary>
        public void Initialize()
        {
            //自定义组件增加
            HRVUserControlWrappers.Plugin = this;
            this.controls.Add(new HRVUserControlWrappers());

        }
        /// <summary>
        /// 注销事件
        /// </summary>
        public void DeInitialize()
        {

        }

        #endregion




    }
}
