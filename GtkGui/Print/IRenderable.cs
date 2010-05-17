
using Cairo;
using System;
using MyInventory.Model;
using Pango;

namespace MyInventory.GtkGui
{
	public interface IRenderable
	{
		void Render(Cairo.Context cr, Layout layout, double x, double y, double width, double height);
	}
}
