/* MyInventory - Keep track of your private assets. 
 * Copyright (C) 2008-2010 Konstantin Weitz
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 3
 * of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301, USA.
 */

/// <summary>
/// About images
/// ============
/// 
/// Each Item can have multiple images assigned.
/// One of those images is the main image.
/// The image is created from an original and is
/// available in two forms.
/// The fullimage, being a copy of the original converted to jpeg
/// And the preview, a small image with a black border also as jpeg.
/// 
/// How images are stored
/// =====================
/// 
/// The preview and fullimage are stored as
/// 
/// "items/"+itemId+"/"+imageId+"_fullimage.jpeg"
/// "items/"+itemId+"/"+imageId+"_preview.jpeg"
/// 
/// Where images are stored
/// =======================
/// 
/// There are two places images may be stored.
/// Once an inventory is saved, all images are stored in
/// 
/// place_of_inventory/items/...
/// 
/// But if an image is added to the inventory and the inventory
/// is not yet saved, it is stored in 
/// 
/// /tmp/modified_inventory/items/...
/// 
/// We need to do it this way in order to loose no old data
/// if the inventory is not saved.
/// 
/// So in order to look up an image, we first check the /tmp/
/// directory. If it isn't there we check the place_of_inventory/
/// 
/// If an image is created, and the file already exists, it is overriden.
/// 
/// If an image is removed, we don't do anything with the filesystem
/// 
/// Once we save, all images from /tmp/ are copied to place_of_inventory/
/// Overriding existing entries.
/// Afterwards all images that are not needed anymore are deleted.
///  
/// E.g.
/// We see how folders chage while we keep modifying the images.
/// We don't show the fullimages, only the previews
/// If a folder is unchanged, it's not mentioned 
/// 
/// Starting with
///   place_of_inventory/1/
///     1_preview.jpeg
///     2_preview.jpeg
///     4_preview.jpeg
///   /tmp/modified_inventory/1/
///   images
///     1,2,4
/// 
/// Adding an image
///   /tmp/modified_inventory/1/
///     3_preview.jpeg
///   images
///     1,2,3,4
/// 
/// Adding an image
///   /tmp/modified_inventory/1/
///     3_preview.jpeg
///     5_preview.jpeg
///   images
///     1,2,3,4,5
/// 
/// Removing image 4
///   images
///     1,2,3,5
///
/// Adding an image 
///   /tmp/modified_inventory/1/
///     3_preview.jpeg
///     4_preview.jpeg
///     5_preview.jpeg
///   images
///     1,2,3,4,5
///  
/// Remove image 3
///   images
///     1,2,4,5
///
/// Remove image 1
///   images
///     2,4,5
///
/// Now we do the saving stepps
/// 
/// Copy all images to place_of_inventory
///   place_of_inventory/1/
///     1_preview.jpeg
///     2_preview.jpeg
///     3_preview.jpeg
///     4_preview.jpeg
///     5_preview.jpeg
///     
/// Remove all files that are not in images
///   place_of_inventory/1/
///     2_preview.jpeg
///     4_preview.jpeg
///     5_preview.jpeg
/// 
/// </summary>

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace MyInventory.Model
{	
	public class Images : PositionableIdCollection<Image,ObservableCollection<Image>>, INotifyPropertyChanged
	{
		private enum ImageState {
			Modified,
			Permanent		
		}
		
		public Images(Item item){
			_item = item;
		}

		private string GetImagesPath(ImageState state){
			string path = (state == ImageState.Modified)?
				Inventory.Settings.ModifiedInventoryPath :
				Inventory.Settings.InventoryPath;
			
			path = Path.Combine(path, "items");
			path = Path.Combine(path, ParentItem.Id.ToString());			
			return path;		
		}
		
		private string GetFullImagePath(uint id, ImageState state)
		{
			
			return Path.Combine(GetImagesPath(state), id.ToString()+"_fullimage.jpeg");
		}
		
		private string GetPreviewPath(uint id, ImageState state)
		{
			return Path.Combine(GetImagesPath(state), id.ToString()+"_preview.jpeg");
		}
		
		private ImageState GetImageState(uint id){
			ImageState[] states = {ImageState.Modified,ImageState.Permanent};
			
			foreach(ImageState state in states){
				string fullImage = GetFullImagePath(id,state);
				if(File.Exists(fullImage)){
					return state;
				}
			}
			
			throw new FileNotFoundException("Unable to load image with item id "+ParentItem.Id.ToString()+
			                                " and image id "+id);
		}
		
		private void CreateNewImageAsModified(uint id, string originalPath){			
			// loading the original			
			string path = GetFullImagePath(id,ImageState.Modified);
			try {
				File.Delete(path);
				File.Delete(GetPreviewPath(id,ImageState.Modified));
			} catch (DirectoryNotFoundException){
				Directory.CreateDirectory(GetImagesPath(ImageState.Modified));
			}	
			File.Copy(originalPath,path);
		}
			
		private void SaveImagePermanently(Image img){
			if(GetImageState(img.Id) == ImageState.Modified){
				string destFullImage = GetFullImagePath(img.Id,ImageState.Permanent);
				string destPreview = GetPreviewPath(img.Id,ImageState.Permanent);
				
				try {
					File.Delete(destFullImage);
					File.Delete(destPreview);
				} catch (DirectoryNotFoundException){
					Directory.CreateDirectory(GetImagesPath(ImageState.Permanent));
				}
					
				string sourceFullImage = GetFullImagePath(img.Id,ImageState.Modified);
				string sourcePreview = GetPreviewPath(img.Id,ImageState.Modified);
			
				File.Move(sourceFullImage,destFullImage);
				// we don't care if the preview doesn't exist we just ignore that
				try { File.Move(sourcePreview,destPreview); }
				catch(FileNotFoundException){}
			}
		}
		
		private void DeleteUnusedImages(){
			string[] files = Directory.GetFiles(GetImagesPath(ImageState.Permanent));
			foreach(string file in files){
				if(file.IndexOf("_") != -1){
					uint id;
					if(uint.TryParse(file.Split(new Char[]{'_'},1)[0], out id)){
						// check if the image does still exist
						foreach(Model.Image img in this)
							if(img.Id == id)
								continue;
						
						File.Delete(GetFullImagePath(id,ImageState.Permanent));
						File.Delete(GetPreviewPath(id,ImageState.Permanent));
					}
				}
			}
		}
				
		private Bitmap CreatePreview(Bitmap original){
			// scale
			float w = Inventory.Settings.PreviewWidth;
			float h = Inventory.Settings.PreviewHeight;
			float oH = original.Height;
			float oW = original.Width;
			
			if(oW/w > oH/h)
				h=(float)Math.Round(h*oH/oW);
			else
				w=(float)Math.Round(w*oW/oH);
			
			float borderW = (float)Inventory.Settings.PreviewBorderWidth;
			
			Bitmap preview = new Bitmap(original,new Size((int)w,(int)h));
			Graphics gra = Graphics.FromImage(preview);
			gra.Clear(Inventory.Settings.PreviewBorderColor);
			// smooting etc will currently be ignored by mono
			gra.SmoothingMode = SmoothingMode.AntiAlias;
    		gra.InterpolationMode = InterpolationMode.HighQualityBicubic;
    		gra.PixelOffsetMode = PixelOffsetMode.HighQuality;
			gra.DrawImage(original,borderW,borderW,w-borderW*2,h-borderW*2);
			
			return preview;
		}
		
		public Image New(string originalPath)
		{
			uint id = UnusedId;
			CreateNewImageAsModified(id,originalPath);
			
			return Load(id);
		}
		
		public Image Load(uint id)
		{
			ImageState state = GetImageState(id);
			
			string fullImagePath = GetFullImagePath(id,state);
			string previewPath = GetPreviewPath(id,state);
			if(!File.Exists(previewPath)){
				Bitmap fullImage = new Bitmap(fullImagePath,true);
				Bitmap preview = CreatePreview(fullImage);
				preview.Save(previewPath);
			}
			
			Image img = new Image(id,fullImagePath,previewPath,this);
			Add(img);
			
			if(MainImage == null)
				MainImage = img;
			
			return img;
		}
		
		public override void Remove(Image img){
			if(object.ReferenceEquals(MainImage,img)){
				MainImage = null;
				// make the first image in position the mainimage
				foreach(Image i in Positions){
					MainImage = i;
					break;
				}
			}
			base.Remove(img);
		}
		
		private Image _mainImage;
		public Image MainImage {
			set {
				_mainImage = value;
				if (PropertyChanged != null)
				   PropertyChanged(this, new PropertyChangedEventArgs("MainImage"));
			}
			get {
				return _mainImage;
			}
		}
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		private Item _item;
		public Item ParentItem {
			get {return _item;}
		}
		public Items Items {
			get {return ParentItem.Items;}
		}
		public Inventory Inventory {
			get {return Items.Inventory;}
		}
		
		public void Serialize(XmlWriter writer) {
			if(Count == 0) return;
			
			writer.WriteStartElement("images");
			if(MainImage != null){
				writer.WriteAttributeString("main-image-id",XmlConvert.ToString(MainImage.Id));
			}
			foreach(Image img in Positions){
				img.Serialize(writer);
				SaveImagePermanently(img);
			}
			writer.WriteEndElement();
			
			DeleteUnusedImages();
		}
		
		public virtual void DeserializeProperties(XPathNavigator nav) {			
			XPathNodeIterator iter = nav.SelectChildren(XPathNodeType.Element);
			foreach(XPathNavigator current in iter){
				uint id = XmlConvert.ToUInt32(current.SelectSingleNode("@id").Value);
				Image image = Load(id);
				image.DeserializeProperties(current);
				Positions.Add(image);
			}
		}
	}
}
