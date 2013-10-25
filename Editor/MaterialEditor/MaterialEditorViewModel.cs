using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Editor.ContentManager;
using DeltaEngine.Editor.Core;
using DeltaEngine.Entities;
using DeltaEngine.Graphics;
using DeltaEngine.Multimedia;
using DeltaEngine.Rendering2D;
using DeltaEngine.ScreenSpaces;
using GalaSoft.MvvmLight;

namespace DeltaEngine.Editor.MaterialEditor
{
	public class MaterialEditorViewModel : ViewModelBase
	{
		public MaterialEditorViewModel(Service service)
		{
			ClearEntities();
			this.service = service;
			colorList = new Dictionary<string, Color>();
			ColorStringList = new ObservableCollection<string>();
			MaterialList = new ObservableCollection<string>();
			BlendModeList = new ObservableCollection<string>();
			RenderStyleList = new ObservableCollection<string>();
			FillLists();
		}

		public Material NewMaterial { get; set; }
		private readonly Service service;
		public ObservableCollection<string> ShaderList { get; set; }
		public ObservableCollection<string> MaterialList { get; set; }
		public ObservableCollection<string> BlendModeList { get; set; }
		public ObservableCollection<string> RenderStyleList { get; set; }
		private readonly Dictionary<string, Color> colorList;
		public ObservableCollection<string> ColorStringList { get; set; }

		private static void ClearEntities()
		{
			var entities = EntitiesRunner.Current.GetAllEntities();
			foreach (var entity in entities)
				if (!entity.GetType().IsSubclassOf(typeof(SoundDevice)))
					entity.IsActive = false;
		}

		private void FillLists()
		{
			LoadImageAndShaderLists();
			LoadColors();
			LoadMaterials();
			FillListWithBlendModes();
			FillListWithRenderSizeModes();
		}

		private void LoadImageAndShaderLists()
		{
			var projectPath = Path.Combine("Content", service.ProjectName);
			CreateListOfLocalContentDataImages(projectPath);
			CreateListOfLocalContentDataShaders(projectPath);
		}

		private void CreateListOfLocalContentDataImages(string projectPath)
		{
			ImageList = new ObservableCollection<string>();
			AddContentTypeToContentList(ImageList, ContentType.Image);
			AddContentTypeToContentList(ImageList, ContentType.ImageAnimation);
			AddContentTypeToContentList(ImageList, ContentType.SpriteSheetAnimation);
		}

		public ObservableCollection<string> ImageList { get; set; }

		private void AddContentTypeToContentList(ObservableCollection<string> contentList,
			ContentType type)
		{
			var contentTypeList = service.GetAllContentNamesByType(type);
			foreach (var content in contentTypeList)
				contentList.Add(content);
		}

		private void CreateListOfLocalContentDataShaders(string projectPath)
		{
			ShaderList = new ObservableCollection<string>();
			var contentList = service.GetAllContentNamesByType(ContentType.Shader);
			foreach (var content in contentList)
			{
				var shader = ContentLoader.Load<Shader>(content) as ShaderWithFormat;
				if (shader.Format.HasUV)
					ShaderList.Add(content);
			}
			SelectedShader = Shader.Position2DColorUV;
			RaisePropertyChanged("SelectedRenderSize");
		}

		private void LoadColors()
		{
			colorList.Add("Black", Color.Black);
			colorList.Add("White", Color.White);
			colorList.Add("Blue", Color.Blue);
			colorList.Add("Cyan", Color.Cyan);
			colorList.Add("Green", Color.Green);
			colorList.Add("Orange", Color.Orange);
			colorList.Add("Pink", Color.Pink);
			colorList.Add("Purple", Color.Purple);
			colorList.Add("Red", Color.Red);
			colorList.Add("Teal", Color.Teal);
			colorList.Add("Yellow", Color.Yellow);
			colorList.Add("CornflowerBlue", Color.CornflowerBlue);
			colorList.Add("LightBlue", Color.LightBlue);
			colorList.Add("LightGray", Color.LightGray);
			colorList.Add("DarkGray", Color.DarkGray);
			colorList.Add("DarkGreen", Color.DarkGreen);
			colorList.Add("Gold", Color.Gold);
			colorList.Add("PaleGreen", Color.PaleGreen);
			FillColorStringList();
		}

		private void FillColorStringList()
		{
			foreach (var color in colorList)
				ColorStringList.Add(color.Key);
			SelectedColor = "White";
			RaisePropertyChanged("SelectedColor");
		}

		private void LoadMaterials()
		{
			MaterialList.Clear();
			var foundmaterials = service.GetAllContentNamesByType(ContentType.Material);
			foreach (var material in foundmaterials)
				MaterialList.Add(material);
			RaisePropertyChanged("MaterialList");
		}

		private void FillListWithBlendModes()
		{
			Array enumValues = Enum.GetValues(typeof(BlendMode));
			foreach (var value in enumValues)
				BlendModeList.Add(Enum.GetName(typeof(BlendMode), value));
			SelectedBlendMode = BlendMode.Normal.ToString();
			RaisePropertyChanged("SelectedBlendMode");
		}

