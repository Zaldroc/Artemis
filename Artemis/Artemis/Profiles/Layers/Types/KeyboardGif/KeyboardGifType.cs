﻿using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Media;
using Artemis.Modules.Abstract;
using Artemis.Profiles.Layers.Abstract;
using Artemis.Profiles.Layers.Interfaces;
using Artemis.Profiles.Layers.Models;
using Artemis.Profiles.Layers.Types.Keyboard;
using Artemis.Properties;
using Artemis.Utilities;
using Artemis.ViewModels;
using Artemis.ViewModels.Profiles;

namespace Artemis.Profiles.Layers.Types.KeyboardGif
{
    public class KeyboardGifType : ILayerType
    {
        public string Name => "Keyboard - GIF";
        public bool ShowInEdtor => true;
        public DrawType DrawType => DrawType.Keyboard;
        public int DrawScale => 4;

        public ImageSource DrawThumbnail(LayerModel layer)
        {
            var thumbnailRect = new Rect(0, 0, 18, 18);
            var visual = new DrawingVisual();
            using (var c = visual.RenderOpen())
                c.DrawImage(ImageUtilities.BitmapToBitmapImage(Resources.gif), thumbnailRect);

            var image = new DrawingImage(visual.Drawing);
            return image;
        }

        public void Draw(LayerModel layerModel, DrawingContext c)
        {
            var props = (KeyboardPropertiesModel) layerModel.Properties;
            if (string.IsNullOrEmpty(props.GifFile))
                return;
            if (!File.Exists(props.GifFile))
                return;

            // Only reconstruct GifImage if the underlying source has changed
            if (layerModel.GifImage == null)
                layerModel.GifImage = new GifImage(props.GifFile, props.AnimationSpeed);
            if (layerModel.GifImage.Source != props.GifFile)
                layerModel.GifImage = new GifImage(props.GifFile, props.AnimationSpeed);

            var rect = new Rect(layerModel.X*4, layerModel.Y*4, layerModel.Width*4, layerModel.Height*4);

            lock (layerModel.GifImage)
            {
                var draw = layerModel.GifImage.GetNextFrame();
                using (var drawBitmap = new Bitmap(draw))
                {
                    c.DrawImage(ImageUtilities.BitmapToBitmapImage(drawBitmap), rect);
                }
            }
        }

        public void Update(LayerModel layerModel, ModuleDataModel dataModel, bool isPreview = false)
        {
            layerModel.ApplyProperties(true);
            if (layerModel.GifImage != null)
                layerModel.GifImage.AnimationSpeed = layerModel.Properties.AnimationSpeed;

            if (isPreview || dataModel == null)
                return;

            // If not previewing, apply dynamic properties according to datamodel
            foreach (var dynamicProperty in layerModel.Properties.DynamicProperties)
                dynamicProperty.ApplyProperty(dataModel, layerModel);
        }

        public void SetupProperties(LayerModel layerModel)
        {
            if (layerModel.Properties is KeyboardPropertiesModel)
                return;

            // Set animation speed to 1.5, this translates back to a GIF playback rate of x1
            layerModel.Properties = new KeyboardPropertiesModel(layerModel.Properties) {AnimationSpeed = 1.5};
        }

        public LayerPropertiesViewModel SetupViewModel(LayerEditorViewModel layerEditorViewModel,
            LayerPropertiesViewModel layerPropertiesViewModel)
        {
            var model = layerPropertiesViewModel as KeyboardPropertiesViewModel;
            if (model == null)
                return new KeyboardPropertiesViewModel(layerEditorViewModel)
                {
                    IsGif = true
                };

            model.IsGif = true;
            return layerPropertiesViewModel;
        }
    }
}