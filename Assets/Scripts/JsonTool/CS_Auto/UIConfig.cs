//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PRO.Data
{
    
    
    public class UIConfig
    {
        
        private string name;
        
        private bool iscoexist;
        
        private bool useobjecttool;
        
        private bool showmode;
        
        private string loadmode;
        
        private string path;
        
        // UI名称
        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }
        
        // 是否允许同时存在多个
        public bool IsCoexist
        {
            get
            {
                return this.iscoexist;
            }
            set
            {
                this.iscoexist = value;
            }
        }
        
        // 是否使用对象池
        public bool UseObjectTool
        {
            get
            {
                return this.useobjecttool;
            }
            set
            {
                this.useobjecttool = value;
            }
        }
        
        // 是否自动反切
        public bool ShowMode
        {
            get
            {
                return this.showmode;
            }
            set
            {
                this.showmode = value;
            }
        }
        
        // 加载模式
        public string LoadMode
        {
            get
            {
                return this.loadmode;
            }
            set
            {
                this.loadmode = value;
            }
        }
        
        // UI路径
        public string Path
        {
            get
            {
                return this.path;
            }
            set
            {
                this.path = value;
            }
        }
    }
}