		private void FillListWithRenderSizeModes()
		{
			Array enumValues = Enum.GetValues(typeof(RenderSizeMode));
			foreach (var value in enumValues)
				RenderStyleList.Add(Enum.GetName(typeof(RenderSizeMode), value));
			SelectedRenderSize = RenderSizeMode.PixelBased.ToString();
			RaisePropertyChanged("SelectedRenderSize");
		}

		public string SelectedRenderSize
		{
			get { return selectedRenderSize; }
			set
			{
				selectedRenderSize = value;
				CreateNewMaterial();
			}
		}

		private string selectedRenderSize;

		public string SelectedBlendMode
		{
			get { return selectedBlendMode; }
			set
			{
				selectedBlendMode = value;
				CreateNewMaterial();
			}
		}

		private string selectedBlendMode;

		private void CreateNewMaterial()
		{
			if (SelectedShader == null || SelectedColor == null ||
				(SelectedImage == null && SelectedAnimation == null))
				return;
			if (string.IsNullOrEmpty(SelectedAnimation))
				NewMaterial = new Material(SelectedShader, SelectedImage);
			else
				NewMaterial = new Material(SelectedShader, SelectedAnimation);
			NewMaterial.DefaultColor = colorList[selectedColor];
			NewMaterial.RenderSizeMode =
				(RenderSizeMode)Enum.Parse(typeof(RenderSizeMode), selectedRenderSize, true);
			EntitiesRunner.Current.Clear();
			NewMaterial.DiffuseMap.BlendMode =
				(BlendMode)Enum.Parse(typeof(BlendMode), SelectedBlendMode);
			var shaderWithFormat = NewMaterial.Shader as ShaderWithFormat;
			Draw2DExample(shaderWithFormat);
		}

		private void Draw2DExample(ShaderWithFormat shader)
		{
			if (shader.Format.HasUV)
				new Sprite(NewMaterial,
					Rectangle.FromCenter(Vector2D.Half,
						ScreenSpace.Current.FromPixelSpace(NewMaterial.DiffuseMap.PixelSize)));
		}

		public string SelectedImage
		{
			get { return selectedImage; }
			set
			{
				selectedImage = value;
				selectedAnimation = "";
				CreateNewMaterial();
				RaisePropertyChanged("SelectedAnimation");
			}
		}

		private string selectedImage;

		public string SelectedAnimation
		{
			get { return selectedAnimation; }
			set
			{
				selectedAnimation = value;
				selectedImage = "";
				CreateNewMaterial();
				RaisePropertyChanged("SelectedImage");
			}
		}

		private string selectedAnimation;

		public string SelectedShader
		{
			get { return selectedShader; }
			set
			{
				selectedShader = value;
				CreateNewMaterial();
			}
		}

		private string selectedShader;

		public string MaterialName
		{
			get { return materialName; }
			set
			{
				materialName = value;
				if (ContentLoader.Exists(materialName, ContentType.Material))
				{
					NewMaterial = ContentLoader.Load<Material>(materialName);
					if (NewMaterial.Animation != null)
						SelectedImage = NewMaterial.Animation.Name;
					else if (NewMaterial.SpriteSheet != null)
						SelectedImage = NewMaterial.SpriteSheet.Name;
					else
						SelectedImage = NewMaterial.DiffuseMap.Name;
					SelectedShader = NewMaterial.Shader.Name;
					foreach (var colorWithString in colorList)
						if (NewMaterial.DefaultColor == colorWithString.Value)
							SelectedColor = colorWithString.Key;
					RaisePropertyChanged("SelectedImage");
					RaisePropertyChanged("SelectedShader");
					RaisePropertyChanged("SelectedColor ");
				}
			}
		}

		private string materialName;

		public void Save()
		{
			if (NewMaterial == null || String.IsNullOrEmpty(MaterialName))
				return;
			var metaDataCreator = new ContentMetaDataCreator();
			ContentMetaData contentMetaData = metaDataCreator.CreateMetaDataFromMaterial(MaterialName,
				NewMaterial);
			if (ContentLoader.Exists(MaterialName))
			{
				service.DeleteContent(MaterialName);
				ContentLoader.RemoveResource(MaterialName);
			}
			service.UploadContent(contentMetaData);
			service.ContentUpdated += SendSuccessMessageToLogger;
		}

		//ncrunch: no coverage start 
		private void SendSuccessMessageToLogger(ContentType type, string content)
		{
			Logger.Info("The saving of the material called " + MaterialName + " was a success.");
			service.ContentUpdated -= SendSuccessMessageToLogger;
		}//ncrunch: no coverage end

		public string SelectedColor
		{
			get { return selectedColor; }
			set
			{
				selectedColor = value;
				CreateNewMaterial();
			}
		}

		private string selectedColor;
	}
}