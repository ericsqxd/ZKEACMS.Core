/* http://www.zkea.net/ Copyright 2016 ZKEASOFT http://www.zkea.net/licenses */
using System;
using System.Collections.Generic;
using Easy.Cache;
using Easy.Extend;
using Easy.MetaData;
using Easy.Models;
using ZKEACMS.ExtendField;
using ZKEACMS.Common.Service;
using ZKEACMS.Common.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Easy;

namespace ZKEACMS.Widget
{
    [ViewConfigure(typeof(WidgetBaseMetaData)), Table("CMS_WidgetBase")]
    public class WidgetBase : EditorEntity, IExtendField
    {
        public static Dictionary<string, Type> KnownWidgetModel { get; } = new Dictionary<string, Type>
        {
           { "ZKEACMS,ZKEACMS.Common.Models.CarouselWidget",typeof(CarouselWidget)},
           { "ZKEACMS,ZKEACMS.Common.Models.HtmlWidget",typeof(HtmlWidget)},
           { "ZKEACMS,ZKEACMS.Common.Models.ImageWidget",typeof(ImageWidget)},
           { "ZKEACMS,ZKEACMS.Common.Models.NavigationWidget",typeof(NavigationWidget)},
           { "ZKEACMS,ZKEACMS.Common.Models.ScriptWidget",typeof(ScriptWidget)},
           { "ZKEACMS,ZKEACMS.Common.Models.StyleSheetWidget",typeof(StyleSheetWidget)},
           { "ZKEACMS,ZKEACMS.Common.Models.VideoWidget",typeof(VideoWidget)},
        };
        public static Dictionary<string, Type> KnownWidgetService { get; } = new Dictionary<string, Type>
        {
           { "ZKEACMS,ZKEACMS.Common.Service.CarouselWidgetService",typeof(CarouselWidgetService)},
           { "ZKEACMS,ZKEACMS.Common.Service.HtmlWidgetService",typeof(HtmlWidgetService)},
           { "ZKEACMS,ZKEACMS.Common.Service.ImageWidgetService",typeof(ImageWidgetService)},
           { "ZKEACMS,ZKEACMS.Common.Service.NavigationWidgetService",typeof(NavigationWidgetService)},
           { "ZKEACMS,ZKEACMS.Common.Service.ScriptWidgetService",typeof(ScriptWidgetService)},
           { "ZKEACMS,ZKEACMS.Common.Service.StyleSheetWidgetService",typeof(StyleSheetWidgetService)},
           { "ZKEACMS,ZKEACMS.Common.Service.VideoWidgetService",typeof(VideoWidgetService)},
        };
        [Key]
        public string ID { get; set; }
        public virtual string WidgetName { get; set; }
        public virtual int? Position { get; set; }
        public virtual string LayoutID { get; set; }
        public virtual string PageID { get; set; }
        public virtual string ZoneID { get; set; }
        public virtual bool IsTemplate { get; set; }
        public virtual string Thumbnail { get; set; }
        public virtual bool IsSystem { get; set; }
        public virtual string PartialView { get; set; }
        public virtual string AssemblyName { get; set; }
        public virtual string ServiceTypeName { get; set; }
        public virtual string ViewModelTypeName { get; set; }
        public virtual string FormView { get; set; }

        public virtual string StyleClass { get; set; }
        private string _customClass;
        [NotMapped]
        public string CustomClass
        {
            get
            {
                if (_customClass != null)
                {
                    return _customClass;
                }
                InitStyleClass();
                return _customClass;
            }
        }
        private string _customStyle;
        [NotMapped]
        public string CustomStyle
        {
            get
            {
                if (_customStyle != null)
                {
                    return _customStyle;
                }
                InitStyleClass();
                return _customStyle;
            }
        }
        private void InitStyleClass()
        {
            if (StyleClass.IsNullOrWhiteSpace())
            {
                _customClass = _customStyle = string.Empty;
            }
            else
            {
                _customClass = CustomRegex.StyleRegex.Replace(StyleClass, evaluator =>
                {
                    _customStyle = evaluator.Groups[1].Value;
                    return string.Empty;
                });
            }
        }
        public WidgetPart ToWidgetPart()
        {
            return new WidgetPart
            {
                Widget = this,
                ViewModel = this
            };
        }
        public WidgetPart ToWidgetPart(object viewModel)
        {
            return new WidgetPart
            {
                Widget = this,
                ViewModel = viewModel
            };
        }


        private IWidgetPartDriver _partDriver;
        public IWidgetPartDriver CreateServiceInstance(IServiceProvider serviceProvider)
        {
            string key = $"{AssemblyName},{ServiceTypeName}";
            if (_partDriver == null && KnownWidgetService.ContainsKey(key))
            {
                return _partDriver = serviceProvider.GetService(KnownWidgetService[key]) as IWidgetPartDriver;
            }
            return _partDriver;
        }


        private WidgetBase _widgetBase;
        public WidgetBase CreateViewModelInstance(IServiceProvider serviceProvider)
        {
            string key = $"{AssemblyName},{ViewModelTypeName}";
            if (_widgetBase == null && KnownWidgetModel.ContainsKey(key))
            {
                return _widgetBase = serviceProvider.GetService(KnownWidgetModel[key]) as WidgetBase;
            }
            return _widgetBase;
        }
        public Type GetViewModelType()
        {
            string key = $"{AssemblyName},{ViewModelTypeName}";
            if (KnownWidgetModel.ContainsKey(key))
            {
                return KnownWidgetModel[key];
            }
            return null;
        }

        public IEnumerable<ExtendFieldEntity> ExtendFields { get; set; }

        public WidgetBase ToWidgetBase()
        {
            return new WidgetBase
            {
                AssemblyName = AssemblyName,
                CreateBy = CreateBy,
                CreatebyName = CreatebyName,
                CreateDate = CreateDate,
                Description = Description,
                ID = ID,
                LastUpdateBy = LastUpdateBy,
                LastUpdateByName = LastUpdateByName,
                LastUpdateDate = LastUpdateDate,
                LayoutID = LayoutID,
                PageID = PageID,
                PartialView = PartialView,
                Position = Position,
                ServiceTypeName = ServiceTypeName,
                Status = Status,
                Title = Title,
                ViewModelTypeName = ViewModelTypeName,
                WidgetName = WidgetName,
                ZoneID = ZoneID,
                FormView = FormView,
                StyleClass = StyleClass,
                IsTemplate = IsTemplate,
                Thumbnail = Thumbnail,
                IsSystem = IsSystem,
                ExtendFields = ExtendFields,
            };
        }
    }
    class WidgetBaseMetaData : ViewMetaData<WidgetBase>
    {

        protected override void ViewConfigure()
        {
            ViewConfig(m => m.StyleClass).AsTextBox().MaxLength(1000);
            ViewConfig(m => m.CustomClass).AsHidden().Ignore();
            ViewConfig(m => m.CustomStyle).AsHidden().Ignore();
            ViewConfig(m => m.ExtendFields).AsHidden().Ignore();
        }
    }


}