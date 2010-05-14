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

using System.Runtime.InteropServices;
using System;
using Gtk;
using MyInventory.Model;
using System.Collections.ObjectModel;

namespace MyInventory.GtkGui {
	public class CreateIdAdjustment : Adjustment {
		public CreateIdAdjustment(Items items) : base(1,1,1000000000,1,10,0) {
			LastId = 0;
			Items = items;
		}
		protected override void OnValueChanged(){
			Console.WriteLine(Value);
			uint id = (uint)Value;
			if(Items.IsIdUsed(id)){
				if(id < LastId)
					id = Items.GetPrevUnusedId(LastId);
				else
					id = Items.GetNextUnusedId(LastId);
			}
			
			if(id == 0)
				id = LastId;
			else 
				LastId = id;
			
			Value = id;
		}
		
		private Items Items;
		private uint LastId;
	}
	
    public class ItemCreateBox : VBox, IShowMe
    {
		private ItemCreateBox(Builder builder)
		: base(builder.GetRawObject("itemCreateBox"))
		{
			builder.Autoconnect (this);
		}
		
        public ItemCreateBox (Items items)
		: this(new Builder("item_create_box.ui"))
        {
			// add columns to the item type combobox
			CellRenderer render = new CellRendererText ();
			createItemType.PackStart(render, true);
			createItemType.AddAttribute(render, "text", 0);
			
			ListStore list = new ListStore(typeof(string),typeof(Type));
			list.AppendValues("Product",typeof(Product));
			list.AppendValues("Room",typeof(Room));
			list.AppendValues("Estate",typeof(Estate));
			createItemType.Model = list;
			
			// set id adjustment
			createItemId.Adjustment = new CreateIdAdjustment(items);
			
			Items = items;
        }
		
		public void CreateItem()
		{
			if(ShowMe != null)
				ShowMe(this,new ShowMeEventArgs());
			
			createItemId.Value = Items.UnusedId;
			createItemType.Active = 0;
			
			Console.WriteLine("Create Item");
		}
		
		/* These functions are used by glade that gets them using reflection.
		 * Therefore the warning that the function is not used is disabled.
		 */
		#pragma warning disable 169
		private void OnCreateIdInput(object o, InputArgs args) {
			SpinButton spin = (SpinButton)o;
			
			uint id;
			if(spin.Text == "")
				args.NewValue = 0;
			else if( uint.TryParse(spin.Text,out id) && !Items.IsIdUsed(id))
				// if the id is readable and unused
				args.NewValue = id;
			else
				// don't alter the spin buttons value if id is not an int or used
				args.NewValue = spin.Value;
			
			args.RetVal = 1;
		}
	
		private void OnCreateItemDone(object sender, EventArgs args)
		{
			// get the id
			uint id = (uint)createItemId.ValueAsInt;
			
			// get the type
			TreeIter typeIter;
			createItemType.GetActiveIter(out typeIter);
			Type type = (Type)createItemType.Model.GetValue(typeIter,1);
			
			// create the item
			Model.Item item = Items.New(id,type);
			
			Items.Positions.Add(item);
			
			Console.WriteLine("Create Item Done");
		}
		#pragma warning restore
		
		public event ShowMeEventHandler ShowMe;
		
		private readonly Items Items;
		
		[Builder.Object] private ComboBox createItemType;
		[Builder.Object] private SpinButton createItemId;
    }
}