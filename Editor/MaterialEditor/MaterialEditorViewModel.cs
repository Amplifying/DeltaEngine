using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Editor.ContentManager;
using DeltaEngine.Editor.Core;
using DeltaEngine.Entities;
using DeltaEngine.Rendering.Sprites;
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
			LoadImageAndShaderLists();
			LoadColors();
			LoadMaterials();
		}

		public Material NewMaterial { get; set; }
		private readonly Service service;
		public ObservableCollection<string> ImageList { get; set; }
		public ObservableCollection<string> ShaderList { get; set; }
		public ObservableCollection<string> MaterialList { get; set; }
		private readonly Dictionary<string, Color> ColorList;
		public ObservableCollection<string> ColorStringList { get; set; }

		private void LoadImageAndShaderLists()
		{
			var projectPath = Path.Combine("Content", service.ProjectName);
			CreateListOfLocalContentDataImages(projectPath);
			CreateListOfLocalContentDataShades(projectPath);
		}

		public class NoLocalContentAvailable : Exception {}

		private void CreateListOfLocalContentDataImages(string projectPath)
		{
			ImageList = new ObservableCollection<string>();
			AddContentTypeToContentList(ContentType.Image);
			AddContentTypeToContentList(ContentType.ImageAnimation);
			AddContentTypeToContentList(ContentType.SpriteSheetAnimation);
		}

		private void AddContentTypeToContentList(ContentType type)
		{
			var contentList = service.GetAllContentNamesByType(type);
			foreach (var content in contentList)
				ImageList.Add(content);
		}

		private void CreateListOfLocalContentDataShades(string projectPath)
		{
			ShaderList = new ObservableCollection<string>();
			var contentList = service.GetAllContentNamesByType(ContentType.Shader);
			foreach (var content in contentList)
				ShaderList.Add(content);
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
		}

		private void LoadMaterials()
		{
			MaterialList.Clear();
			var foundmaterials = service.GetAllContentNamesByType(ContentType.Material);
			foreach (var material in foundmaterials)
				MaterialList.Add(material);
			RaisePropertyChanged("MaterialList");
		}

		private void CreateNewMaterial()
		{
			if (SelectedShader == null || SelectedImage == null || SelectedColor == null)
				return;
			NewMaterial = new Material(SelectedShader, SelectedImage);
			NewMaterial.DefaultColor = ColorList[selectedColor];
			EntitiesRunner.Current.Clear();
			new Sprite(NewMaterial, Rectangle.FromCenter(new Point(0.5f, 0.5f), new Size(0.5f, 0.5f)));
		}

		public string SelectedImage
		{
			get { return selectedImage; }
			set
			{
				selectedImage = value;
				CreateNewMaterial();
			}
		}

		private string selectedImage;

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
			var fileNameAndBytes = new Dictionary<string, byte[]>();
			fileNameAndBytes.Add(MaterialName + ".deltamaterial", null);
			var metaDataCreator = new ContentMetaDataCreator(service);
			ContentMetaData contentMetaData = metaDataCreator.CreateMetaDataFromMaterial(MaterialName,
				NewMaterial);
			if (ContentLoader.Exists(MaterialName))
			{
				service.DeleteContent(MaterialName);
				ContentLoader.RemoveResource(MaterialName);
			}
			service.UploadContent(contentMetaData, fileNameAndBytes);
		}

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