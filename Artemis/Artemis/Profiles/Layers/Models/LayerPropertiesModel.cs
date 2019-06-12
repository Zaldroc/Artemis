using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using Artemis.Utilities.Converters;
using Newtonsoft.Json;

namespace Artemis.Profiles.Layers.Models
{
    public abstract class LayerPropertiesModel
    {
        private Brush _brush;

        public LayerPropertiesModel(LayerPropertiesModel source = null)
        {
            if (source == null)
                return;

            // Clone the source's properties onto the new properties model (useful when changing property type)
            X = source.X;
            Y = source.Y;
            Width = source.Width;
            Height = source.Height;
            Contain = source.Contain;
            Opacity = source.Opacity;
            AnimationSpeed = source.AnimationSpeed;
            Conditions = source.Conditions;
            LayerKeybindModels = source.LayerKeybindModels;
            ConditionType = source.ConditionType;
            DynamicProperties = source.DynamicProperties;
            Brush = source.Brush;
            HeightEase = source.HeightEase;
            WidthEase = source.WidthEase;
            OpacityEase = source.OpacityEase;
            HeightEaseTime = source.HeightEaseTime;
            WidthEaseTime = source.WidthEaseTime;
            OpacityEaseTime = source.OpacityEaseTime;
        }

        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public bool Contain { get; set; }
        public double AnimationSpeed { get; set; }
        public double OpacityEaseTime { get; set; }
        public double HeightEaseTime { get; set; }
        public double WidthEaseTime { get; set; }
        public string WidthEase { set; get; }
        public string HeightEase { get; set; }
        public string OpacityEase { get; set; }
        public ConditionType ConditionType { get; set; }
        public List<LayerConditionModel> Conditions { get; set; } = new List<LayerConditionModel>();
        public List<LayerKeybindModel> LayerKeybindModels { get; set; } = new List<LayerKeybindModel>();
        public List<DynamicPropertiesModel> DynamicProperties { get; set; } = new List<DynamicPropertiesModel>();

        // Opacity isn't saved since it's only accesable by LUA
        [JsonIgnore]
        public double Opacity { get; set; } = 1;

        [JsonConverter(typeof(BrushJsonConverter))]
        public Brush Brush
        {
            get { return _brush; }
            set
            {
                if (value == null)
                {
                    _brush = null;
                    return;
                }

                if (value.IsFrozen)
                {
                    _brush = value;
                    return;
                }

                // Clone the brush off of the UI thread and freeze it
                var cloned = value.Dispatcher.Invoke(value.CloneCurrentValue);
                cloned.Freeze();
                _brush = cloned;
            }
        }

        public Rect PropertiesRect(int scale = 4)
        {
            return new Rect(X*scale, Y*scale, Width*scale, Height*scale);
        }
    }

    public enum ConditionType
    {
        [Description("All met")] AllMet,
        [Description("Any met")] AnyMet,
        [Description("None met")] NoneMet
    }
}