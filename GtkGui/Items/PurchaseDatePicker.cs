using System.Runtime.InteropServices;
using System;
using Gtk;
using MyInventory.Model;
using System.Collections.ObjectModel;

namespace MyInventory.GtkGui {
	public class DateChangedEventArgs : EventArgs {
		public DateChangedEventArgs(DateTime? date)
		{
			Date = date;
		}
		
		public DateTime? Date;
	}
	
	public delegate void DateChangedEventHandler(object sender, DateChangedEventArgs args);

	public class PurchaseDatePicker : Gtk.Window
    {
		private PurchaseDatePicker(Builder builder)
		: base(builder.GetRawObject("purchaseDatePicker"))
		{
			builder.Autoconnect (this);
		}
		
        public PurchaseDatePicker ()
		: this(new Builder("purchase_date_picker.ui"))
        {
			purchaseDateCalendar.DaySelected += OnPurchaseDateChanged;
			purchaseDateCalendar.DaySelectedDoubleClick += OnPurchaseDateChangedDoubleClick;
        }
		
		public void PickDate(DateTime? initTime, Widget alignWidget)
		{
			// set all the values
			if(initTime == null){
				initTime = DateTime.Today;
			}
			purchaseDateAvailable.Active = true;
			purchaseDateCalendar.Date = (DateTime)initTime;
			
			// show the picker
			ShowAll();
				
			// calculate the picker position
			Gtk.Window window = (Window)alignWidget.Toplevel;
			Gdk.Gravity oldWindGravity = window.Gravity;
			window.Gravity = Gdk.Gravity.Static;
			
			Gdk.Rectangle widget;
			Gdk.Point win;
			Gdk.Rectangle popup;
			
			widget = alignWidget.Allocation;
			window.GetPosition(out win.X, out win.Y);
			GetSize(out popup.Width, out popup.Height);
			
			Console.WriteLine(win.Y);
			
			popup.X = win.X + widget.X + widget.Width - popup.Width;
			popup.Y = win.Y + widget.Y + widget.Height;
			
			// if the picker would be outside of the visible screen area
			// pop it up north of the button, default is south
			if(popup.Y + popup.Height > window.Screen.Height){
				popup.Y = win.Y + widget.Y - popup.Height;
			}
			
			Move(popup.X,popup.Y);
			
			window.Gravity = oldWindGravity;
			
			// set the grabs (get all the mouse events even if they aren't over our popup)
			Grab.Add(this);
			Gdk.EventMask mask = Gdk.EventMask.ButtonPressMask|Gdk.EventMask.ButtonReleaseMask|
								 Gdk.EventMask.PointerMotionMask;
			Gdk.Pointer.Grab(this.Child.GdkWindow,true,mask,null,null,Global.CurrentEventTime);
		}
		
		private void Ungrab(){
			Grab.Remove(this);
			Gdk.Pointer.Ungrab(Global.CurrentEventTime);
			HideAll();
		}
		
		private void OnPurchaseDateChangedDoubleClick(object sender, EventArgs args){
			EmitDateChanged(purchaseDateCalendar.Date);
			Ungrab();
		}
		
		private void OnPurchaseDateChanged(object sender, EventArgs args){
			
			Console.WriteLine("DATE PICKED");
			
			if(object.ReferenceEquals(sender,purchaseDateAvailable)){
				if(purchaseDateAvailable.Active){
					purchaseDateCalendar.Sensitive = true;
					EmitDateChanged(purchaseDateCalendar.Date);
				} else {
					purchaseDateCalendar.Sensitive = false;
					EmitDateChanged(null);
				}
			}
			else if(object.ReferenceEquals(sender, purchaseDateCalendar)){
				EmitDateChanged(purchaseDateCalendar.Date);
			}
		}
		
		private void EmitDateChanged(DateTime? date){
			if(DateChanged != null){
				DateChanged(this,new DateChangedEventArgs(date));
			}
		}
		
		public event DateChangedEventHandler DateChanged;
				
		private void OnPurchaseDatePickerEvent(object o, WidgetEventArgs args){
			// hide the picker if click outside
			if(args.Event is Gdk.EventButton){
				Gdk.EventButton evnt = (Gdk.EventButton)args.Event;
	
				Gdk.Point pt = new Gdk.Point((int)evnt.XRoot,(int)evnt.YRoot);
				Gdk.Rectangle rect = Allocation;
				GetPosition(out rect.X, out rect.Y);
				
				if(!rect.Contains(pt)){
					Ungrab();
				}
			}
		}
		
		[Builder.Object] private Calendar purchaseDateCalendar;
		[Builder.Object] private CheckButton purchaseDateAvailable;
    }
}