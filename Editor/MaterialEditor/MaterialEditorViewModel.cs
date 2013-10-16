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
using DeltaEngine.Rendering2D;
using DeltaEngine.ScreenSpaces;
using GalaSoft.MvvmLight;

namespace DeltaEngine.Editor.MaterialEditor
{
	public class MaterialEditorViewModel : ViewModelBase
	{
		public MaterialEditorViewModel(Service service)
		{
			this.service = service;
			ColorList = new Dictionary<string, Color>();
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
		private readonly Dictionary<string, Color> ColorList;
		public ObservableCollection<string> ColorStringList { get; set; }

		private void FillLists()
		{
			LoadImageAndShaderLists();
			LoadColors();
			LoadMaterials();
			FillListWithBlendModes();
			FillListWithRenderSize();
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
			AnimationList = new ObservableCollection<string>();
			ImageList.Add("");
			AnimationList.Add("");
			AddContentTypeToContentList(ImageList, ContentType.Image);
			AddContentTypeToContentList(ImageList, ContentType.ImageAnimation);
			AddContentTypeToContentList(ImageList, ContentType.SpriteSheetAnimation);
		}

		public ObservableCollection<string> ImageList { get; set; }
		public ObservableCollection<string> AnimationList { get; set; }

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
			ColorList.Add("Black", Color.Black);
			ColorList.Add("White", Color.White);
			ColorList.Add("Blue", Color.Blue);
			ColorList.Add("Cyan", Color.Cyan);
			ColorList.Add("Green", Color.Green);
			ColorList.Add("Orange", Color.Orange);
			ColorList.Add("Pink", Color.Pink);
			ColorList.Add("Purple", Color.Purple);
			ColorList.Add("Red", Color.Red);
			ColorList.Add("Teal", Color.Teal);
			ColorList.Add("Yellow", Color.Yellow);
			ColorList.Add("CornflowerBlue", Color.CornflowerBlue);
			ColorList.Add("LightBlue", Color.LightBlue);
			ColorList.Add("LightGray", Color.LightGray);
			ColorList.Add("DarkGray", Color.DarkGray);
			ColorList.Add("DarkGreen", Color.DarkGreen);
			ColorList.Add("Gold", Color.Gold);
			ColorList.Add("PaleGreen", Color.PaleGreen);
			FillColorStringList();
		}

		private void FillColorStringList()
		{
			foreach (var color in ColorList)
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

		private void FillListWithRenderSize()
		{
			Array enumValues = Enum.GetValues(typeof(RenderSize));
			foreach (var value in enumValues)
				RenderStyleList.Add(Enum.GetName(typeof(RenderSize), value));
			SelectedRenderSize = RenderSize.PixelBased.ToString();
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
			NewMaterial.DefaultColor = ColorList[selectedColor];
			NewMaterial.SetRenderSize(
				(RenderSize)Enum.Parse(typeof(RenderSize), selectedRenderSize, true));
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
					foreach (var colorWithString in ColorList)
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
			Logger.Info("The saving of the material called " + MaterialName + " was a succes.");
			service.ContentUpdated -= SendSuccessMessageToLogger;
		}
		//ncrunch: no coverage end

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